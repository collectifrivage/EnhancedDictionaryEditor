angular.module("umbraco")
    .controller("Sigmund.EnhancedDictionaryEditorEditController",
    function ($scope, sigmundDictionaryEditorApiService, $q, navigationService, $routeParams, contentEditingHelper, notificationsService) {

        var defaultDictionaryItemKey = "";
        init();

        $scope.save = function () {
            $scope.loading = true;

            sigmundDictionaryEditorApiService.save($scope.dictionaryItem)
                .then(function () {
                    $scope.dictionaryItemForm.$setPristine();
                    if ($scope.dictionaryItem.Key !== defaultDictionaryItemKey) {
                        updateTreeNodeWithId($scope.dictionaryItem.Id);
                        defaultDictionaryItemKey = $scope.dictionaryItem.Key;
                    }
                    $scope.loading = false;
                    $scope.isNew = false;
                },
                function (err) {
                    contentEditingHelper.handleSaveError({
                        redirectOnFailure: false,
                        err: err
                    });

                    notificationsService.error(err.data.message);
                    $scope.loading = false;
                });
        }

        function updateTreeNodeWithId(id) {
            navigationService.syncTree({ tree: 'EnhancedDictionaryEditorSectionTree', path: [id], forceReload: true, activate: false });
        }

        function init() {
            $scope.loading = true;
            $scope.dictionaryItem = undefined;
            $scope.cultures = [];
            $scope.isNew = false;

            $q.all([
                sigmundDictionaryEditorApiService.GetDictionaryItemById($routeParams.id), 
                sigmundDictionaryEditorApiService.getCultures()
            ]).then(function (result) {
                var dictionaryItem = result[0].data;
                if (dictionaryItem == null) {
                    dictionaryItem = getNewDictionaryItem();
                    $scope.isNew = true;
                }
                var cultures = result[1].data
                $scope.dictionaryItem = dictionaryItem;
                $scope.cultures = cultures;

                defaultDictionaryItemKey = dictionaryItem;
                $scope.loading = false;
            },
            function (err) {
                notificationsService.error(err.data.ExceptionMessage);
            });
        }

        function getNewDictionaryItem() {

            return {
                Key: "",
                Values: {},
                ParentId: $location.search().parent
            };
        }
    });