var gulp = require("gulp");
var sass = require('gulp-sass');

gulp.task('sass', function () {
    return gulp.src('./Sass/**/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest('./wwwroot/css/'));
});

gulp.task('watchsass', function () {
    gulp.watch('./Sass/**/*.scss', gulp.series('sass'));
});