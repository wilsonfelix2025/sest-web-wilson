const path = require('path');

module.exports = {
  resolve: {
    alias: {
      "sass": path.resolve(__dirname, 'src/sass/'),
      "components": path.resolve(__dirname, 'src/app/components'),
    } 
  }
};