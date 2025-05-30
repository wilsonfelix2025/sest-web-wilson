var gulp = require('gulp');
var browserSync = require('browser-sync').create();
var reload      = browserSync.reload;

gulp.task('default', function() {  
console.log("Hi! I'm Gulp default task!");  
}); 

gulp.task('serve', function() {
    browserSync.init({
        server: {
            baseDir: "src"
        }
    });
    gulp.watch("*.html").on("change", reload);
});