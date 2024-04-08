const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const { join } = require('path');
const TerserPlugin = require('terser-webpack-plugin');
const ReactRefreshWebpackPlugin = require('@pmmmwh/react-refresh-webpack-plugin');

/** @typedef { import('webpack').Configuration } WebpackConfig */
/** @typedef { import('webpack-dev-server').Configuration } WebpackDevServerConfig */

const outPath = join(__dirname, '../../out');

/** @template [T=object] */
function compact(/** @type T[] */ array) {
  return array.filter(item => !!item);
}

function config(env) {
  const isProd = env['NODE_ENV'] === 'prod';
  const hot = true; //env['HMR_HOT'] === 'true';
  /** @type {WebpackConfig & {devServer: WebpackDevServerConfig}} */
  const config = {
    context: __dirname,
    entry: join(__dirname, 'lib/index.js'),
    mode: isProd ? 'production' : 'development',
    devtool: isProd ? 'source-map' : 'cheap-module-source-map',
    target: 'web',
    bail: true,
    resolve: {
      extensions: ['.js', '.jsx'],
      alias: {},
    },
    node: {
      __dirname: false,
    },
    output: {
      filename: '[name].js',
      path: outPath,
      publicPath: '',
    },
    optimization: {
      concatenateModules: true,
      minimizer: isProd
        ? [
            new TerserPlugin({
              cache: true,
              parallel: true,
              sourceMap: true,
              terserOptions: { output: { comments: false } },
            }),
            new OptimizeCssAssetsPlugin({
              cssProcessorOptions: {
                mergeIdents: true,
              },
              ...{
                cssProcessorPluginOptions: {
                  preset: [
                    'default',
                    { discardComments: { removeAll: true }, mergeIdents: true },
                  ],
                },
              },
            }),
          ]
        : [],
      splitChunks: {
        chunks: 'all',
      },
      runtimeChunk: 'single',
      emitOnErrors: false,
    },
    devServer: {
      hot: !isProd && hot,
      historyApiFallback: true,
      port: 8080,
      host: 'localhost',
      proxy: [
        {
          context: ['/api'],
          target: 'http://localhost:1234',
        },
      ],
    },
    module: {
      rules: [
        {
          test: /\.[tj]sx?$/,
          exclude: /node_modules/,
          use: {
            loader: 'babel-loader',
            options: {
              cacheDirectory: true,
              babelrc: false,
              presets: [['@babel/preset-env', { targets: { chrome: '123' } }]],
              plugins: compact([
                '@babel/plugin-syntax-dynamic-import',
                '@babel/plugin-transform-optional-chaining',
                ['@babel/plugin-transform-class-properties', { loose: true }],
                ['@babel/plugin-transform-object-rest-spread', { loose: true }],
                ['transform-react-remove-prop-types', { mode: 'wrap' }],
                !isProd && 'react-refresh/babel',
              ]),
            },
          },
        },
        {
          test: /\.(woff2?|eot|ttf|png|jpg|gif)$/,
          use: {
            loader: 'url-loader',
            options: {
              limit: 20000,
              fallback: 'file-loader',
            },
          },
        },
        {
          test: /\.css$/,
          use: createCssUse(),
        },
        {
          test: /\.scss$/,
          include: /styles[\\/]main\.scss/,
          use: createCssUse(true, false),
        },
        {
          test: /\.scss$/,
          exclude: /styles[\\/]main\.scss/,
          use: createCssUse(true, true),
        },
        {
          test: /\.jsx?$/,
          use: 'source-map-loader',
          exclude: /node_modules(?![\\/]\@vestaboard-wordle)/,
          enforce: 'pre',
        },
      ],
    },
    plugins: compact([
      new MiniCssExtractPlugin({
        chunkFilename: '[name].css',
        filename: '[name].css',
      }),
      new HtmlWebpackPlugin({
        template: join(__dirname, 'index.ejs'),
        title: 'Vestaboard Wordle',
        filename: 'index.html',
        minify: { collapseWhitespace: true },
      }),
      hot && new ReactRefreshWebpackPlugin(),
    ]),
  };
  return config;
}

function createCssUse(sass = false, modules = false) {
  return compact([
    MiniCssExtractPlugin.loader,
    {
      loader: 'css-loader',
      options: {
        importLoaders: 1,
        modules: modules && {
          localIdentName: '[path][name]-[local]-[hash:base64:5]',
        },
      },
    },
    sass && {
      loader: 'sass-loader',
      options: {
        // Prefer `sass` but not working w/ bp3
        implementation: require('node-sass'),
      },
    },
  ]);
}

module.exports = config;
