adminApp.controller('VehicleSelectController', function ($scope, $http, NgTableParams) {
    // #region Veriables
    var mVehicleSelect = "#mVehicleSelect";
    $scope.vechicleBrandName = "";
    $scope.vechicleModelName = "";
    // #endregion

    $scope.vehicleSelecetOpen = function () {

        $(mVehicleSelect).appendTo("body").modal('show');
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetVehicleBrandList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.tableVehicleBrandParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            fireCustomLoading(false);
        });
    };
    $scope.getVehicleModelList = function (brand) {
        $scope.vechicleBrandName = brand;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetVehicleModelList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { brand: brand }
        }).then(function (response) {
            
            $scope.tableVehicleBrandModelParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            fireCustomLoading(false);
        });
    };
    $scope.getVehicleModelTypeList = function (model) {
        $scope.vechicleModelName = model;
        var brand = $scope.vechicleBrandName;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetVehicleTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { brand: brand, model: model }
        }).then(function (response) {
            $scope.tableVehicleBrandModelTypeParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            fireCustomLoading(false);

        });
    };
    $scope.insertSelectedVehicle = function (row) {
        var selectedVehicleId = row.VehicleId;
        var product = $scope.selectedProduct;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/InsertVehicle",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { product: product, selectedVehicleId: selectedVehicleId }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getVehicleBrandModel();
            $(mVehicleSelect).modal("hide");
            fireCustomLoading(false);

        });
    };
});