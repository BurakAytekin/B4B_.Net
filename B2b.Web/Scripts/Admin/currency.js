adminApp.controller('currencyController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.currency = {
        Id: 0,
        Type: '',
        Rate: '',//0
        Icon: '',
        CheckBist: false
    };

    $scope.updateCurrency = function () {
        $http({
            method: "POST",
            url: "/Admin/Currency/UpdateCurrencyBist",
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

            $scope.loadData();

        });
    };


    $scope.loadData = function () {
        $scope.currencyDefault = angular.copy($scope.currency);
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Currency/GetCurrencyList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.currencyList = response.data;
            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.currencyList)
            });
        });
    };


    $scope.openModal = function (type, row) {


        if (type == 'Add') {
            $scope.addMode = true;
            $scope.editMode = false;
            $scope.currency = angular.copy($scope.currencyDefault);
            $('#modal-currency').modal('show');


        } else if (type == 'Edit') {
            $scope.addMode = false;
            $scope.editMode = true;
            $scope.currency = angular.copy(row);
            $('#modal-currency').modal('show');

        } else if (type == 'Delete') {
            $scope.tempRow = [];
            $scope.tempRow = row;
            $('#modal-currency-delete').modal('show');
        };
    };

    $scope.delete = function (Id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Currency/DeleteCurrency",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: Id }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $('#modal-currency-delete').modal('hide');

            $scope.loadData();

        });
    };



    $scope.validateCurrency = function () {
        $scope.ValidateError = true;
        $scope.ValidateResult = '';

        if (($scope.currency.Type == '' || $scope.currency.Type == undefined)
            || ($scope.currency.Rate == '' || $scope.currency.Rate < 1 || $scope.currency.Rate == undefined)
            || ($scope.currency.Icon == '' || $scope.currency.Icon == undefined)
            || ($scope.currency.CheckBist == undefined)
            ) {
            $scope.ValidateError = 'Lütfen Tüm Alanları Doldurunuz';
            $scope.ValidateResult = false;
        }

        var rateJs = $scope.currency.Rate;
        var formattedRate = rateJs.toString().replace('.', ',');

        $scope.currency.Rate = formattedRate;

    };

    $scope.addOrUpdate = function () {


        $scope.validateCurrency();

        if ($scope.ValidateResult === false) {
            iziToast.error({
                message: $scope.ValidateError,
                position: 'topCenter'
            });
        }
        else {
            fireCustomLoading(true);

            $http({
                method: "POST",
                url: "/Admin/Currency/AddOrUpdate",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { currency: $scope.currency }

            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });

                $('#modal-currency').modal('hide');

                $scope.currency = angular.copy($scope.currencyDefault);
                $scope.loadData();
            });

        };
    };

    $(document).ready(function () {
        $scope.loadData();
    });

}]);