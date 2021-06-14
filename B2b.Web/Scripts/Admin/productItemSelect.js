adminApp.controller('ProductItemSelectController', function ($scope, $http, $parse) {
    // $scope.searchItemCriteria = {};
    // #region Veriables
    var mProductItemSearchTable;
    $scope.selectedItemProduct = {};
    var mProductItemSearch = "#mProductItemSearch";
    $scope.veriableName;
    // #endregion

    $scope.generalItemSearch = function (searchItemCriteria) {
        $scope.selectedItemProduct = {};
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductSearch",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { searchCriteria: searchItemCriteria }
        }).then(function (response) {
            $scope.fillItemData(response.data, $scope.veriableName);
            fireCustomLoading(false);
        });
    };

    $scope.selectItemProduct = function (product) {
        var model = $parse($scope.veriableName);
        model.assign($scope, product);
        console.log($scope.alternativeItemProduct);
        $(mProductItemSearch).modal('hide');
    };

    $scope.fillItemData = function (searchData) {
        var mProductSearchTableName = '#mProductItemSearchTable'; //Id Or Class

        mProductItemSearchTable = $(mProductSearchTableName).DataTable({
            data: searchData,
            columns: [
                { defaultContent: '<button class="btn btn-xs btn-rounded btn-primary">Seçiniz</button>', className: "text-right ws-nowrap" },
                { title: "Ürün Kodu", data: "Code" },
                { title: "Üretici Kodu", data: "ManufacturerCode", className: "text-left ws-nowrap" },
                { title: "Ürün Adı", data: "Name", className: "ws-nowrap overflow-hidden scroll-on-hover ellipsis" },
                { title: "Üretici", data: "Manufacturer", className: "text-left ws-nowrap" },
                { title: "Birim", data: "Unit", className: "text-center ws-nowrap" },
                { title: "Koşul", data: "RuleCode", className: "text-center ws-nowrap" },
                { title: "Raf Adresi", data: "ShelfAddress", className: "text-center ws-nowrap" },
                { title: "Resim", data: "HavePicture", className: "text-center ws-nowrap" },
                { title: "Satış Fiyat", data: "SalesPrice1", className: "text-right ws-nowrap" },
                { title: "Alış Fiyat", data: "PurchasePrice", className: "text-right ws-nowrap" }
            ],
            language: {
                url: "/Scripts/Admin/vendor/datatables/js/Turkish.json"
            },

            searching: false,
            scrollY: ((searchData.length > 5) ? 160 : false),
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            destroy: true,
            initComplete: function () {
                $scope.fireScrollOnHover();
            }
        });

        $(mProductSearchTableName + ' tbody').on('click', 'button', function () {
            var data = mProductItemSearchTable.row($(this).parents('tr')).data();
            $scope.selectItemProduct(data, $scope.veriableName);
            $(mProductItemSearch).modal('hide');
        });
    };

    $scope.fireScrollOnHover = function () {
        $(".scroll-on-hover").mouseover(function () {
            $(this).removeClass("ellipsis");
            var maxscroll = $(this).width();
            var speed = maxscroll * 10;
            $(this).animate({
                scrollLeft: maxscroll
            }, speed, "linear");
        });

        $(".scroll-on-hover").mouseout(function () {
            $(this).stop();
            $(this).addClass("ellipsis");
            $(this).animate({
                scrollLeft: 0
            }, 'slow');
        });
    };

    $scope.keypressItemEventSearch = function (e, searchItemCriteria, veriableName) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            $scope.generalItemSearch(searchItemCriteria);
            e.preventDefault();
        }
    };

    $scope.ProductItemSelectOpen = function (searchItemCriteria, veriableName) {
        $(mProductItemSearch).appendTo("body").modal('show');
        $(mProductItemSearch).on('shown.bs.modal', function () {
            $("#iProductGeneralText").focus();
        });
        $scope.veriableName = veriableName;
    };

    $scope.clearItemProdcut = function () {
        $scope.searchItemCriteria = {};
    };

});

