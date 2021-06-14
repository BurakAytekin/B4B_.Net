b2bApp.controller('paymentController', ['$scope', '$http', function ($scope, $http) {
    var screenWidth = window.screenWidth;

   


    $(document).ready(function () {
       
    });
   
}]).directive('onFinishRender', function ($timeout) {
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
});