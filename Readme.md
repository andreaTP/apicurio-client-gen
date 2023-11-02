# Deprecated

Please checkout the successor: https://github.com/redhat-developer/kiota-wasm

# Apicurio Client Generation

This project uses [Kiota]((https://github.com/microsoft/kiota)) in the browser to generate client SDKs for OpenAPI specs.

## Build

To build this project you need `dotnet` version 7+ and the `wasm-tools`:

```bash
dotnet workload install wasm-tools
```

The main [Kiota project](https://github.com/microsoft/kiota) is supposed to be available as a sibling folder.

```bash
dotnet build --configuration Release
```

## Release

This repository is released on tag:

```
git fetch origin
git checkout origin/main
git tag <version>
git push origin <version>
```

and the CI will do the job.
