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
        }

        $scope.translate = function (culture) {
            var firstNotEmptyText = "";
            var firstNotEmptyCulture = "";
            for (var key in $scope.dictionaryItem.Values) {
                var value = $scope.dictionaryItem.Values[key];
                
                if (value) {
                    firstNotEmptyText = value;
                    firstNotEmptyCulture = key;
                    break;
                }
            }
            sigmundDictionaryEditorApiService.translate(firstNotEmptyText, firstNotEmptyCulture, culture.Name)
                .then(function (result) {
                    $scope.dictionaryItem.Values[culture.Name] = result.data;
                });
        }

        $scope.shouldDisplayTranslateButton = function (buttonCulture) {
            if (!$scope.isTranslationAvailable || $scope.dictionaryItem.Values[buttonCulture.Name]) return false;

            var shouldDisplay = false;
            $scope.cultures.forEach(function (culture) {
                if ($scope.dictionaryItem.Values[culture.Name]) shouldDisplay = true;
            });

            return shouldDisplay;
        }

        function updateTreeNodeWithId(id) {
            navigationService.syncTree({ tree: 'EnhancedDictionaryEditorSectionTree', path: [id], forceReload: true, activate: false });
        }

        function init() {
            $scope.loading = true;
            $scope.dictionaryItem = undefined;
            $scope.cultures = [];
            
            $q.all([
                sigmundDictionaryEditorApiService.getDictionaryItemById($routeParams.id), 
                sigmundDictionaryEditorApiService.getCultures(),
                sigmundDictionaryEditorApiService.isTranslationAvailable()
            ]).then(function (result) {
                $scope.dictionaryItem = result[0].data ? result[0].data : getNewDictionaryItem();
                $scope.cultures = result[1].data
                $scope.isTranslationAvailable = result[2].data == "true";

                defaultDictionaryItemKey = $scope.dictionaryItem;
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