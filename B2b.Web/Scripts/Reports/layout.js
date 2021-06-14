reportApp.controller('layoutController', ['$scope', '$http', function ($scope, $http) {

    $scope.loadMenu = function () {
        $http({
            method: "POST",
            url: "/Report/Home/GetMenuList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { reportIsActive: 0 }
        }).then(function (response) {
            $scope.menuList = response.data;
        });
    };



    $(document).ready(function () {
        $scope.loadMenu();
    });
}]);