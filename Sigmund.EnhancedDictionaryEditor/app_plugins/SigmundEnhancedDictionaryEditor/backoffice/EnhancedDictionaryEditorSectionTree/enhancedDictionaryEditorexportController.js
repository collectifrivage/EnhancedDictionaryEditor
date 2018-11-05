angular.module("umbraco")
    .controller("Sigmund.EnhancedDictionaryEditorExportController",
    function ($scope, sigmundDictionaryEditorApiService, navigationService, notificationsService) {

        init();

        $scope.exportItems = function () {
            $scope.loading = true;
            sigmundDictionaryEditorApiService.getDictionaryKeysXml()
            .then(function (result) {
                downloadData(result);
                $scope.loading = false;
            }, function (err) {
                $scope.loading = false;
                notificationsService.error(err.data.ExceptionMessage);
            });
        };

        function init() {
            $scope.loading = false;
        }

        function downloadData(result) {
            var blob = new Blob([result.data], { type: "application/xml;charset=utf-8;" });
            var downloadLink = angular.element('<a></a>');
            downloadLink.attr('href', window.URL.createObjectURL(blob));
            downloadLink.attr('download', 'export.xml');
            downloadLink[0].click();
        }
    });