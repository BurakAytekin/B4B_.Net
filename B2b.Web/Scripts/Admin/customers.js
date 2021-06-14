adminApp.controller('IndexController', ['$scope', '$http', 'NgTableParams', function ($scope, $http, NgTableParams) {
    $scope.checkedAcitveManufacturerList = [];
    $scope.checkedPassiveManufacturerList = [];
    $scope.selectedProduct = {};
    $scope.lockUpdate = true;
    $scope.userList = null;
    $scope.salesPriceIsActive = false;
    $scope.salesPriceIsActiveP = false;
    $scope.salesPriceIsActivePg = false;

    $scope.clearRuleAdditionalData = function () {
        $scope.RuleAdditionalManufacturer = {
            "Disc1": 0,
            "Disc2": 0,
            "Disc3": 0,
            "Disc4": 0,
            "MainDiscountPassive": false,
            "DueDay": 0,
            "Rate": 0,
            "SalesPrice":-1
        };
        $scope.RuleAdditionalProduct = {
            "Disc1": 0,
            "Disc2": 0,
            "Disc3": 0,
            "Disc4": 0,
            "MainDiscountPassive": false,
            "DueDay": 0,
            "Rate": 0,
            "SalesPrice":-1
        };
        $scope.RuleAdditionalProductGroup = {
            "Disc1": 0,
            "Disc2": 0,
            "Disc3": 0,
            "Disc4": 0,
            "MainDiscountPassive": false,
            "DueDay": 0,
            "Rate": 0,
            "SalesPrice":-1
        };
        $scope.selectedProduct = {};
    }


     $scope.showEmailSendScreenCustomer = function () {

        $scope.customerEmail = $scope.selectedCustomer.Mail;
        $('#modal-EmailShowCustomer').appendTo("body").modal('show');sendPasswordResetMailCustomer
    };

    $scope.sendPasswordResetMailCustomer = function () {
        if ($scope.customerEmail === '') {
            iziToast.error({
                message: 'Lütfen mail adresi giriniz',
                position: 'topCenter',
            });
            return;
        }

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/SendPasswordResetMailForCustomer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customer: $scope.selectedCustomer, email: $scope.customerEmail }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
            $('#modal-EmailShowCustomer').appendTo("body").modal('hide');

        });
    };

    $scope.userCountList = [
    { Count: 0 }, { Count: 1 }, { Count: 2 }, { Count: 3 }, { Count: 4 }, { Count: 5 }, { Count: 6 }, { Count: 7 }, { Count: 8 }, { Count: 9 }, { Count: 10 }, { Count: 11 }, { Count: 12 }, { Count: 13 }, { Count: 14 }, { Count: 15 }, { Count: 16 }, { Count: 17 }, { Count: 18 }, { Count: 19 }, { Count: 20 }];

    $scope.addActiveManufacturerList = function (manufacturer, checked) {
        if (checked) {
            $scope.checkedAcitveManufacturerList.push(manufacturer);

        } else {
            $scope.checkedAcitveManufacturerList.splice($scope.checkedAcitveManufacturerList.indexOf(manufacturer), 1);

        }
    };



    // #region Additional Rule codes
    $scope.addRuleAdditionalOpenItem = function () {
        $scope.ProductItemSelectOpen(String.empty, "ruleAdditionalItemProduct");
    };

    $scope.saveAdditionalRuleManufacturer = function () {

        if ($scope.selectedProduct.Manufacturer === "") {
            iziToast.warning({
                position: 'topCenter',
                message: 'Lütfen İlgili alanları doldrunuz'
            });
            return;
        }

        //string manufacturer, int productId, string productGroup1, string productGroup2,
        //string productGroup3, double disc1, double disc2, double disc3, bool isMainDiscountPassive
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/AddRuleAdditional",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id, manufacturer: $scope.selectedProduct.Manufacturer, productId: -1, productGroup1: "", productGroup2: "", productGroup3: "", disc1: $scope.RuleAdditionalManufacturer.Disc1, disc2: $scope.RuleAdditionalManufacturer.Disc2, disc3: $scope.RuleAdditionalManufacturer.Disc3, disc4: $scope.RuleAdditionalManufacturer.Disc4, isMainDiscountPassive: $scope.RuleAdditionalManufacturer.MainDiscountPassive, dueDay: $scope.RuleAdditionalManufacturer.DueDay, rate: $scope.RuleAdditionalManufacturer.Rate, salesPrice: $scope.RuleAdditionalManufacturer.SalesPrice }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.clearRuleAdditionalData();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

        });
    };
    $scope.saveAdditionalRuleProduct = function () {
        if ($scope.ruleAdditionalItemProduct.Code === "") {
            iziToast.warning({
                position: 'topCenter',
                message: 'Lütfen İlgili alanları doldrunuz'
            });
            return;
        }

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/AddRuleAdditional",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id, manufacturer: "", productId: $scope.ruleAdditionalItemProduct.Id, productGroup1: "", productGroup2: "", productGroup3: "", disc1: $scope.RuleAdditionalProduct.Disc1, disc2: $scope.RuleAdditionalProduct.Disc2, disc3: $scope.RuleAdditionalProduct.Disc3, disc4: $scope.RuleAdditionalProduct.Disc4, isMainDiscountPassive: $scope.RuleAdditionalProduct.MainDiscountPassive, dueDay: $scope.RuleAdditionalProduct.DueDay, rate: $scope.RuleAdditionalProduct.Rate, salesPrice: $scope.RuleAdditionalProduct.SalesPrice }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.clearRuleAdditionalData();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

        });
    }
    $scope.saveAdditionalRuleProductGroup = function () {
        if ($scope.selectedProduct.ProductGroup1 === "") {
            iziToast.warning({
                position: 'topCenter',
                message: 'Lütfen İlgili alanları doldrunuz'
            });
            return;
        }

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/AddRuleAdditional",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id, manufacturer: "", productId: -1, productGroup1: $scope.selectedProduct.ProductGroup1, productGroup2: $scope.selectedProduct.ProductGroup2, productGroup3: $scope.selectedProduct.ProductGroup3, disc1: $scope.RuleAdditionalProductGroup.Disc1, disc2: $scope.RuleAdditionalProductGroup.Disc2, disc3: $scope.RuleAdditionalProductGroup.Disc3, disc4: $scope.RuleAdditionalProductGroup.Disc4, isMainDiscountPassive: $scope.RuleAdditionalProductGroup.MainDiscountPassive, dueDay: $scope.RuleAdditionalProductGroup.DueDay, rate: $scope.RuleAdditionalProductGroup.Rate, salesPrice: $scope.RuleAdditionalProductGroup.SalesPrice }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.clearRuleAdditionalData();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

        });
    }

    $scope.getCustomerRuleAdditionalList = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerRuleAdditionalList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.rowRuleAdditionalList = response.data;
            $scope.tableRuleAdditionalParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
        });
    };
    $scope.changeRuleAdditionalValue = function (e, id, name, value) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 46)
            e.returnValue = false;
        if (key === 13) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Customers/UpdateRuleAdditional",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: id, name: name, value: value }
            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });


            });

        }
    };
    $scope.searchLogList = function () {
        //alert($scope.dateStart);
        //alert($scope.dateEnd);
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetSearchList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                dateStart: $('#iSearchLogStartDate').val(), dateEnd: $('#iSearchLogEndDate').val(), customerId: $scope.selectedCustomer.Id, userId: -1
            }
        }).then(function (response) {
            fireCustomLoading(false);
            if (response.data.length <= 0) {
                iziToast.error({
                    message: 'Aradığınız kriterlerde sonuç bulunamamıştır.',
                    position: 'topCenter'
                });
            }

            $scope.reportList = response.data;
            console.log($scope.reportList);
        });
    };
    $scope.searchLogDetail = function (id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetSearchDetailList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.reportDetailList = response.data;
            $('#modal-logdetail').appendTo("body").modal('show');

        });
    }
    function setDefaultDate() {
        var today = new Date();

        $('.detepicker').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    };
    $scope.deleteRuleAdditional = function (row) {
        
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () {
                            fireCustomLoading(true);
                            $http({
                                method: "POST",
                                url: "/Admin/Customers/DeleteRuleAdditional",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { id: row.Id }
                            }).then(function (response) {
                                fireCustomLoading(false);
                                $scope.getCustomerRuleAdditionalList();
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                            });
                        }
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

    // #endregion

    $scope.changeLockUpdate = function () {
        $scope.lockUpdate = !$scope.lockUpdate;
    };

    $scope.cancelUpdate = function () {
        $scope.selectedCustomer = angular.copy($scope.selectedCustomer_Orjinal);
    };
    $scope.newCustomer = function () {
        $scope.selectedCustomer = {};
        $scope.selectedCustomer.NumberOfUser = 0;
        $scope.selectedCustomer.NumberOfUserAndroid = 0;
        $scope.selectedCustomer.NumberOfUserIos = 0;
        $scope.selectedCustomer.CurrencyType = "TL";

        $scope.checkedAcitveManufacturerList = [];
        $scope.checkedPassiveManufacturerList = [];
        $scope.userList = null;
        $scope.searchCriteria = null;
        $scope.warehouseList = {};
        $scope.lockUpdate = false;
    };

    $scope.addCustomer = function () {
        $scope.selectedCustomer.Code = $scope.searchCriteria.Code;
        $scope.selectedCustomer.Name = $scope.searchCriteria.Name;

        var postUrl = "AddCustomer";

        if ($scope.selectedCustomer.Id !== undefined && $scope.selectedCustomer.Id !== 0 && $scope.selectedCustomer.Id !== -1)
            postUrl = "UpdateCustomer";
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/" + postUrl,
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customer: $scope.selectedCustomer }
        }).then(function (response) {
            fireCustomLoading(false);
            if (parseInt(response.data) !== -1) {
                if (postUrl === "AddCustomer")
                    $scope.selectedCustomer.Id = parseInt(response.data);

                iziToast.success({
                    position: 'topCenter',
                    title: 'Başarılı',
                    message: 'İşlem Başarı ile tamamlandı'
                });
            } else {
                iziToast.warning({
                    position: 'topCenter',
                    message: 'İşlem Sırasında Hata Oluştu'
                });
            }




        });
    };

    $scope.deleteCustomer = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/DeleteCustomer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customer: $scope.selectedCustomer }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.newCustomer();
        });

    }
    $scope.addPassiveManufacturerList = function (manufacturer, checked) {
        if (checked) {
            $scope.checkedPassiveManufacturerList.push(manufacturer);

        } else {
            $scope.checkedPassiveManufacturerList.splice($scope.checkedPassiveManufacturerList.indexOf(manufacturer), 1);
        }

    };

    $scope.getManufacturers = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetActiveManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedCustomer.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.tablePassiveManufacturerParams = new NgTableParams({
                count: response.data.PassiveManufacturer.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data.PassiveManufacturer)
            });

            $scope.tableActiveManufacturerParams = new NgTableParams({
                count: response.data.ActiveManufacturer.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data.ActiveManufacturer)
            });
        });
    }

    $scope.getCustomerSpecialInstallment = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerInstallment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.tableCustomerSpecialInstallmentParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
        });
    };
    $scope.getcustomerLoginLog = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerLoginLog",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id, userId: -1 }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.tableCustomerLoginParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
        });
    };

      $scope.getCustomerLicence = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerLicence",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.tableCustomerLicenceParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
        });
    };

    $scope.tableLicenceForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.deleteLicence(row); }

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
    $scope.deleteLicence = function (licenceItem) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/DeleteLicence",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { licenceItem: licenceItem }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getCustomerLicence();
        });
    };


    $scope.insertPassiveManufacturer = function () {
        if ($scope.checkedAcitveManufacturerList.length === 0) {
            iziToast.show({
                message: "Eklenecek Üretici Seçiniz.",
                position: 'topCenter',
                color: "error",
                icon: "fa fa-ban"
            });
            return;
        }
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/InsertPassiveManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedCustomer.Id, manufacturer: $scope.checkedAcitveManufacturerList }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.checkedAcitveManufacturerList = [];
            $scope.getManufacturers();

        });
    };

    $scope.deletePassiveManufacturer = function () {
        if ($scope.checkedPassiveManufacturerList.length === 0) {
            iziToast.show({
                message: "Silinecek Üretici Seçiniz.",
                position: 'topCenter',
                color: "error",
                icon: "fa fa-ban"
            });
            return;
        };

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/DeletePassiveManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedCustomer.Id, manufacturer: $scope.checkedPassiveManufacturerList }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.checkedPassiveManufacturerList = [];
            $scope.getManufacturers();
        });
    };

    $scope.getAuthorityCustomer = function () {

        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetAuthorityCustomer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedCustomer.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.fieldList = response.data.FieldList;
            $scope.authorityItem = response.data.AuthorityItem;

        });

    };

    $scope.checkAuthorityCustomer = function (field) {

        var updateValue = $scope.authorityItem[0][field];
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/UpdateAuthorityCustomer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.authorityItem[0].Id, updateValue: updateValue, field: field }
        }).then(function (response) {
            fireCustomLoading(false);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.getCustomerUserList = function () {
        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerUserList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id }
        }).then(function (response) {

            fireCustomLoading(false);
            console.log(response.data);
            $scope.userList = response.data;
            $scope.tableCustomerUsersParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
        });
    };

    $scope.showDeleteModal = function (row) {
        $scope.DeleteItem = row;
        $scope.deleteModalShow();

    };

    $scope.deleteCustomerUser = function (id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/DeleteCustomerUser",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {id : id}
        }).then(function (response) {

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            $scope.deleteModalClose();

            //tabları yenilemek için
            //$('#link-customerDetail-generalTab').click();

            $scope.getCustomerUserList();

            fireCustomLoading(false);
        });
    };

    $scope.deleteModalShow = function () {
        $('#modal-delete').modal('show');
    };

    $scope.deleteModalClose = function () {
        $('#modal-delete').modal('hide');
    };


    $(document).ready(function () {
        setDefaultDate();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetCurrencyTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.currencyList = response.data;
            $scope.clearRuleAdditionalData();
        });
    });

}]).directive('customCustomerSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Customers/CustomersSelect",
        controller: 'CustomerSelectController'
    };
}).directive('customProductRuleSelect', function () {
    return {
        restrict: 'E',

        templateUrl: "/Admin/Customers/RuleSelect",
        controller: 'CustomerRuleSelectController'


    };
}).directive('customSalesmanSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Salesmans/SalesmansSelect",
        controller: 'SalesmanSelectController'
    };
}).directive('customUserDetail', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Customers/UserDetail",
        controller: 'UserDetailController'
    };
}).directive('customProductGroupsSelect', function () {
    return {
        restrict: 'E',
        templateUrl: "/Admin/Products/ProductGroupsSelect",
        controller: 'ProductGroupsSelectController'
    };
}).directive('customItemProductSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Products/ProductsItemSelect",
        controller: 'ProductItemSelectController'
    };
}).directive('customProductManufacturerSelect', function () {
    return {
        restrict: 'E',
        templateUrl: "/Admin/Products/ManufacturerSelect",
        controller: 'ProductManufacturerSelectController'
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
}).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return item.replace("T", " ");
        }
        return "";
    };
}).filter("dateFilter2", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});