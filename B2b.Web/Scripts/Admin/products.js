adminApp.controller('IndexController', ['$scope', '$http', 'NgTableParams', function ($scope, $http, NgTableParams) {
    //#region Veriables
    //var mProductSearchTable;
    //$scope.modalIsOpen = false;
    $scope.selectedProduct = [];
    $scope.productAlternativeList = {};
    $scope.oemOrRivalVehicleList = [];
    $scope.priceType = [
        {
            Type: 1,
            TypeStr: "SATIS"
        },
        {
            Type: 2,
            TypeStr: "ALIS"
        }
    ];

    //var mProductSearch = "#mProductSearch";
    // $scope.searchType = { "Code": 1, "Name": 2, "None": 3 } Örnek olması için bırakıldı(Enum tanımlaması projede searchType.Code  gibi değer ulaşılabilir)
    //#endregion

    $scope.getAlternativeProducts = function () {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.GroupId === undefined)
            return;
        var groupId = $scope.selectedProduct.GroupId;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetAlternativeProducts",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { groupId: groupId }
        }).then(function (response) {
            $scope.productAlternativeList = response.data;

            $scope.tableAlternativeParams = new NgTableParams({
                count: $scope.productAlternativeList.length === 0 ? 1 : $scope.productAlternativeList.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy($scope.productAlternativeList)
            });

            $scope.tableAlternativeOrjinalData = angular.copy($scope.productAlternativeList);
            $scope.tableAlternativedelete = $scope.tableAlternativedelete;
            $scope.tableAlternativeadd = $scope.tableAlternativeadd;
            fireCustomLoading(false);
        });
    }

    // #region Alternative Prodcut Table Codes
    $scope.tableAlternativeadd = function () {
        $scope.ProductItemSelectOpen(String.empty, "alternativeItemProduct");
    };

    $scope.$watch('alternativeItemProduct', function (newValue, oldValue) {
        var product = $scope.selectedProduct;
        if (newValue !== oldValue && product !== newValue) {
            var mainProductGroupId = product.GroupId;
            var newProductGroupId = newValue.GroupId;
            $scope.updateAlternativeProducts(newProductGroupId, mainProductGroupId);
        }

    }, true);

    $scope.updateAlternativeProducts = function (newProductGroupId, mainProductGroupId) {
        fireCustomLoading(true);        
        $http({
            method: "POST",
            url: "/Admin/Products/UpdateAlternativeProducts",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { newProductGroupId: newProductGroupId, mainProductGroupId: mainProductGroupId }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.selectedProduct.GroupId = mainProductGroupId;
            $scope.getAlternativeProducts();
            fireCustomLoading(false);
        });
    }

    $scope.tableAlternativeaskForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.tableAlternativedelete(row); }

                    },
                    Cancel: {
                        text: "Hayır",
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
        });
    };

    $scope.tableAlternativedelete = function (row) {
        var productId = row.Id;
        if (productId === undefined || productId === 0)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/DeleteAlternativeProducts",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { productId: productId }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getAlternativeProducts();
        });
    };
    // #endregion

    $scope.getLinkedProducts = function () {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.Id === undefined)
            return;
        var productId = $scope.selectedProduct.Id;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetLinkedProducts",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { productId: productId }
        }).then(function (response) {

            $scope.tableLinkedProductParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            $scope.tableLinkedProdcutOrjinalData = angular.copy($scope.productAlternativeList);
            $scope.tableLinkedProdcutdelete = $scope.tableLinkedProdcutdelete;
            $scope.tableLinkedProdcutadd = $scope.tableLinkedProdcutadd;
            fireCustomLoading(false);
        });
    }

    // #region Linked Product Table Codes

    $scope.tableLinkedProdcutadd = function () {
        $scope.ProductItemSelectOpen(String.empty, "linkedItemProduct");
    };

    $scope.$watch('linkedItemProduct', function (newValue, oldValue) {
        var product = $scope.selectedProduct;
        if (newValue !== oldValue) {
            var mainProductId = product.Id;
            var linkedProductId = newValue.Id;
            $scope.insertLinkedProduct(linkedProductId, mainProductId);
        }

    }, true);

    $scope.insertLinkedProduct = function (linkedProductId, mainProductId) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/InertLinkedProduct",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { linkedProductId: linkedProductId, mainProductId: mainProductId }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getLinkedProducts();
        });
    };

    $scope.tableLinkedaskForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.tableLinkedDelete(row); }
                    },
                    Cancel: {
                        text: "Hayır",
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
        });
    };

    $scope.tableLinkedDelete = function (row) {
        var pId = row.LinkedId;
        if (pId === undefined || pId === 0)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Products/DeleteLinkedProducts",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { pId: pId }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getLinkedProducts();
        });
    };

    // #endregion

    $scope.getOemOrRivalVehicleList = function () {
        if ($scope.oemOrRivalVehicleList.length <= 0) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Products/GetVehicleList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}
            }).then(function (response) {
                $scope.oemOrRivalVehicleList = response.data;
                $scope.enableAutoComplate(response.data);
                fireCustomLoading(false);
            });
        } else {
            $scope.enableAutoComplate($scope.oemOrRivalVehicleList);
        }
    };

    $scope.enableAutoComplate = function (data) {
        var lData = [];
        $.each(data, function (a, b) {
            lData.push(b.Brand);
        });
        setTimeout(function () {

            var states = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace,
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: lData
            });

            $('.typeaheadOemOrRival').typeahead({
                hint: true,
                highlight: true,
                minLength: 1
            },
            {
                name: 'states',
                source: states
            });
        }, 250);
    };

    $scope.getOemNoList = function () {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.GroupId === undefined)
            return;
        var groupId = $scope.selectedProduct.GroupId;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetOemNoList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { groupId: groupId }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.tableOemParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            $scope.originalDataOem = angular.copy(response.data);
            $scope.cancelOem = $scope.cancelOem;
            $scope.deleteOem = $scope.deleteOem;
            $scope.saveOem = $scope.saveOem;
            $scope.addOem = $scope.addOem;
            $scope.cancelChangesOem = $scope.cancelChangesOem;
            $scope.hasChangesOem = $scope.hasChangesOem;
        });
    }

    // #region OemTable Codes

    $scope.editingChangeOem = function (row) {
        row.isEditingOem = true;
        $scope.getOemOrRivalVehicleList();
    };

    $scope.hasChangesOem = function () {
        return $scope.tableOemParams.$dirty || $scope.deleteCountOem > 0;
    };

    $scope.cancelChangesOem = function () {
        $scope.resetTableStatusOem();
        var currentPage = $scope.tableOemParams.page();
        $scope.getOemNoList();
        if (!$scope.isAddingOem) {
            $scope.tableOemParams.page(currentPage);
        }
    };

    $scope.resetTableStatusOem = function () {
        $scope.isEditingOem = false;
        $scope.isAddingOem = false;

        for (var i = 0; i < $scope.tableOemParams._settings.dataset.length; i++) {
            $scope.tableOemParams._settings.dataset[i].isEditingOem = false;
        }
    };

    $scope.addOem = function () {
        $scope.isEditingOem = true;
        $scope.isAddingOem = true;
        $scope.tableOemParams.settings().dataset.unshift({
            Id: 0,
            Brand: "",
            OemNo: ""
        });

        $scope.tableOemParams.sorting({});
        $scope.tableOemParams.page(1);
        $scope.tableOemParams.reload();

        for (var i = 0; i < $scope.tableOemParams._settings.dataset.length; i++) {
            $scope.tableOemParams._settings.dataset[i].isEditingOem = true;
        }
        $scope.getOemOrRivalVehicleList();
    };

    $scope.cancelOem = function (row, rowForm) {
        var originalRowOem = $scope.resetRowOem(row, rowForm);
        angular.extend(row, originalRowOem);
    };

    $scope.askForDeleteOem = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.deleteOem(row); }
                    },
                    Cancel: {
                        text: "Hayır",
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });
                        }
                    }

                }
        });
    };

    $scope.deleteOem = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/DeleteOem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { pId: row.Id }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            var index = $scope.tableOemParams.data.indexOf(row);
            $scope.deleteCountOem++;
            $scope.tableOemParams.data.splice(index, 1);
            $scope.tableOemParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(this.tableParams.data)
            });
        });
    };

    $scope.resetRowOem = function (row, rowForm) {

        row.isEditingOem = false;
        for (let i in $scope.originalDataOem) {
            if ($scope.originalDataOem.hasOwnProperty(i)) {
                if ($scope.originalDataOem[i].Id === row.Id) {
                    return $scope.originalDataOem[i];
                }
            }
        }
    };

    $scope.saveOem = function (row, rowForm) {
        var product = $scope.selectedProduct;
        var type = 0;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/SaveOem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { product: product, row: row, type: type }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            var originalRowOem = $scope.resetRowOem(row, rowForm);
            angular.extend(originalRowOem, row);
        });
    };

    $scope.keypressOemEvent = function (e, row) {

        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 46)
            e.returnValue = false;
        if (key === 13) {
            $scope.saveOem(row);
        }
    };

    // #endregion

    $scope.getRivalCodeList = function () {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.GroupId === undefined)
            return;
        var groupId = $scope.selectedProduct.GroupId;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetRivalList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { groupId: groupId }
        }).then(function (response) {

            $scope.tableRivalParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            $scope.originalDataRival = angular.copy(response.data);
            $scope.cancelRival = $scope.cancelRival;
            $scope.deleteRival = $scope.deleteRival;
            $scope.saveRival = $scope.saveRival;
            $scope.addRival = $scope.addRival;
            $scope.cancelChangesRival = $scope.cancelChangesRival;
            $scope.hasChangesRival = $scope.hasChangesRival;
            fireCustomLoading(false);
        });
    }

    // #region Rival Table Code
    $scope.editingChangeRival = function (row) {
        row.isEditingRival = true;
        $scope.getOemOrRivalVehicleList();
    }

    $scope.hasChangesRival = function () {
        return $scope.tableRivalParams.$dirty || $scope.deleteCountRival > 0;
    };

    $scope.cancelChangesRival = function () {
        $scope.resetTableStatusRival();
        var currentPage = $scope.tableRivalParams.page();

        $scope.getRivalCodeList();
        if (!$scope.isAddingRival) {
            $scope.tableRivalParams.page(currentPage);
        }
    };

    $scope.resetTableStatusRival = function () {
        $scope.isEditingRival = false;
        $scope.isAddingRival = false;
        for (var i = 0; i < $scope.tableRivalParams._settings.dataset.length; i++) {
            $scope.tableRivalParams._settings.dataset[i].isEditingRival = false;
        }
    };

    $scope.addRival = function () {
        $scope.isEditingRival = true;
        $scope.isAddingRival = true;
        $scope.tableRivalParams.settings().dataset.unshift({
            Id: 0,
            Brand: "",
            OemNo: ""
        });
        $scope.tableRivalParams.sorting({});
        $scope.tableRivalParams.page(1);
        $scope.tableRivalParams.reload();
        for (var i = 0; i < $scope.tableRivalParams._settings.dataset.length; i++) {
            $scope.tableRivalParams._settings.dataset[i].isEditingRival = true;
        }
        $scope.getOemOrRivalVehicleList();
    };

    $scope.cancelRival = function (row, rowForm) {
        var originalRowRival = $scope.resetRowRival(row, rowForm);
        angular.extend(row, originalRowRival);
    };

    $scope.askForDeleteRival = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.deleteRival(row); }
                    },
                    Cancel: {
                        text: "Hayır",
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });
                        }
                    }

                }
        });
    };

    $scope.deleteRival = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/DeleteOem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { pId: row.Id }

        }).then(function (response) {
            
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            var index = $scope.tableRivalParams.data.indexOf(row);
            $scope.deleteCountRival++;
            $scope.tableRivalParams.data.splice(index, 1);
            $scope.tableRivalParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy($scope.tableRivalParams.data)
            });
            fireCustomLoading(false);
        });
    };

    $scope.resetRowRival = function (row, rowForm) {

        row.isEditingRival = false;
        for (let i in $scope.originalDataRival) {
            if ($scope.originalDataRival.hasOwnProperty(i)) {
                if ($scope.originalDataRival[i].Id === row.Id) {
                    return $scope.originalDataRival[i];
                }
            }
        }
    };

    $scope.saveRival = function (row, rowForm) {
        var product = $scope.selectedProduct;
        var type = 1;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/SaveOem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { product: product, row: row, type: type }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            var originalRowRival = $scope.resetRowRival(row, rowForm);
            angular.extend(originalRowRival, row);
        });
    };

    $scope.keypressRivalEvent = function (e, row) {

        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 46)
            e.returnValue = false;
        if (key === 13) {
            $scope.saveRival(row);
        }
    };

    // #endregion

    $scope.getVehicleBrandModel = function () {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.GroupId === undefined)
            return;
        var groupId = $scope.selectedProduct.GroupId;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetVehicleBrandModelList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { groupId: groupId }
        }).then(function (response) {
            $scope.tableVehicleParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            $scope.tableVehicleeOrjinalData = angular.copy(response.data);
            $scope.tableVehicledelete = $scope.tableVehicledelete;
            $scope.tableVehicleadd = $scope.tableVehicleadd;
            fireCustomLoading(false);
        });
    }

    // #region VehicleTable Codes
    $scope.tableVehicleadd = function () {
        $scope.vehicleSelecetOpen();
    };

    $scope.tableVehicleaskForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.tableVehicledelete(row); }
                    },
                    Cancel: {
                        text: "Hayır",
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });
                        }
                    }

                }
        });
    };

    $scope.tableVehicledelete = function (row) {
        var vehicleId = row.VehicleId;
        if (vehicleId === undefined || vehicleId === 0)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/DeleteVehicleProducts",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { vehicleId: vehicleId }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            $scope.getVehicleBrandModel();
        });
    };
    // #endregion

    $scope.fireTabTableLoad = function (tName, tData, tColumns) {
        $(tName).attr("width", "100%");

        var tTable = $(tName).DataTable({
            data: tData,
            columns: tColumns,
            language: {
                url: "/Scripts/Admin/vendor/datatables/js/Turkish.json"
            },
            autoWidth: false,
            searching: false,
            scrollY: 300,
            scrollX: true,
            destroy: true,
            paging: false,
            initComplete: function () {
                fireScrollOnHover();
            }
        });

        $(tName + ' tbody').on('click', 'tr', function () {
            if ($(this).hasClass('row_selected')) {
                $(this).removeClass('row_selected');
            } else {
                tTable.$('tr.row_selected').removeClass('row_selected');
                $(this).addClass('row_selected');
            }
        });
    };

    $scope.getNextProduct = function (type) {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.Code === undefined)
            return;
        var code = $scope.selectedProduct.Code;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetNextProduct",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { code: code, type: type }
        }).then(function (response) {
            if (response.data.length <= 0)
                return;

            $scope.selectedProduct = response.data[0];
            $scope.searchCriteria.Name = $scope.selectedProduct.Name;
            $scope.searchCriteria.Code = $scope.selectedProduct.Code;
            $scope.getWarehousesList(response.data[0].Id);
            $('a[href="#tProductGeneralInformation"]').click();
            fireCustomLoading(false);
        });
    };

    $scope.changeSelectedProduct = function (prodcutId) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductById",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { prodcutId: prodcutId }
        }).then(function (response) {
            $scope.selectedProduct = response.data[0];
            $scope.getAlternativeProducts();
            $scope.getLinkedProducts();
            $scope.getWarehousesList($scope.selectedProduct.Id);
            $scope.searchCriteria.Name = $scope.selectedProduct.Name;
            $scope.searchCriteria.Code = $scope.selectedProduct.Code;
            iziToast.show({
                message: "Ürün Seçilmiştir",
                position: 'topCenter',
                color: "success",
                icon: "fa fa-check"
            });
            fireCustomLoading(false);
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


    $scope.getProductPrices = function () {
        if ($scope.selectedProduct === {} || $scope.selectedProduct.GroupId === undefined)
            return;
        var productId = $scope.selectedProduct.Id;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductPrices",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { productId: productId }
        }).then(function (response) {
            $scope.tableParamsPrice = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            $scope.tablePriceOrjinalData = angular.copy(response.data);
            $scope.tablePricedelete = $scope.tablePricedelete;
            $scope.tablePriceadd = $scope.tablePriceadd;
            fireCustomLoading(false);
        });

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetCurrencyTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.currencyList = response.data;
        });
    };

    // #region ProductPrice Table Codes

    $scope.editingChangePrice = function (row) {
        row.isEditingPrice = true;
        $scope.getProductPrices();
    };

    $scope.hasChangesPrice = function () {
        return $scope.tableParamsPrice.$dirty || $scope.deleteCountPrice > 0;
    };

    $scope.cancelChangesPrice = function () {
        $scope.resetTableStatusPrice();
        var currentPage = $scope.tableParamsPrice.page();

        $scope.getProductPrices();
        if (!$scope.isAddingPrice) {
            $scope.tableParamsPrice.page(currentPage);
        }
    };

    $scope.resetTableStatusPrice = function () {
        $scope.isEditingPrice = false;
        $scope.isAddingPrice = false;

        for (var i = 0; i < $scope.tableParamsPrice._settings.dataset.length; i++) {
            $scope.tableParamsPrice._settings.dataset[i].isEditingPrice = false;
        }
    };

    $scope.addPrice = function () {
        $scope.isEditingPrice = true;
        $scope.isAddingPrice = true;
        $scope.tableParamsPrice.settings().dataset.unshift({
            Id: 0,
            PriceNumber: -1,
            Type: 1,
            TypeStr: "SATIS",
            Price: 0,
            Currency: "TL"
        });


        $scope.tableParamsPrice.sorting({});
        $scope.tableParamsPrice.page(1);
        $scope.tableParamsPrice.reload();

        for (var i = 0; i < $scope.tableParamsPrice._settings.dataset.length; i++) {
            $scope.tableParamsPrice._settings.dataset[i].isEditingPrice = true;
        }
        // $scope.getProductPrices();
    };

    $scope.cancelPrice = function (row, rowForm) {
        var originalRowRival = $scope.resetRowPrice(row, rowForm);
        angular.extend(row, originalRowRival);
    };

    $scope.askForDeletePrice = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.deletePrice(row); }
                    },
                    Cancel: {
                        text: "Hayır",
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });
                        }
                    }

                }
        });
    };

    $scope.deletePrice = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/DeletePrice",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { pId: row.Id }

        }).then(function (response) {
            
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            var index = $scope.tableParamsPrice.data.indexOf(row);
            $scope.deleteCountPrice++;
            $scope.tableParamsPrice.data.splice(index, 1);
            $scope.tableParamsPrice = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(this.tableParamsPrice.data)
            });
            fireCustomLoading(false);

        });
    };

    $scope.resetRowPrice = function (row, rowForm) {

        row.isEditingPrice = false;
        for (let i in $scope.originalDataPrice) {
            if ($scope.originalDataPrice.hasOwnProperty(i)) {
                if ($scope.originalDataPrice[i].Id === row.Id) {
                    return $scope.originalDataPrice[i];
                }
            }
        }
    };

    $scope.savePrice = function (row, rowForm) {
        var product = $scope.selectedProduct;
        var price = row;

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/InsertPrice",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { product: product, price: price }

        }).then(function (response) {
            fireCustomLoading(false); 
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            var originalRowRival = $scope.resetRowPrice(row, rowForm);
            angular.extend(originalRowRival, row);

        });
    };

    $scope.keypressPriceEvent = function (e, row, rowForm) {

        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 46)
            e.returnValue = false;
        if (key === 13) {
            $scope.savePrice(row, rowForm);
        }
    };

    // #endregion

    $scope.saveProduct = function () {
        if ($scope.selectedProduct === {} || $scope.searchCriteria.Code === undefined || $scope.searchCriteria.Code === "")
            return;
        $scope.selectedProduct.Name = $scope.searchCriteria.Name;
        $scope.selectedProduct.Code = $scope.searchCriteria.Code;
        var product = $scope.selectedProduct;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/SaveProduct",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { product: product }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
        });
    };

    function fileUploads() {

        var manualUploader = new qq.FineUploader({
            element: document.getElementById('fine-uploader-manual-trigger'),
            template: 'qq-template-manual-trigger',
            success: OnComplete,
            error: OnFail,
            request: {
                endpoint: 'FilesUpload/Index2?productId=' + $scope.selectedProduct.Id + "&productCode=" + $scope.selectedProduct.Code + "&watermark=" + $scope.txtwatermark
            },

            callbacks: {
                onComplete: function (id, name, response) {

                    if (response.success) {
                        $scope.getPictureList();
                        iziToast.show({
                            message: "Yüklemeleriniz Gerçekleşti.",
                            position: 'topCenter',
                            color: "success",
                            icon: "fa fa-check"
                        });
                    }
                }
            },

            thumbnails: {
                placeholders: {
                    waitingPath: '/Content/fine-uploader/images/waiting-generic.png',
                    notAvailablePath: '/Content/fine-uploader/images/not_available-generic.png'
                }
            },
            autoUpload: false,
            debug: true
        });

        qq(document.getElementById("trigger-upload")).attach("click", function () {
            manualUploader.uploadStoredFiles();
        });
    };

    $scope.reloadImageArea = function () {
        fileUploads();
    };

    $scope.deletePicture = function (type, item) {


        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "Silmek istediğinize emin misiniz?",
                buttons:
                {
                    EVET: {
                        btnClass: 'btn-blue',
                        action: function () { $scope.deletePicture(true, item); }

                    },
                    HAYIR: {
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
            });
        }
        else {
            fireCustomLoading(true);
            item.Deleted = true;
            $http({
                method: "POST",
                url: "/Admin/Products/UpdatePicture",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { picture: item }
            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $scope.getPictureList();
            });
        }
    };

    $scope.$on('ngRepeatFileListFinished', function (ngRepeatFileListFinishedEvent) {
        $('.mix-grid').mixItUp();
        $('.mix-grid').mixItUp('destroy')
        $('.mix-grid').mixItUp();

        //$('.mix-controls .select-all input').change(function () {
        //    if ($(this).is(":checked")) {
        //        $('.gallery').find('.mix').addClass('selected');
        //        enableGalleryTools(true);
        //    }
        //    else {
        //        $('.gallery').find('.mix').removeClass('selected');
        //        enableGalleryTools(false);
        //    }
        //});

        //$('.mix .img-select').click(function () {
        //    var mix = $(this).parents('.mix');
        //    if (mix.hasClass('selected')) {
        //        mix.removeClass('selected');
        //        enableGalleryTools(false);
        //    }
        //    else {
        //        mix.addClass('selected');
        //        enableGalleryTools(true);
        //    }
        //});

        //var enableGalleryTools = function (enable) {

        //    if (enable) {

        //        $('.mix-controls li.mix-control').removeClass('disabled');

        //    }
        //    else {
        //        var selected = false;

        //        $('.gallery .mix').each(function () {
        //            if ($(this).hasClass('selected')) {
        //                selected = true;
        //            }
        //        });

        //        if (!selected) {
        //            $('.mix-controls li.mix-control').addClass('disabled');
        //        }
        //    }
        //}
    });





    $scope.setIsDefault = function (item) {
        fireCustomLoading(true);
        item.IsDefault = true;
        $http({
            method: "POST",
            url: "/Admin/Products/UpdatePicture",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { picture: item }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getPictureList();
        });
    }


    $scope.getPictureList = function () {
        var productId = $scope.selectedProduct.Id;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetPictureList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { productId: productId }

        }).then(function (response) {
            $scope.pictureList = response.data;
            fireCustomLoading(false);


        });
    };
    

    $scope.fireFileGallery = function () {
        $('.mix-grid').mixItUp();

        $('.mix-controls .select-all input').change(function () {
            if ($(this).is(":checked")) {
                $('.gallery').find('.mix').addClass('selected');
                enableGalleryTools(true);
            } else {
                $('.gallery').find('.mix').removeClass('selected');
                enableGalleryTools(false);
            }
        });

        $('.mix .img-select').click(function () {
            var mix = $(this).parents('.mix');
            if (mix.hasClass('selected')) {
                mix.removeClass('selected');
                enableGalleryTools(false);
            } else {
                mix.addClass('selected');
                enableGalleryTools(true);
            }
        });

        var enableGalleryTools = function (enable) {

            if (enable) {

                $('.mix-controls li.mix-control').removeClass('disabled');

            } else {

                var selected = false;

                $('.gallery .mix').each(function () {
                    if ($(this).hasClass('selected')) {
                        selected = true;
                    }
                });

                if (!selected) {
                    $('.mix-controls li.mix-control').addClass('disabled');
                }
            }
        }
    };

    $scope.getPictureActive = function () {
        fileUploads();
        $scope.getPictureList();
    };

    $(document).ready(function () { fileUploads(); });

    //#region FileUploadFuctions


    function OnComplete(result) {
        alert('Success');
    }

    function OnFail(result) {
        alert('Request failed');
    }
    //#endregion

}]).directive('customProductSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Products/ProductsSelect",
        controller: 'ProductSelectController'
    };
}).directive('customItemProductSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Products/ProductsItemSelect",
        controller: 'ProductItemSelectController'
    };
}).directive('customProductGroupsSelect', function () {
    return {
        restrict: 'E',
        templateUrl: "/Admin/Products/ProductGroupsSelect",
        controller: 'ProductGroupsSelectController'
    };
}).directive('customProductManufacturerSelect', function () {
    return {
        restrict: 'E',
        templateUrl: "/Admin/Products/ManufacturerSelect",
        controller: 'ProductManufacturerSelectController'
    };
}).directive('customProductRuleSelect', function () {
    return {
        restrict: 'E',

        templateUrl: "/Admin/Products/RuleSelect",
        controller: 'ProductRuleSelectController'


    };
}).directive('customVehicleSelect', function () {
    return {
        restrict: 'E',

        templateUrl: "/Admin/Products/VehicleSelect",
        controller: 'VehicleSelectController'


    };
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
    }
});;