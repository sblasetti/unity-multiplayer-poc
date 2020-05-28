const path = require('path');
const nodeExternals = require('webpack-node-externals');
const FileManagerPlugin = require('filemanager-webpack-plugin');

const apiConfig = {
    entry: {
        main: './src/main.ts',
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    target: 'node',
    plugins: [
        new FileManagerPlugin({
            onStart: {
                delete: ['./dist'],
            },
        }),
    ],
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    externals: [nodeExternals()],
    output: {
        filename: '[name].bundle.js',
        path: path.resolve(__dirname, 'dist'),
    },
};

module.exports = [apiConfig];
