module.exports = {
    //mode: "production",
    mode: "development",
    entry: {
        app: './Typescript/Overview.tsx',
        menu: './Typescript/Components/Menu.tsx',
        crypto: './Typescript/Crypto.tsx',
        bankAccounts: './Typescript/BankAccounts.tsx',
        auth: './Typescript/Components/Auth/Auth.tsx'
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
            // 'material-ui': 'window["material-ui"]'
            '@material-ui/core': 'MaterialUI',
            '@material-ui/core/styles': 'MaterialUI',
            // _: 'lodash'
            "lodash": "_",
            'moment': 'moment'
        }
        // , /@material-ui\/core\/.*/
    ]
};