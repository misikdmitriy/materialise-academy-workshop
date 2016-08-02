(function () {
    'use strict';

    var application = angular.module("criminalApiModule", ['ngRoute']);

    application.config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when("/", {
            redirectTo: '/criminals'
        })
    }]);
})();