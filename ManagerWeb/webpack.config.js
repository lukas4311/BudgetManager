module.exports = {
    //mode: "production",
    mode: "development",
    entry: {
        app: './Typescript/Overview.tsx',
        menu: './Typescript/Components/Menu.tsx',
        crypto: './Typescript/Crypto.tsx'
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
            }
        ]
    },
    externals: [
        {
        "react": "React",
        "react-dom": "ReactDOM",
        'material-ui': 'window["material-ui"]'
        }
    ]
};