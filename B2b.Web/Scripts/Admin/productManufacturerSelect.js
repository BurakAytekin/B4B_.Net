adminApp.controller('ProductManufacturerSelectController', function ($scope, $http) {
    // #region Veriables
    var mManufacturerSelect = "#mManufacturerSelect";
    $scope.productManufacturerList = {};
    // #endregion
    $scope.manufacturerSelect = function (manufacturer) {
        $scope.selectedProduct.Manufacturer = manufacturer.Name;
        $(mManufacturerSelect).modal('hide');
    };

    $scope.productManufacturerOpen = function () {
        $(mManufacturerSelect).appendTo("body").modal('show');
        if ($scope.productManufacturerList.length > 0)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);            
            $scope.productManufacturerList = response.data;
        });
    };

    $scope.$on('ngProductManufacturerFinished', function (ngProductManufacturerFinishEvent) {

        $('#tProductManufacturerTable').DataTable({
            searching: true,
            language: {
                url: "/Scripts/Admin/vendor/datatables/js/Turkish.json"
            },
            destroy: true,
            paging: false,
            scrollY: 300
        });
    });

})
