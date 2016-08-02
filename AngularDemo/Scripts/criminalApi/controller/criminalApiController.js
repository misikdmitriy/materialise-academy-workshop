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

            console.log('Deleted');
            console.log(vm.criminals);
        }

        function addNewCriminal() {
            var model = {
                name: vm.newCriminal.name,
                description: vm.newCriminal.description,
                reward: vm.newCriminal.reward,
            }
        }
    }
})();
