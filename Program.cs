using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Kiota.Builder;
using Kiota.Builder.Configuration;
using System.Threading;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Versioning;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace apicurio
{
    [SupportedOSPlatform("browser")]
    public partial class KiotaClientGen
    {

        // Mandatory `main` for WASM modules
        static void Main(string[] args)
        {
            Console.WriteLine("Hello browser!");
        }

        private static readonly ThreadLocal<HashAlgorithm> HashAlgorithm = new(() => SHA256.Create());
        private static readonly CancellationTokenSource source = new CancellationTokenSource();
        private static readonly CancellationToken token = source.Token;

        [JSExport]
        internal async static Task<string> Generate(string spec, string language, string clientClassName, string namespaceName, string includePatterns, string excludePatterns)
        {
            var cl = new ConsoleLogger();
            ILogger<KiotaBuilder> consoleLogger = cl;

            try
            {
                Console.WriteLine($"Starting to Generate with parameters: {language}, {clientClassName}, {namespaceName}");

                var defaultConfiguration = new GenerationConfiguration();

                var hashedUrl = BitConverter.ToString(HashAlgorithm.Value!.ComputeHash(Encoding.UTF8.GetBytes(spec))).Replace("-", string.Empty);
                string OutputPath = Path.Combine(Path.GetTempPath(), "kiota", "generation", hashedUrl);

                if (File.Exists(OutputPath))
                {
                    Console.WriteLine("Deleting OutputPath");
                    File.Delete(OutputPath);
                }
                Directory.CreateDirectory(OutputPath);

                string filename = "openapi.";
                if (isJson(spec))
                {
                    filename += "json";
                }
                else
                {
                    filename += "yaml";
                }

                string OpenapiFile = Path.Combine(Path.GetTempPath(), filename);
                if (File.Exists(OpenapiFile))
                {
                    Console.WriteLine("Deleting OpenapiFile");
                    File.Delete(OpenapiFile);
                }
                await File.WriteAllTextAsync(OpenapiFile, spec);

                if (!Enum.TryParse<GenerationLanguage>(language, out var parsedLanguage))
                {
                    throw new ArgumentOutOfRangeException($"Not supported language: {language}");
                }

                var generationConfiguration = new GenerationConfiguration
                {
                    OpenAPIFilePath = OpenapiFile,
                    IncludePatterns = (includePatterns is null) ? new() : includePatterns?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(static x => x.Trim()).ToHashSet(),
                    ExcludePatterns = (excludePatterns is null) ? new() : excludePatterns?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(static x => x.Trim()).ToHashSet(),
                    Language = parsedLanguage,
                    OutputPath = OutputPath,
                    ClientClassName = clientClassName,
                    ClientNamespaceName = namespaceName,
                    IncludeAdditionalData = false,
                    UsesBackingStore = false,
                    Serializers = defaultConfiguration.Serializers,
                    Deserializers = defaultConfiguration.Deserializers,
                    StructuredMimeTypes = defaultConfiguration.StructuredMimeTypes,
                    DisabledValidationRules = new(),
                    CleanOutput = true,
                    ClearCache = true,
                };

                try
                {
                    var builder = new KiotaBuilder(consoleLogger, generationConfiguration, new HttpClient());
                    var result = await builder.GenerateClientAsync(token).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Lock"))
                    {
                        Console.WriteLine("Problems during the Lock file write, ignoring", e);
                    }
                    else
                    {
                        throw;
                    }
                }

                var zipFilePath = Path.Combine(Path.GetTempPath(), "kiota", "clients", hashedUrl, "client.zip");
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);
                else
                    Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);

                ZipFile.CreateFromDirectory(OutputPath, zipFilePath);

                byte[] fileBytes = File.ReadAllBytes(zipFilePath);
                string base64Content = System.Convert.ToBase64String(fileBytes);
                return base64Content;
            }
            catch (Exception e)
            {
                var errorMessage = "Error:\n" + e + "\nLogs:\n" + cl.GetAllLogs();
                throw new Exception(errorMessage);
            }
        }

        private static bool isJson(string str)
        {
            try
            {
                JsonValue.Parse(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    class ConsoleLogger : ILogger<KiotaBuilder>
    {
        private StringBuilder sb = new StringBuilder();
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return new DummyDisposable();
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            sb.AppendLine(formatter(state, exception));
            Console.WriteLine(formatter(state, exception));
        }

        public string GetAllLogs()
        {
            return sb.ToString();
        }
    }

    class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
            // Do nothing
        }
    }

}
