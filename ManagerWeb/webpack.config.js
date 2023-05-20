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