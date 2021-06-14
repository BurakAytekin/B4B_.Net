b2bApp.controller('resultController', function ($scope, $http) {
    $scope.frameUrl = '';

    $scope.showPdf = function (item) {

        $http({
            method: "POST",
            url: "/Payment/GetPayment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { itemId: item }//, 
        }).then(function (response) {
            $scope.payment = response.data;

            $http({
                method: "POST",
                url: "/Payment/SavePdf",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { eItem: $scope.payment }//, 
            }).then(function (response) {
                $scope.frameUrl = response.data;

                $('#mPdfShow').appendTo("body").modal('show');

                fireCustomLoading(false);

            });

        });



    };


});