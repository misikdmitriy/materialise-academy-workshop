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
            var index;

            for (index = 0; index < vm.criminals.length; index++) {
                if (vm.criminals[index].id == id) {
                    break;
                }
            }

            vm.criminals.splice(index, 1);
        }

        function replace(criminal) {
            for (var i = 0; i < vm.criminals.length; i++) {
                if (vm.criminals[i].id == criminal.id) {
                    vm.criminals[i] = criminal;
                    break;
                }
            }
        }

        function addNewCriminal() {
            if (!vm.newCriminal.id) {
                postNew();
            } else {
                putAnother();
            }
            
        }

        function putAnother() {
            var model = {
                id: vm.newCriminal.id,
                name: vm.newCriminal.name,
                description: vm.newCriminal.description,
                reward: vm.newCriminal.reward,
            }

            criminalApiFactory.putAnother(model).success(function () {
                console.log('Put');
                replace(model);
                vm.newCriminal = initNewCriminal();
                vm.buttonName = 'Add New';
            })
        }

        function postNew() {
            var model = vm.newCriminal;

            criminalApiFactory.postNew(model).success(function () {
                console.log('Added');
                vm.newCriminal = initNewCriminal();
                vm.criminals.push(model);
            })
        }

        function setForm(criminal) {
            console.log(vm.newCriminalForm);
            console.log(vm.newCriminal);
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
    }
})();
