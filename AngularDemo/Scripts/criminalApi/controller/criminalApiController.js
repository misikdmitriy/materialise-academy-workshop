(function () {
    'use strict';

    angular
        .module('criminalApiModule')
        .controller('criminalApiController', criminalApiController);

    criminalApiController.$inject = ['criminalApiFactory'];

    function criminalApiController(criminalApiFactory) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'criminalApiController';
        vm.service = criminalApiFactory;
        vm.delete = deleteCriminal;
        vm.create = addNewCriminal;
        vm.setForm = setForm;
        vm.buttonName = 'Add New';
        vm.newCriminal = initNewCriminal();

        activate();

        function activate() {
            criminalApiFactory.getAll().success(function (data) {
                vm.criminals = data;
            });
        }

        function deleteCriminal(id) {
            criminalApiFactory.deleteById(id || vm.id).success(function () {
                deleteById(id || vm.id);
            })
        }

        function deleteById(id) {
            for (var i = 0; i < vm.criminals.length; i++) {
                if (vm.criminals[i].id == id) {
                    vm.criminals.splice(i, 1);
                    return;
                }
            }
        }

        function replaceById(criminal) {
            for (var i = 0; i < vm.criminals.length; i++) {
                if (vm.criminals[i].id == criminal.id) {
                    vm.criminals[i] = criminal;
                    return;
                }
            }
        }

        function addByUrl(location) {
            criminalApiFactory.getByLocation(location).success(function (data) {
                vm.criminals.push(data);
            })
        }

        function addNewCriminal() {
            if (!vm.newCriminal.id) {
                postNew();
            } else {
                putAnother();
            }
        }

        function putAnother() {
            var model = getNewCriminal();

            criminalApiFactory.putAnother(model).success(function () {
                replaceById(model);
                vm.newCriminal = initNewCriminal();
                vm.buttonName = 'Add New';
            })
        }

        function postNew() {
            var model = getNewCriminal();

            criminalApiFactory.postNew(model).success(function (data, status, headers, config) {
                addByUrl(headers().location);
                vm.newCriminal = initNewCriminal();
                
            })
        }

        function setForm(criminal) {
            vm.newCriminal.id = criminal.id;
            vm.newCriminal.name = criminal.name;
            vm.newCriminal.reward = criminal.reward;
            vm.newCriminal.description = criminal.description;
            vm.buttonName = 'Replace';
        }

        function initNewCriminal() {
            return {
                id: undefined,
                name: "",
                description: "",
                reward: 0
            }
        }

        function getNewCriminal() {
            return {
                id: vm.newCriminal.id,
                name: vm.newCriminal.name,
                description: vm.newCriminal.description,
                reward: vm.newCriminal.reward,
            }
        }
    }
})();
