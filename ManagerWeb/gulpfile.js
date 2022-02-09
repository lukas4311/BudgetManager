var gulp = require("gulp");
var sass = require('gulp-sass')(require('sass'));
const webpack_stream = require('webpack-stream');
const webpack_config = require('./webpack.config.js');

gulp.task('webpack', () => {
    return webpack_stream(webpack_config)
        .pipe(gulp.dest('./wwwroot/js/'));
});

gulp.task('sass', function () {
    return gulp.src('./Sass/**/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest('./wwwroot/css/'));
});

gulp.task('watchsass', function () {
    gulp.watch('./Sass/**/*.scss', gulp.series('sass'));
});

gulp.task('watchwebpack', function () {
    gulp.watch('./Typescript/**/*.{tsx,ts,js,jsx}', gulp.series('webpack'));
});

gulp.task('watch', function () {
    gulp.watch('./Sass/**/*.scss', gulp.series('sass'));
    gulp.watch('./Typescript/**/*.{tsx,ts,js,jsx}', gulp.series('webpack'));
});