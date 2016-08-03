(function () {
    'use strict';

    angular
        .module('criminalApiModule')
        .factory('criminalApiFactory', criminalApiFactory);

    criminalApiFactory.$inject = ['$http'];

    function criminalApiFactory($http) {
        var service = {
            getAll: getAll,
            getByLocation: getByLocation,
            deleteById: deleteById,
            postNew: postNew,
            putAnother: putAnother
        };

        return service;

        function getAll() {
            return $http.get('/api/criminal/');
        }

        function getByLocation(location) {
            return $http.get(location);
        }

        function deleteById(id) {
            return $http.delete('/api/criminal/' + id);
        }

        function postNew(criminal) {
            return $http.post('/api/criminal/', criminal);
        }

        function putAnother(criminal) {
            return $http.put('/api/criminal/', criminal);
        }
    }
})();