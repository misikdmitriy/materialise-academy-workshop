(function () {
    'use strict';

    angular
        .module('criminalApiModule')
        .factory('criminalApiFactory', criminalApiFactory);

    criminalApiFactory.$inject = ['$http'];

    function criminalApiFactory($http) {
        var service = {
            getAll: getAll,
            deleteById: deleteById
        };

        return service;

        function getAll() {
            return $http.get('/api/criminal');
        }

        function deleteById(id) {
            return $http.delete('/api/criminal/' + id);
        }
    }
})();