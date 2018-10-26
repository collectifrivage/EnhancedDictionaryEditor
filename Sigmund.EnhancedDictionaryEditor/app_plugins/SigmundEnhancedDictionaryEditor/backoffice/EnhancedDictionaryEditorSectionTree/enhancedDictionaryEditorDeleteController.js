angular.module("umbraco")
    .controller("Sigmund.EnhancedDictionaryEditorDeleteController",
    function ($scope, sigmundDictionaryEditorApiService, navigationService, treeService) {

        init();

        $scope.deleteItem = function () {
            $scope.loading = true;
            sigmundDictionaryEditorApiService.deleteItem($scope.currentNode.id)
            .then(function () {
                treeService.removeNode($scope.currentNode);
                navigationService.hideMenu();
                $scope.loading = false;
            }, function (err) {
                $scope.loading = false;
                notificationsService.error(err.data.ExceptionMessage);
            });
        }

        $scope.cancel = function () {
            navigationService.hideDialog();
        }

        function init() {
            $scope.loading = false;
        }
    });