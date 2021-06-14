b2bApp.controller('PaymentListController', function ($scope, $http) {
    $(document).ready(function() {
        $http({
            method: "POST",
            url: "/Payment/PaymenProcesstList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {  }

        }).then(function (response) {
            $scope.paymnetAllList = response.data;
           
        });
    });

    $scope.showPdf = function (item) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Payment/SavePdf",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { eItem: item }//, 
        }).then(function (response) {
            $scope.frameUrl = response.data;

            $('#mPdfShow').appendTo("body").modal('show');

            fireCustomLoading(false);

        });

    };
    
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