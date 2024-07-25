const webpackMerge = require("webpack-merge");

const baseWebpackConfig = require("@kentico/xperience-webpack-config");

const path = require("path");

module.exports = (opts, argv) => {
    const baseConfig = (webpackConfigEnv, argv) => {
        return baseWebpackConfig({
            // Sets the organizationName and projectName
            // The JS module is registered on the backend using these values
            orgName: "hbs",
            projectName: "xperience-transformable-views",
            webpackConfigEnv: webpackConfigEnv,
            argv: argv
        });
    };

    const projectConfig = {
        module: {
            rules: [
                {
                    test: /\.(js|ts)x?$/,
                    exclude: [/node_modules/],
                    loader: "babel-loader",
                },
                {
                    test: /\.css$/i,
                    exclude: [/node_modules/],
                    use: ["style-loader", "css-loader"],
                },
            ],
        },
        output: {
            clean: true
        },
        // Webpack server configuration. Required when running the boilerplate in 'Proxy' mode.
        devServer: {
            port: 3009,
        },
    };

    return webpackMerge.merge(projectConfig, baseConfig(opts, argv));
};
