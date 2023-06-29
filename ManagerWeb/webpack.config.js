const webpack5esmInteropRule = {
    test: /\.m?js/,
    resolve: {
        fullySpecified: false
    }
};

module.exports = {
    //mode: "production",
    mode: "development",
    entry: {
        app: './Typescript/App.tsx'
    },
    output: {
        filename: "[name].js",
        path: __dirname + "/wwwroot/js/"
    },
    devtool: "source-map",

    resolve: {
        extensions: [".ts", ".tsx", ".js", "jsx"]
    },

    module: {
        rules: [
            {
                test: /\.ts(x?)$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: "ts-loader"
                    }
                ]
            },
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            },
            webpack5esmInteropRule
        ]
    },
    externals: [
        {
            "react": "React",
            "react-dom": "ReactDOM",
            '@material-ui/core': 'MaterialUI',
            '@material-ui/core/styles': 'MaterialUI',
            "lodash": "_",
            'moment': 'moment'
        }
    ]
};




// // Webpack plugins
// const ForkTsCheckerPlugin = require('fork-ts-checker-webpack-plugin');

// const BUNDLE = {
//     entry: {
//         admin: './Typescript/Admin.tsx',
//         login: "./Typescript/Components/Login/Login.tsx"
//     },
//     output: {
//         filename: "[name].js",
//         path: __dirname + "/wwwroot/js/"
//     }
// };

// module.exports = {
//     entry: BUNDLE.entry,
//     stats: 'normal',
//     module: getLoaders(),
//     plugins: getPlugins(),
//     resolve: {
//         extensions: ['.tsx', '.ts', '.js', '.json']
//     },
//     output: BUNDLE.output,
// };

// /**
//  * Loadery
//  */
// function getLoaders() {
//     const esbuild = {
//         test: /\.(js|jsx|ts|tsx)?$/,
//         loader: 'esbuild-loader',
//         options: {
//             loader: 'tsx',
//             target: 'esnext'
//         },
//         exclude: /node_modules/,
//     };

//     const loaders = {
//         rules: [esbuild,
//             {
//                 test: /\.(s*)css$/,
//                 use: ['style-loader', 'css-loader', 'sass-loader']
//             },
//             {
//                 test: /\.(png|woff|woff2|eot|ttf|svg)$/,
//                 use: [
//                     {
//                         loader: 'url-loader',
//                         options: {
//                             limit: 100000,
//                         },
//                     },
//                 ],
//             },
//             {
//                 test: /\.(ico|jpe?g|png|gif|webp|svg|mp4|webm|wav|mp3|m4a|aac|oga)(\?.*)?$/,
//                 loader: "file-loader"
//             }
//         ]
//     };

//     return loaders;
// }

// /**
//  * Pluginy
//  */
// function getPlugins() {
//     const tsChecker = new ForkTsCheckerPlugin();
//     return [tsChecker];
// }

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
