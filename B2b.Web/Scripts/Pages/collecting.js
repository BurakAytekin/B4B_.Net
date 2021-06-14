b2bApp.controller('IndexController', function ($scope, $http) {
    // #region veriables
    $scope.selectedTab = 99; //0:Nakit 1:Çek 2:Senet 3:Kredi Kartı

    $scope.formatClearFilters = function () {
        $scope.collecting =
        {
            Currency: "TL",
            AmountKrs: "00"

        };

        //Default değerlere ihtiyaçım varsa burda veriyorum
        $('.datetimepicker').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: new Date(),
            locale: "tr"
        });
    };
    $scope.saveCollecting = function (collecting) {
        if ($scope.selectedTab === 99) {
            $scope.showErrorMessage("Tahsilat işlemi için Veri girişi yapılmalı");
            return;
        }
        if (collecting.Amount === undefined || collecting.Amount === "0" || collecting.Amount === "") {
            $scope.showErrorMessage("Tutar girişi yapılmalı");
            return;
        }
        var collectingType = $scope.selectedTab;
        switch ($scope.selectedTab) {
            case 1:
                $scope.collecting.DueDate = $("#checkDueDate").val();
                $scope.collecting.DrawerDate = $("#checkDrawDate").val();
                break;
            case 2:
                $scope.collecting.DueDate = $("#contractDueDate").val();
                $scope.collecting.DrawerDate = $("#contractDrawDate").val();
                break;

        }
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/SaveCollecting",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { collectingType: collectingType, collecting: collecting }

        }).then(function (response) {
            $scope.formatClearFilters();
            $scope.loadData();
            iziToast.show({
                message: response.data.Message,
                position: 'topRight',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);

        });

    };
    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/LoadInsertedLines",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.headersData = response.data;
            fireCustomLoading(false);

        });

    };
    $scope.deleteItem = function (collecting) {
        var id = collecting.Id;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/Delete",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topRight',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
            $scope.loadData();

        });
    };
    $scope.sendCollecting = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/SendCollecting",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.formatClearFilters();
            $scope.headersData = {};
            //$scope.loadData();
            $scope.selectedTab = 99;
        });
    };
    $scope.fireBankList = function () {

        $http({
            method: "POST",
            url: "/Collecting/GetBankList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.bankList = response.data;
            $('#modal-text').modal();
        });


    };
    $scope.showErrorMessage = function (message) {
        iziToast.error({
            message: message,
            position: 'topRight'
        });
    }

    $(document).ready(function () {
        $scope.formatClearFilters();

        $('.selectbox').SumoSelect();
        $('.datetimepicker').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: new Date(),
            locale: "tr"
        });
        $scope.loadData();
    });

}).directive('onFinishRender', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) { //ng repeat dönerken son kayıtmı diye bakıyorum
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