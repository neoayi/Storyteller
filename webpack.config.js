var ProgressPlugin = require('./util/progress-plugin');
module.exports = {
  entry: {
    "bundle": ['./client/client.js'],
    "batch-bundle": ['./client/batch.js'],
    "embed": ['./client/embed.js'],
    "topics": ['./client/topics']
  },
  output: {
    path: __dirname + '/src/ST',
    filename: "[name].js",
    publicPath: '/client/public/javascript/',
    pathinfo: true
  },
  resolve: {
    // Allow to omit extensions when requiring these files
    extensions: ['', '.js', '.jsx']
  },
  plugins: [
    ProgressPlugin()
  ],
  module: {
    loaders: [
      { test: /\.css$/, loader: "style!css" },
      {
        test: /\.jsx?$/,
        loader: 'babel', // 'babel-loader' is also a legal name to reference 
        query: {
          presets: ['react', 'es2015']
        }
      },
      {
        test: /\.js$/,
        loader: 'babel', // 'babel-loader' is also a legal name to reference 
        query: {
          presets: ['es2015']
        }
      }
    ]
  },
  devtool: 'eval'
}
