b2bApp.controller('ReturnProductController', function ($scope, $http) {
    $scope.ReturnForm = { Type: 0, ReturnReason: -1 };
    $scope.selectedManufacturer = '';
    $scope.selectedProductCode = '';

    $scope.convertDate = function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
    };

    $scope.manufacturerChanged = function () {
        $scope.findInvoice();
    }

    $scope.saveReturnProduct = function () {
        fireCustomLoading(true);
        $scope.ReturnForm.InvoiceDate = $scope.convertDate($scope.ReturnForm.InvoiceDate);

        if ($scope.ReturnForm.Quantity === undefined || $scope.ReturnForm.Quantity === null || $scope.ReturnForm.Quantity === '' || $scope.ReturnForm.Quantity <= 0 || $scope.ReturnForm.ReturnReason === -1) {

            iziToast.error({
                message: 'Lütfen miktar ve iade nedeni girdiğinizden emin olunuz.',
                position: 'topCenter',
            });
            return;
        }

        $http({
            method: "POST",
            url: "/ReturnProduct/SaveReturnForm",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { retrunForm: $scope.ReturnForm }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
            $scope.clear();
        });



    };

    $scope.setValues = function (item) {
        $scope.ReturnForm.ProductCode = angular.copy(item.ProductCode);
        $scope.ReturnForm.ProductName = angular.copy(item.ProductName);
        $scope.ReturnForm.ProductManufacturer = angular.copy(item.Manufacturer);
        $scope.ReturnForm.ProductManufacturerCode = angular.copy(item.ManufacturerCode);
        $scope.ReturnForm.InvoiceDate = angular.copy(item.Date);
        $scope.ReturnForm.InvoiceNumber = angular.copy(item.DocumentNo);
        $scope.ReturnForm.Price = angular.copy(item.Price);
        $scope.ReturnForm.MaxQuantity = angular.copy(item.Quantity);
        $scope.ReturnForm.EntegreId = angular.copy(item.EntegreId);
        $scope.ReturnForm.StlineRef = angular.copy(item.StlineRef);
        $scope.ReturnForm.InvoiceRef = angular.copy(item.InvoiceRef);
        $('#modal-invoive').modal('hide');
    };

    $scope.checkQuantity = function (item) {
        if (item.Quantity > item.MaxQuantity) {
            item.Quantity = angular.copy(item.MaxQuantity);
            iziToast.error({
                message: 'Fatura miktarından fazla iade edemezsiniz',
                position: 'topCenter',
            });
            return;
        }
    }

    $scope.findInvoice = function () {
        fireCustomLoading(true);
        $scope.clear();
        $http({
            method: "POST",
            url: "/ReturnProduct/FindInvoice",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { manufacturer: $scope.selectedManufacturer, productCode: $scope.selectedProductCode }

        }).then(function (response) {
            $scope.IncoiveList = response.data;
            if ($scope.IncoiveList.length === 1)
                $scope.setValues($scope.IncoiveList[0]);
            else
                $('#modal-invoive').modal('show');

            fireCustomLoading(false);
        });
    }


    $scope.clear = function () {
        $scope.ReturnForm = { Type: 0, ReturnReason: -1 };
    };


    $scope.getManufacturerList = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/ReturnProduct/GetManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.manufacturerList = response.data;
            fireCustomLoading(false);
        });
    }

    $(document).ready(function () {
        $scope.getManufacturerList();
        $scope.selectedProductCode = $scope.getParameterByName('ProductCode');
        if ($scope.selectedProductCode !== '') 
            $scope.findInvoice();
    });

    $scope.getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };

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