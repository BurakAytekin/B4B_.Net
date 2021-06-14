b2bApp.controller('returnProductDetailController', function ($scope, $http) {
    $scope.ReturnForm = { Type: 0, ReturnReason: -1 };
    $scope.selectedManufacturer = '';
    $scope.selectedProductCode = '';

    $scope.convertDate = function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
    };
    
    

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/ReturnProduct/GetReturnProductList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.returnProductList = response.data;
            fireCustomLoading(false);
        });
    }

    $(document).ready(function () {
        $scope.loadData();
    });

    $scope.$on('ngRepeatGroup1Finished', function (ngRepeatGroup1FinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        //$('.selectbox-productgroup1')[0].sumo.reload();
        $('.selectbox-productgroup1').SumoSelect();
        $scope.selectedManufacturer = "";
        $('.selectbox-productgroup1').on('sumo:closed', function (sumo) {

            if ($('.selectbox-productgroup1').val() !== "Seçiniz") {
                $scope.selectedManufacturer = $('.selectbox-productgroup1').val();
            }
        });
    });


}).directive('onFinishRender', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {//ng repeat dönerken son kayıtmı diye bakıyorum
                $timeout(function () {
                    scope.$emit(attr.onFinishRender);
                });
            }
        }
    };
}).filter('convertDate', [
    '$filter', function ($filter) {
        return function (input, format) {
            //  return $filter('date')((new Date(parseInt(input.substr(6))),format(format)));
            return $filter('date')(new Date(parseInt(input.substr(6))), format);
        };
    }
]);