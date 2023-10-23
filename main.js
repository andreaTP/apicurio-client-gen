import { dotnet } from './dotnet.js';

let kiotaExports;

export async function generate(spec, language, clientClassName, namespaceName, includePatterns, excludePatterns) {
    if (kiotaExports === undefined) {
        console.debug('Initializing Kiota exports');
        const { getAssemblyExports, getConfig } = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();
    
        const config = getConfig();
        kiotaExports = await getAssemblyExports(config.mainAssemblyName);
    }

    console.debug('Running Kiota generate function');
    return await kiotaExports.apicurio.KiotaClientGen.Generate(spec, language, clientClassName, namespaceName, includePatterns, excludePatterns);
}
