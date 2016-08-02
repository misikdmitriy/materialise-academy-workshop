(function () {
    'use strict';

    angular
        .module('criminalApiModule')
        .directive('validNumber', validNumber);

    validNumber.$inject = ['$filter'];

    function validNumber($filter) {
        // Usage:
        //     <valid-number-directive></valid-number-directive>
        // Creates:
        // 
        var directive = {
            link: link,
            require: '?ngModel',
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$formatters.unshift(function (a) {

                if (!ngModelCtrl.$modelValue || document.activeElement === attrs.$$element[0])
                    return ngModelCtrl.$modelValue;

                var result = getEnhancedFilter(ngModelCtrl.$modelValue);
                return result;
            });

            ngModelCtrl.$parsers.unshift(function (viewValue) {

                if (!viewValue)
                    return viewValue;

                var numberRegex = /^[1-9]{1,5}$/;

                if (!numberRegex.test(viewValue)) {
                    viewValue = ngModelCtrl.$modelValue || 1;
                    renderValue(viewValue);
                }

                function renderValue(value) {
                    ngModelCtrl.$setViewValue(getEnhancedFilter(value));
                    ngModelCtrl.$render();
                }

                return viewValue;
            });

            function getEnhancedFilter(value) {
                return $filter('number')(value, 0).replace(/\s/g, '');
            }
        }
    }

})();