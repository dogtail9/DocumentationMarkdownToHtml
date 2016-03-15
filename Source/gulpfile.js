"use strict";

var gulp = require("gulp"),
    data = require('gulp-data'),
    img64 = require('gulp-img64'),
    markdown = require('gulp-markdown'),
    rename = require('gulp-rename'),
    replace = require('gulp-replace'),
    path = require('path'),
    fs = require('fs'),
    highlight = require('highlight.js'),
    livereload = require('gulp-livereload'),
    wait = require('gulp-wait');

var paths = {
    Template: "./Template.html",
    Markdown: "./Markdown/*.md",
    dist: "./Markdown/",
    temp: "./temp/"
};

var minPort = 1000;
var maxPort = 1998;
var myPort = Math.floor(Math.random() * (maxPort - minPort + 1) + minPort);

gulp.task('Watch', function() {
    livereload.listen({ port: myPort, basePath: paths.dist });
    gulp.watch([paths.Markdown, paths.Template], ['BuildDocumentation']);
})

gulp.task('BuildDocumentation', ['GenerateHtmlFromMarkdown'], function() {
    var BuildNumber, time, i = process.argv.indexOf("--BuildNumber");
    if (i > -1) {
        BuildNumber = '<p class="buildNumber">' + process.argv[i + 1] + '</p>';
         time = 0;
    }
    else {
        BuildNumber = '<p class="buildNumber">DevBuild</p>\n<script src="http://localhost:' + myPort + '/livereload.js?snipver=1"></script>';
         time = 1000;
    }

    console.log(process.argv);
    console.log(BuildNumber);

    return gulp.src(paths.temp + '*.html')
        .pipe(data(function(file) {
            return gulp.src(paths.Template)
                .pipe(replace('@@@HTML@@@', fs.readFileSync(file.path, "utf8")))
                .pipe(replace('@@@BuildName@@@', BuildNumber))
                .pipe(replace('.md', '.html'))
                .pipe(rename(path.basename(file.path)))
                .pipe(gulp.dest(paths.dist))
                .pipe(wait(time))
                .pipe(livereload());
        }))
});

gulp.task('GenerateHtmlFromMarkdown', function() {
    return gulp.src(paths.Markdown)
        .pipe(markdown({
            highlight: function(code) {
                return highlight.highlightAuto(code).value;
            }
        }))
        .pipe(img64())
        .pipe(gulp.dest(paths.temp));
});
