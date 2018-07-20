const path = require('path');

module.exports = {
  entry: {
    vendor: '@aspnet/signalr',
    custom: './src/chat.js'
  },
  output: {
    path: path.resolve(__dirname, './dist'),
    filename: '[name].bundle.js'
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        use: ["source-map-loader"],
        enforce: "pre"
      }
    ]
  },
  optimization: {
    minimize: false,
    occurrenceOrder: true,
    removeAvailableModules: false,
    removeEmptyChunks: false,
    mergeDuplicateChunks: false
  },
};