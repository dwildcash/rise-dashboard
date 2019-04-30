﻿/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');

// Node_modules deps
var deps = {
    "admin-lte": {
        "/dist/css/adminlte.min.css": "css",
        "/dist/js/adminlte.min.js": "js"
    },
    "bootstrap": {
        "/dist/css/bootstrap.min.css": "css",
        "/dist/js/bootstrap.bundle.min.js": "js"
    },
    "odometer": {
        "/themes/odometer-theme-slot-machine.css": "css",
        "/odometer.min.js": "js"
    },
    "chart.js": {
        "/dist/Chart.min.js": "js"
    },
    "@fortawesome/": {
        "/fontawesome-free/webfonts/*": "webfonts",
        "/fontawesome-free/css/all.css": "css",
        "/fontawesome-free/js/all.js": "js"
    },
    "pe7-icon": {
        "/dist/fonts/*": "fonts",
        "/dist/dist/pe-icon-7-stroke.min.css": "css"
    },
    "ionicons": {
        "/dist/fonts/*": "fonts",
        "/dist/css/ionicons.min.css": "css"
    },
    "datatables.net": {
        "/js/*": "js"
    },
    "datatables.net-bs4": {
        "/js/*": "js",
        "/css/*": "css"
    },
    "datatables.net-responsive-bs4": {
        "/css/*": "css",
        "/js/*": "js"
    },
    "datatables.net-responsive": {
        "/js/*": "js"
    },
    "datatables.net-fixedheader-bs4": {
        "/css/fixedHeader.bootstrap4.min.css": "css",
        "/js/fixedHeader.bootstrap4.min.js": "js"
    },
    "pace-js": {
        "/pace.min.js": "js"
    },
    "jquery": {
        "/dist/jquery.min.js": "js"
    },
    "jquery-knob": {
        "/dist/jquery.knob.min.js": "js"
    },
    "jquery-sparkline": {
        "/jquery.sparkline.min.js": "js"
    },
    "jquery-ui-touch-punch": {
        "jquery.ui.touch-punch.min.js": "js"
    },
    "jvectormap-next": {
        "/jquery-jvectormap.min.js": "js",
        "/jquery-jvectormap.css": "css"
    },
    "jquery-ui-dist": {
        "/jquery-ui.min.js": "js",
        "/jquery-ui.min.css": "css"
    }
};

gulp.task('clean', function () {
    return del([
        'wwwroot/js/**/*',
        'wwwroot/fonts/**/*',
        'wwwroot/css/**/*',
        'wwwroot/json/**/*',
        '!wwwroot/css/jquery-ui-autocomplete.css',
        '!wwwroot/js/jquery-jvectormap-world-mill.js',
        '!wwwroot/js/roulette.js',
        '!wwwroot/css/pace.css'
    ]);
});

// Copy required scripts
gulp.task("scripts", function () {
    var streams = [];

    for (var prop in deps) {
        console.log("Prepping wwwroot for: " + prop);
        for (var itemProp in deps[prop]) {
            streams.push(gulp.src("node_modules/" + prop + "/" + itemProp)
                .pipe(gulp.dest("wwwroot/" + deps[prop][itemProp])));
        }
    }

    return streams;
});

gulp.task("default", ['clean', 'scripts']);