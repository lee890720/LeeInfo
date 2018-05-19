$(function () {
    'use strict'
    var myLayout = [
        'fixed',
        'layout-boxed',
        'sidebar-collapse',
        'sidebar-mini-expand-feature',
    ];

    function get(name) {
        if (typeof (Storage) !== 'undefined') {
            return localStorage.getItem(name)
        } else {
            window.alert('Please use a modern browser to properly view this template!')
        }
    };

    function store(name, val) {
        if (typeof (Storage) !== 'undefined') {
            localStorage.setItem(name, val)
        } else {
            window.alert('Please use a modern browser to properly view this template!')
        }
    };

    function changeLayout(cls) {
        for (var i = 0; i < myLayout.length; i++) {
            $("body").removeClass(myLayout[i]);
        }
        $('body').addClass(cls);
        store('layout', cls);
    };
    var tmp = get('layout');
    if (tmp && $.inArray(tmp, myLayout))
        changeLayout(tmp);

    $("#fixed").click(function () {
        changeLayout("fixed  sidebar-mini-expand-feature");
    });

    $("#boxed").click(function () {
        changeLayout("layout-boxed");
    });
    $("#sidebar-collapse").click(function () {
        changeLayout("sidebar-collapse");
    });
    $("#sidebar-mini").click(function () {
        changeLayout("sidebar-mini");
    });
});