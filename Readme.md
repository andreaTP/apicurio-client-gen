# Kiota-Wasm

This project brings the power of [Kiota](https://github.com/microsoft/kiota) straight into your browser!
[Kiota](https://github.com/microsoft/kiota) is a C# project and the `dotnet` toolchain allows you to compile it with target `browser-wasm`and eventually run it in the browser.

## Usage

There are a few minor challenges with the integration of the produced artifacts into a real-world, modern, frontend application.
We do maintain working examples up to date to make sure that it's feasible to leverage this functionality.

1. Add this dependency to your project:

`npm install -D @redhat-developer/kiota-wasm`

2. Unpack the resources in the public assets folder of your project (and we suggest to add it to your `.gitignore`):

```bash
npm install -D copyfiles rimraf
```

```json
  "scripts": {
    ...
    "postinstall": "rimraf ./public/kiota-wasm && copyfiles -u 4 'node_modules/@redhat-developer/kiota-wasm/dist/**/*.*' 'public/kiota-wasm'"
  }
  ...
```

3. With some build tool such as `vite` you need to exclude those dependencies from the bundler:

```ts
export default defineConfig({
  plugins: [
    react(),
  ],
  build: {
    rollupOptions: {
      external: [
        /^.*kiota-wasm.*/,
      ]
    }
  }
})
```

4. Now you can integrate the WASM module with a dynamic import, with `vite`:

```ts
// @ts-ignore
const { generate } = await import('/kiota-wasm/main.js?url');
```

or with `webpack`:

```ts
const { generate } = await import(
  /* webpackIgnore: true */ './kiota-wasm/main.js'
);
```

5. Use the code generator functionality:

```ts
generate(spec: string, language: string, clientClassName: string, namespaceName: string, includePatterns: string, excludePatterns: string);
```


## Build

To build this project you need `dotnet` version 7+ and the `wasm-tools`:

```bash
dotnet workload install wasm-tools
```

and run the build:

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
