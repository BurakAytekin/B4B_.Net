adminApp.controller('ProductSelectController', function ($scope, $http) {

    // #region Veriables
    var mProductSearchTable;
    $scope.modalIsOpen = false;
    $scope.selectedProduct = {};
    $scope.searchCriteria = {};
    $scope.warehousesList = {};
    $scope.searchProductResultList = {};
    var mProductSearch = "#mProductSearch";

    // #endregion

    $scope.generalSearch = function (searchCriteria) //
    {
        fireCustomLoading(true);
        $scope.selectedProduct = {};
        $scope.warehousesList = {};
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductSearch",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { searchCriteria: searchCriteria }
        }).then(function (response) {
            $scope.searchProductResultList = response.data;
            if ($scope.searchProductResultList.length === 1 && !$scope.modalIsOpen) {
                $scope.selectProduct($scope.searchProductResultList[0]);
                $scope.getWarehousesList($scope.searchProductResultList[0].Id);

            } else {
                if (!$scope.modalIsOpen) {
                    $scope.ProductSelectOpen(searchCriteria);
                    setTimeout(function () {
                        $scope.fillData(response.data);
                    }, 1000);
                } else {
                    $scope.fillData(response.data);
                }
            }
            fireCustomLoading(false);

        });
    };
    $scope.selectProduct = function (product) {
        $scope.selectedProduct = product;
        $scope.searchCriteria.Name = $scope.selectedProduct.Name;
        $scope.searchCriteria.Code = $scope.selectedProduct.Code;
        $scope.getWarehousesList(product.Id);
        $(mProductSearch).modal('hide');


    }
    $scope.getWarehousesList = function (id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductQuantity",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }
        }).then(function (response) {

            $scope.warehousesList = response.data;
            fireCustomLoading(false);
        });

    };
    $scope.updateUiWarehouse = function (warehouse) {

        var content = ''
        content += '<form action="" class="formName">';
        content += '<form action="" class="formName">';
        content += '<div class="form-group">';
        content += '<span>Yolda Miktarı</span>';
        content += '<input type="text"  placeholder="0" class="warehouseQuantityOnWay form-control eryaz-numeric-input" required value="' + warehouse.QuantityOnWay + '" />';
        content += '<span>Miktar</span>';
        content += '<input type="text"  placeholder="0" class="warehouseQuantity form-control eryaz-numeric-input" required value="' + warehouse.Quantity + '" />';
        content += '<span>Bakiye Posizyou</span>';
        content += '<select class="warehouseQuantityType form-control">';
        content += '<option val="0" ' + ((warehouse.QuantityType == 0) ? "selected=selected" : "") + ' >Bakiye Durumuna Göre</option>';
        content += '<option val="1"  ' + ((warehouse.QuantityType == 1) ? "selected=selected" : "") + '>Sürekli Mevcut Göster</option>';
        content += '<option val="2"  ' + ((warehouse.QuantityType == 2) ? "selected=selected" : "") + '>Sürekli Mevcut Değil Göster</option>';
        //content += '<option val="3"  ' + ((warehouse.QuantityType == 3) ? "selected=selected" : "") + '>Yolda</option>';
        content += '<option val="4"  ' + ((warehouse.QuantityType == 4) ? "selected=selected" : "") + '>Sipariş Üzerine</option>';
        content += '</select>';
        content += '</div>';
        content += '</form>';

        $.confirm({
            title: warehouse.Warehouse.Name + " Deposu",
            content: content,
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter', 'shift'],
                    action: function () {
                        var warehouseQuantityOnWay = this.$content.find('.warehouseQuantityOnWay').val();
                        var warehouseQuantity = this.$content.find('.warehouseQuantity').val();
                        var warehouseQuantityType = this.$content.find('.warehouseQuantityType')[0].selectedIndex;
                        if (!warehouseQuantityOnWay || !warehouseQuantity) {
                            return false;
                        }

                        $scope.updateWarehouseQuantity(warehouse.Id, warehouseQuantityOnWay, warehouseQuantity, warehouseQuantityType);

                    }

                },
                Vazgeç: function () {
                    //close
                }
            }
        });
    };
    $scope.updateWarehouseQuantity = function (id, warehouseQuantityOnWay, warehouseQuantity, warehouseQuantityType) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/UpdateWarehouseQuantity",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, warehouseQuantityOnWay: warehouseQuantityOnWay, warehouseQuantity: warehouseQuantity, warehouseQuantityType:warehouseQuantityType }
        }).then(function (response) {
            $scope.getWarehousesList($scope.selectedProduct.Id);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
        });

    };

    $scope.fillData = function (searchData) {
        var mProductSearchTableName = '#mProductSearchTable'; //Id Or Class

        mProductSearchTable = $(mProductSearchTableName).DataTable({
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
                //{ title: "Resim", data: "HavePicture", className: "text-center ws-nowrap" },
                 { title: "Satış Fiyat", data: "SalesPrice1", className: "text-right ws-nowrap" },
                { title: "Alış Fiyat", data: "PurchasePrice", className: "text-right ws-nowrap" },

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
            var data = mProductSearchTable.row($(this).parents('tr')).data();
            $scope.selectProduct(data);
            $scope.getWarehousesList(data.Id);
            $(mProductSearch).modal('hide');
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
    }
    $scope.keypressEventSearch = function (e, searchCriteria,pressedKey) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            if (pressedKey == 1) {

                $scope.searchCriteria.Name = "";
            }
            if (pressedKey == 2)
                $scope.searchCriteria.Code = "";

            $scope.generalSearch(searchCriteria);
            e.preventDefault();
        }
    };
    $scope.ProductSelectOpen = function (searchCriteria) {
        $scope.modalIsOpen = true;
        //if (!$('#mProductSearchTable').hasClass('hide'))
        //    $('#mProductSearchTable').addClass('hide');
        $(mProductSearch).appendTo("body").modal('show');
        $(mProductSearch).on('shown.bs.modal', function () {
            $("#iProductGeneralText").focus();
        });


        $(mProductSearch).on('hidden.bs.modal', function () {
            $scope.modalIsOpen = false;
        });

    }
    $scope.clear = function () {
        $scope.searchCriteria = {};
        $scope.searchProductResultList = {};
        $scope.selectedProduct = {};

    }
    $(document).ready(function () {

    });
});
