b2bApp.controller('financeDetailController', ['$scope', '$http', '$element', function ($scope, $http, $element) {


    $scope.loadFinanceDetailData = function (DocumentNo) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Finance/GetFinanceDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { DocumentNo: DocumentNo }
        }).then(function (response) {
            $scope.financeDetailList = response.data;

            console.log($scope.financeDetailList);
            $scope.getSubTotals($scope.financeDetailList);
        });
    };


    $scope.getSubTotals = function (list) {

        $http({
            method: "POST",
            url: "/Finance/GetDetailSubTotals",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { list: list }
        }).then(function (response) {
            $scope.subTotalItem = response.data;
            fireCustomLoading(false);
        });

    };


    $(document).ready(function () {
        $scope.loadFinanceDetailData($scope.getParameterByName('DocumentNo'));
    });

    $scope.getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };

}]);