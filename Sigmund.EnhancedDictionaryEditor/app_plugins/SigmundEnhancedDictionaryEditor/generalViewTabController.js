angular.module("umbraco")
    .controller("Sigmund.EnhancedDictionaryEditorGeneralViewTabController",
    function ($scope, sigmundDictionaryEditorApiService, $q, $timeout, $location, appState) {

        init();

        $scope.search = function () {
            $timeout(function () {//Wait that angular refresh the model "$scope.searchForm.searchTerm"
                if ($scope.searchForm.searchTerm) {
                    $scope.displayedDictionnaryKeys = $scope.allDictionnaryKeys.filter(isItemContainsSearchTerm);
                } else {
                    $scope.displayedDictionnaryKeys = $scope.allDictionnaryKeys;
                }
            });
        }

        $scope.editItem = function (dictionaryItem) {
            var currentSection = appState.getSectionState("currentSection");
            $location.path("/" + currentSection + "/EnhancedDictionaryEditorSectionTree/edit/" + dictionaryItem.Id);
        }

        function isItemContainsSearchTerm(dictionaryItem) {
            if (dictionaryItem.Key.indexOf($scope.searchForm.searchTerm) !== -1) return true;

            for (var property in  dictionaryItem.Values) {
                if (dictionaryItem.Values.hasOwnProperty(property) &&
                    dictionaryItem.Values[property].indexOf($scope.searchForm.searchTerm) !== -1) return true;
            }

            return false;
        }

        function init() {
            $scope.loading = true;
            $scope.displayedDictionnaryKeys = [];
            $scope.allDictionnaryKeys = [];
            $scope.cultures = [];
            $scope.searchForm = { searchTerm: "" };

            $q.all([
                sigmundDictionaryEditorApiService.getAll(),
                sigmundDictionaryEditorApiService.getCultures()
            ]).then(function (result) {
                var dictionaryItems = result[0].data;
                var cultures = result[1].data

                $scope.allDictionnaryKeys = dictionaryItems;
                $scope.displayedDictionnaryKeys = dictionaryItems;
                $scope.cultures = cultures;
                $scope.loading = false;
            },
            function (err) {
                notificationsService.error(err.data.ExceptionMessage);
            });
        }
    });