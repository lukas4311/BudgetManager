const ForkTsCheckerPlugin = require('fork-ts-checker-webpack-plugin');

const BUNDLE = {
    entry: {
        app: './Typescript/App.tsx'
    },
    output: {
        filename: "[name].js",
        path: __dirname + "/wwwroot/js/"
    }
};

module.exports = {
    entry: BUNDLE.entry,
    stats: 'normal',
    mode: 'development',
    devtool: "source-map",
    module: getLoaders(),
    plugins: getPlugins(),
    resolve: {
        extensions: ['.tsx', '.ts', '.js', '.json']
    },
    output: BUNDLE.output,
};

/**
 * Loadery
 */
function getLoaders() {
    const esbuild = {
        test: /\.(js|jsx|ts|tsx)?$/,
        loader: 'esbuild-loader',
        options: {
            loader: 'tsx',
            target: 'esnext'
        },
        exclude: /node_modules/,
    };

    const loaders = {
        rules: [esbuild,
            {
                test: /\.(s*)css$/,
                use: ['style-loader', 'css-loader', 'sass-loader']
            }
        ]
    };

    return loaders;
}

/**
 * Pluginy
 */
function getPlugins() {
    const tsChecker = new ForkTsCheckerPlugin();
    return [tsChecker];
}

// DEV FILE
// const { merge } = require('webpack-merge');
// const common = require('./webpack.config.js');
// const SpeedMeasurePlugin = require("speed-measure-webpack-plugin");

// module.exports = merge(common, {
//     mode: 'development',
//     devtool: "source-map"
// });

// /**
//  * Odkomentuj pro trackovani a zakomentuj kod nad
//  */
// // const smp = new SpeedMeasurePlugin();

// // module.exports = smp.wrap(merge(common, {
// //     mode: 'development',
// //     devtool: "source-map"
// // }));
