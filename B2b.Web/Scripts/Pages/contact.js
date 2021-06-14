b2bApp.controller('contactController', ['$scope', '$rootScope', '$http', function ($scope, $rootScope, $http) {




    $scope.getListFAQ = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Contact/GetListFAQ",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.faqList = response.data;
            fireCustomLoading(false);
        });
    };

    $(document).ready(function () {
        $scope.getListFAQ();

    });

}]);