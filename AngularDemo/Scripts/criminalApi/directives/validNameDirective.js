(function() {
    'use strict';

    angular
        .module('criminalApiModule')
        .directive('validName', validName);

    validName.$inject = ['$window'];
    
    function validName($window) {
        // Usage:
        //     <valid-name-directive></valid-name-directive>
        // Creates:
        // 
        var directive = {
            link: link,
            require: '?ngModel',
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attrs, ngModelCtrl) {
            //alert('Dimchik');

            ngModelCtrl.$parsers.unshift(function (value) {
                var valid = value == 'Dima';
                ngModelCtrl.$setValidity('correct', valid);
                return valid ? value : undefined;
            });

            //ngModelCtrl.$formatters.unshift(function (value) {
            //    ngModelCtrl.$setValidity('correct', value == 'Dima');
            //    return value.toLowerCase();
            //});
        }
    }

})();