angular.module("umbraco")
    .controller("Sigmund.EnhancedDictionaryEditorImportController",
    function ($scope, sigmundDictionaryEditorApiService, notificationsService) {

        init();
        var file = undefined;

        function init() {
            $scope.loading = false;
            $scope.importButtonDisabled = true;
        }

        $scope.importXml = function () {
            $scope.loading = true;
            $scope.importButtonDisabled = true;

            var fileReader = new FileReader();
            fileReader.onload = (e) => {
                sigmundDictionaryEditorApiService.uploadDictionaryKeysXml(fileReader.result)
                .then(function () {
                    $scope.loading = false;
                    //To reload the entire tree.
                    window.location.reload(false);
                }, function (err) {
                    $scope.loading = false;
                    notificationsService.error(err.data.ExceptionMessage);
                });
            }
            fileReader.readAsText(file);
        }

        $scope.$on("filesSelected", function (ev, args) {
            $scope.importButtonDisabled = true;

            var selectedFile = args.files[0];
            if (!selectedFile.name.endsWith(".xml") || selectedFile.type != "text/xml") {
                notificationsService.error("The selected file must be in xml format");
                file = null;
                $scope.importButtonDisabled = true;
            } else {
                file = selectedFile;
                $scope.importButtonDisabled = false;
            }
        });
    });