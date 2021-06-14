adminApp.controller('reportSeritakipController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

 
    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Report/GetMarsSeriList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.settingsList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.settingsList)
            });

            $scope.originalData = angular.copy($scope.settingsList);

        });
    };

    $(document).ready(function () {
        $scope.loadData();
    });

}]);