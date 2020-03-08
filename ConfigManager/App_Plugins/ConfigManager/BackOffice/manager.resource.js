(function () {
    "use strict";

    function configManagerResource(umbRequestHelper, $http) {
        return {

            saveThemeFile: function (configFile) {
                console.log(umbRequestHelper);
                return umbRequestHelper.resourcePromise(
                    $http.post(
                        umbRequestHelper.getApiUrl(
                            "configManagerEditorsBaseUrl",
                            "PostSaveConfigFile"), configFile),
                    "Failed to delete theme");
            },

            //deleteItem: function (id) {

            //    return umbRequestHelper.resourcePromise(
            //        $http.post(
            //            umbRequestHelper.getApiUrl(
            //                "configManagerEditorApiBaseUrl",
            //                "PostDeleteItem", { id: id })),
            //        "Failed to delete theme");
            //},

            //copyTheme: function (themeName, copy) {

            //    return umbRequestHelper.resourcePromise(
            //        $http.post(
            //            umbRequestHelper.getApiUrl(
            //                "articulateThemeEditorApiBaseUrl",
            //                "PostCopyTheme", { themeName: themeName, copy: copy })),
            //        "Failed to retrieve themes");
            //},

            //createFile: function (parentId, fileName, type) {

            //    return umbRequestHelper.resourcePromise(
            //        $http.post(
            //            umbRequestHelper.getApiUrl(
            //                "articulateThemeEditorApiBaseUrl",
            //                "PostCreateFile", { parentId, name: fileName, type: type })),
            //        "Failed to retrieve themes");
            //},

            //getThemes: function (virtualpath) {

            //    return umbRequestHelper.resourcePromise(
            //        $http.get(
            //            umbRequestHelper.getApiUrl(
            //                "configManagerEditorApiBaseUrl",
            //                "GetCofigFile")),
            //        "Failed to retrieve themes");
            //},

            getByPath: function (virtualpath) {
                console.log("getByPath");
                console.log(umbRequestHelper.getApiUrl(
                    "configManagerEditorsBaseUrl", "GetByPath"));
                return umbRequestHelper.resourcePromise(
                    $http.get(
                        umbRequestHelper.getApiUrl(
                            "configManagerEditorsBaseUrl",
                            "GetByPath",
                            { virtualPath: virtualpath })),
                    "Failed to retrieve data from virtual path " + virtualpath);
            }
        };
    }

    angular.module("umbraco").factory("configManagerResource", configManagerResource);


})();