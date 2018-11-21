angular.module('umbraco.resources').factory('sigmundDictionaryEditorApiService',
    function ($http, $q) {

        var apiUrl = "/umbraco/backoffice/SigmundEnhancedDictionaryEditor/EnhancedDictionaryEditor/";

        return {
            deleteItem: function (id) {
                return $http.delete(apiUrl + "DeleteById?id=" + id);
            },
            getChildren: function (item) {
                return $http.get(apiUrl + "GetChildren/" + item.Id);
            },
            getDictionaryItemById: function (id) {
                var defered = $q.defer();

                if (!id) defered.resolve({ data: null });
                else defered.resolve($http.get(apiUrl + "GetDictionaryItemById?id=" + encodeURI(id)));

                return defered.promise;
            },
            getCultures: function () {
                return $http.get(apiUrl + "GetCultures");
            },
            getRootKeys: function () {
                return $http.get(apiUrl + "GetRoot");
            },
            getAll: function () {
                return $http.get(apiUrl + "GetAll");
            },
            save: function (saveData) {
                return $http.post(apiUrl + "Save/", saveData);
            },
            getDictionaryKeysXml: function () {
                return $http.get(apiUrl + "ExportXml/");
            },
            uploadDictionaryKeysXml: function (xml) {
                var req = {
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': "text/plain"
                    },
                    dataType: 'xml'
                };

                return $http.post(apiUrl + "ImportXml/", xml, req);
            }
        };
    });