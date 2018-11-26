angular.module("umbraco")
    .controller("Sigmund.EnhancedDictionaryEditorEditController",
    function ($scope, sigmundDictionaryEditorApiService, $q, $location, navigationService, $routeParams, contentEditingHelper, notificationsService) {

        var defaultDictionaryItemKey = "";
        init();

        $scope.save = function () {
            $scope.loading = true;

            sigmundDictionaryEditorApiService.save($scope.dictionaryItem)
                .then(function (result) {
                    $scope.dictionaryItemForm.$setPristine();
                    if ($scope.dictionaryItem.Key !== defaultDictionaryItemKey) {
                        updateTreeNodeWithId($scope.dictionaryItem.Id);
                        defaultDictionaryItemKey = $scope.dictionaryItem.Key;
                    }
                    $scope.dictionaryItem.Id = result.data.Id;
                    $scope.loading = false;
                },
                function (err) {
                    contentEditingHelper.handleSaveError({
                        redirectOnFailure: false,
                        err: err
                    });

                    notificationsService.error(err.data.message);
                    $scope.loading = false;
                });
        };

        $scope.isNew = function () {
            return $routeParams.id ? false : !$scope.dictionaryItem.Id;
        };

        function updateTreeNodeWithId(id) {
            navigationService.syncTree({ tree: 'EnhancedDictionaryEditorSectionTree', path: [id], forceReload: true, activate: false });
        }

        function init() {
            $scope.loading = true;
            $scope.dictionaryItem = undefined;
            $scope.cultures = [];
            
            $q.all([
                sigmundDictionaryEditorApiService.getDictionaryItemById($routeParams.id), 
                sigmundDictionaryEditorApiService.getCultures()
            ]).then(function (result) {
                var dictionaryItem = result[0].data;
                if (dictionaryItem == null) {
                    dictionaryItem = getNewDictionaryItem();
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