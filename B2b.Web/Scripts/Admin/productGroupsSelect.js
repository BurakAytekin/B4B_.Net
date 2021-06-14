adminApp.controller('ProductGroupsSelectController', function ($scope, $http) {

    // #region Veriables
    var mProductGroupSelect = "#mProductGroupSelect";
    $scope.ProductGroup1 = "";
    $scope.ProductGroup2 = "";
    $scope.ProductGroups1 = {};
    $scope.ProductGroups2 = {};
    $scope.ProductGroups3 = {};
    // #endregion

    $scope.productGroupsOpen = function () {
        $(mProductGroupSelect).appendTo("body").modal('show');
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsGroup1List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.ProductGroups1 = response.data;
        });
    };

    $scope.getListProductGroup2 = function (productGroup1) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsGroup2List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { productGroup1: productGroup1 }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.ProductGroups2 = response.data;
            $scope.ProductGroup1 = productGroup1;
        });

    };

    $scope.getListProductGroup3 = function (productGroup2) {
        var productGroup1 = $scope.ProductGroup1;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsGroup3List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { productGroup1: productGroup1, productGroup2: productGroup2 }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.ProductGroups3 = response.data;
            $scope.ProductGroup2 = productGroup2;
        });
    };

    $scope.productGroupSelectAll = function (productGroup3) {
        $scope.selectedProduct.ProductGroup1 = $scope.ProductGroup1;
        $scope.selectedProduct.ProductGroup2 = $scope.ProductGroup2;
        $scope.selectedProduct.ProductGroup3 = productGroup3;
        $(mProductGroupSelect).modal("hide");
    };

});