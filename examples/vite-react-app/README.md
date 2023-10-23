# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react/README.md) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type aware lint rules:

- Configure the top-level `parserOptions` property like this:

```js
   parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module',
    project: ['./tsconfig.json', './tsconfig.node.json'],
    tsconfigRootDir: __dirname,
   },
```

- Replace `plugin:@typescript-eslint/recommended` to `plugin:@typescript-eslint/recommended-type-checked` or `plugin:@typescript-eslint/strict-type-checked`
- Optionally add `plugin:@typescript-eslint/stylistic-type-checked`
- Install [eslint-plugin-react](https://github.com/jsx-eslint/eslint-plugin-react) and add `plugin:react/recommended` & `plugin:react/jsx-runtime` to the `extends` list

## Using Kiota Wasm in Vite

Copy the static assets coming from the kiota-wasm packge in `/public`, for example you can automate the process with this configuration in `package.json`:

```json
  "scripts": {
    ...
    "postinstall": "rm -rf ./public/kiota-wasm && copy-files-from-to"
  },
  "copyFiles": [
    {
        "from": "node_modules/@andreatp/test-kiota-wasm/dist/*",
        "to": "public/kiota-wasm"
    },
    {
        "from": "node_modules/@andreatp/test-kiota-wasm/dist/managed/*",
        "to": "public/kiota-wasm/managed"
    },
    {
        "from": "node_modules/@andreatp/test-kiota-wasm/dist/supportFiles/*",
        "to": "public/kiota-wasm/supportFiles"
    }
  ],
  ...
```

Exclude the `kiota-wasm` folders from rollup:

```json
  build: {
    rollupOptions: {
      external: [
        /^.*kiota-wasm.*/,
      ]
    }
  }
```

Now you can add the code using the wasm library to your app:

```js
// @ts-ignore
const { generate } = await import('/kiota-wasm/main.js?url');
generate("", "", "", "", "", "")
```
