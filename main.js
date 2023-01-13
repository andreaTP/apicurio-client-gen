import { dotnet } from './dotnet.js'

export async function generate(spec, language, clientClassName, namespaceName) {
    if (window.kiota.exports === undefined) {
        const { getAssemblyExports, getConfig } = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();
    
        const config = getConfig();
        window.kiota.exports = await getAssemblyExports(config.mainAssemblyName);
    }

    return await window.kiota.exports.KiotaClientGen.Generate(spec, language, clientClassName, namespaceName);
}
