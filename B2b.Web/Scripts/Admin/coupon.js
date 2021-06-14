adminApp.controller('couponController', ['$scope', '$http', 'NgTableParams', '$element', '$filter', function ($scope, $http, NgTableParams, $element, $filter) {

    $scope.selectedCoupon = {
        Type: 0,
        Price: 0,
        Manufacturers: '',
        ProductGroups: '',
        Rules: '',
        SpecialCodes: '',
        Code: '',
        CalculateType: 0,
        MinQuantity: 0
    };

    $scope.clearAllValues = function () {
        $scope.selectedCoupon = {
            Type: 0,
            Price: 0,
            Manufacturers: '',
            ProductGroups: '',
            Rules: '',
            SpecialCodes: '',
            Code: '',
            CalculateType: 0,
            MinQuantity: 0
        };

        $('#isIgnoreCampaign').prop('checked', false);
        $('#isCancelMainDisc').prop('checked', false);
        $('#isCancelAdditionalDisc').prop('checked', false);
        $('#isCancelManuelDisc').prop('checked', false);
        $('#isAutoUse').prop('checked', false);
        $('#isOnlySelectedItemTotal').prop('checked', false);
        $('#isJustAvailable').prop('checked', false);
        $('#isCounter').prop('checked', false);
        $('#isCounter').prop('checked', false);
        $('#isOneUsed').prop('checked', true);
        $("[name='customRadio']:checked").val("0");
        $("[name='customCalculateType']:checked").val("0");
        $("[name='customUserType']:checked").val("0");
        $('#iCampaignStartDate').val('');
        $('#iCampaignEndDate').val('');
        $scope.StartDate = '';
        $scope.EndDate = '';
    };

    $scope.changePrice = function () {
        $scope.priceLeft = $scope.selectedCoupon.Price.toString().split(',')[0];
        $scope.priceRight = $scope.selectedCoupon.Price.toString().split(',')[1];

    };

    $scope.convertDate = function (dateStr) {
        var dateString = dateStr.substr(0, 10);
        var currentTime = new Date(Date.parse(dateString));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var date = day + "." + month + "." + year;
        return date;
    }

    $scope.openNewCoupon = function (row) {
        $scope.clearAllValues();
        $scope.loadManufacturer();
        $scope.loadProductGroup();
        $scope.loadProductRule();
        $scope.loadProductSpecialCode();

        if (row === null) {
            $scope.generateCopunCode();

        }
        else {
            $('.controls a:last').tab('show');
            $scope.selectedCoupon = angular.copy(row);

            $('#isIgnoreCampaign').prop('checked', $scope.selectedCoupon.IsIgnoreCampaign);
            $('#isCancelMainDisc').prop('checked', $scope.selectedCoupon.IsCancelMainDisc);
            $('#isCancelAdditionalDisc').prop('checked', $scope.selectedCoupon.IsCancelAdditionalDisc);
            $('#isCancelManuelDisc').prop('checked', $scope.selectedCoupon.IsCancelManuelDisc);
            $('#isAutoUse').prop('checked', $scope.selectedCoupon.IsAutoUse);
            $('#isOnlySelectedItemTotal').prop('checked', $scope.selectedCoupon.IsOnlySelectedItemTotal);
            $('#isJustAvailable').prop('checked', $scope.selectedCoupon.IsJustAvailable);
            $('#isCounter').prop('checked', $scope.selectedCoupon.IsCounter);
            $('#isOneUsed').prop('checked', $scope.selectedCoupon.IsOneUsed);
            $("[name='customRadio']")[$scope.selectedCoupon.Type].checked = true;
            $("[name='customCalculateType']")[$scope.selectedCoupon.CalculateType].checked = true;
            $("[name='customUserType']")[$scope.selectedCoupon.UserType].checked = true;
            $("[name='customRadio']:checked").val($scope.selectedCoupon.Type);
            $("[name='customCalculateType']:checked").val($scope.selectedCoupon.CalculateType);
            $("[name='customUserType']:checked").val($scope.selectedCoupon.UserType);
            $('#iCampaignStartDate').val($scope.convertDate($scope.selectedCoupon.StartDate));
            $('#iCampaignEndDate').val($scope.convertDate($scope.selectedCoupon.EndDate));

            setDate();
        }

    };

    $scope.updateCouponPriority = function (_Id, _Priority) {
        if (_Priority === null) return false;
        fireCustomLoading(true);
        var result = false;
        angular.forEach($scope.CouponIdList, function (rowa) {
            if (rowa.Id === _Priority) result = true;
        });
        if (result) {
            iziToast.show({
                message: 'Belirtilen öncelik farklı bir kuponda tanımlıdır.',
                position: 'topCenter',
                color: 'error',
                icon: 'fa fa-ban'
            });
            $scope.loadCouponList();
            fireCustomLoading(false);
            return false;
        }
        $http({
            method: "POST",
            url: "/Admin/Coupon/UpdateCouponPriority",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { _Id: _Id, _Priority: _Priority }
        }).then(function (response) {
            $scope.loadCouponList();
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.generateCopunCode = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/GenerateCouponCode",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            var item = response.data;
            $scope.selectedCoupon.Code = item.Code;
        });
    }

    $scope.setAllCustomers = function (row) {

        $.confirm({
            title: 'Uyarı!',
            content: "Seçili kupon tanımlı olmayan tüm müşterilere tanımlanacaktır. Emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {

                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Admin/Coupon/SetAllCustomers",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: { couponId: row.Id }
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
                },
                Cancel: {
                    text: "Hayır",
                    btnClass: 'btn-red any-other-class',
                    action: function () {
                        iziToast.error({
                            message: 'İşleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });
                    }
                }
            }
        });

    };

    $scope.addManufacturerText = function (row, check) {
        if (check) {
            if (row.Name === 'Hepsi') {
                $scope.selectedCoupon.Manufacturers = '';

                angular.forEach($scope.originalManufacturerData, function (row) {
                    if (row.Name !== 'Hepsi') {
                        $scope.selectedCoupon.Manufacturers += '<' + row.Name + '>';

                    }
                });

            } else {
                var manufacturer = $scope.selectedCoupon.Manufacturers;
                $scope.selectedCoupon.Manufacturers = manufacturer + '<' + row.Name + '>';

            }
        }
        else {

            if (row.Name === 'Hepsi') {
                $scope.selectedCoupon.Manufacturers = '';

            } else {
                var manufacturer = '<' + row.Name + '>';
                var newStr = $scope.selectedCoupon.Manufacturers.replace(manufacturer, "");
                $scope.selectedCoupon.Manufacturers = newStr;

                //Hepsi seçeneğini kaldır..
                var x = $('#iChkAcitveManufacturer').prop('checked', false);

            }
        }
    };

    $scope.addProductGroupText = function (row, check) {
        if (check) {

            if (row.Name === 'Hepsi') {

                $scope.selectedCoupon.ProductGroups = '';

                angular.forEach($scope.originalProductGrouprData, function (row) {
                    if (row.Name !== 'Hepsi') {
                        $scope.selectedCoupon.ProductGroups += '<' + row.Name + '>';
                    }
                });

            } else {
                var productGroup = $scope.selectedCoupon.ProductGroups;
                $scope.selectedCoupon.ProductGroups = productGroup + '<' + row.Name + '>';
            }
        }
        else {
            if (row.Name === 'Hepsi') {
                $scope.selectedCoupon.ProductGroups = '';

            } else {
                var productGroup = '<' + row.Name + '>';
                var newStr = $scope.selectedCoupon.ProductGroups.replace(productGroup, "").replace('<Hepsi>', "");;
                $scope.selectedCoupon.ProductGroups = newStr;

                //Hepsi seçeneğini kaldır..
                var x = $('#iChkAcitveProductGroup').prop('checked', false);
            }
        }
    };

    $scope.addRulesText = function (row, check) {
        if (check) {

            if (row.Product === 'Hepsi') {

                $scope.selectedCoupon.Rules = '';

                angular.forEach($scope.originalRuleData, function (row) {
                    if (row.Product !== 'Hepsi') {
                        $scope.selectedCoupon.Rules += '<' + row.Product + '>';
                    };
                });

            } else {
                var rule = $scope.selectedCoupon.Rules;
                $scope.selectedCoupon.Rules = rule + '<' + row.Product + '>';
            }
        }
        else {

            if (row.Product === 'Hepsi') {

                $scope.selectedCoupon.Rules = '';

            } else {
                var rule = '<' + row.Product + '>';
                var newStr = $scope.selectedCoupon.Rules.replace(rule, "").replace('<Hepsi>', "");
                $scope.selectedCoupon.Rules = newStr;

                //Hepsi seçeneğini kaldır..
                var x = $('#iChkAcitveRule').prop('checked', false);

            }
        }
    };

    $scope.addSpecialCodesText = function (row, check) {
        if (check) {

            if (row.SpecialCode1 === 'Hepsi') {

                $scope.selectedCoupon.SpecialCodes = '';

                angular.forEach($scope.originalSpecialCodeData, function (row) {
                    if (row.SpecialCode1 !== 'Hepsi') {
                        $scope.selectedCoupon.SpecialCodes += '<' + row.SpecialCode1 + '>';

                    }
                });

            } else {

                var specialCode = $scope.selectedCoupon.SpecialCodes;
                $scope.selectedCoupon.SpecialCodes = specialCode + '<' + row.SpecialCode1 + '>';
            }



        }
        else {

            if (row.SpecialCode1 === 'Hepsi') {

                $scope.selectedCoupon.SpecialCodes = '';

            } else {
                var specialCode = '<' + row.SpecialCode1 + '>';
                var newStr = $scope.selectedCoupon.SpecialCodes.replace(specialCode, "").replace('<Hepsi>', "");;
                $scope.selectedCoupon.SpecialCodes = newStr;

                //Hepsi seçeneğini kaldır..
                var x = $('#iChkAcitveSpecialCode').prop('checked', false);

            }




        }
    };

    $scope.loadManufacturer = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.manufacturerList = response.data;

            $scope.tableActiveManufacturerParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.manufacturerList)
            });

            $scope.originalManufacturerData = angular.copy($scope.manufacturerList);

        });
    };

    $scope.loadProductGroup = function () {

        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsGroup1List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.productGroupList = response.data;

            $scope.tableProductGroupParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.productGroupList)
            });

            $scope.originalProductGrouprData = angular.copy($scope.productGroupList);

        });
    };

    $scope.loadProductRule = function () {

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/GetProductsRuleList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.productRuleList = response.data;

            $scope.tableRulesParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.productRuleList)
            });

            $scope.originalRuleData = angular.copy($scope.productRuleList);

        });
    };

    $scope.loadProductSpecialCode = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/GetProductsSpecialCodeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.productSpecialCodeList = response.data;

            $scope.tableSpecialCodesParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.productSpecialCodeList)
            });

            $scope.originalSpecialCodeData = angular.copy($scope.productSpecialCodeList);

        });
    };

    $scope.changeActivity = function (row) {

        var active = angular.copy(!row.IsActive);
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/ChangeActivity",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { couponId: row.Id, active: active }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            $scope.loadCouponList();
        });

    };

    $scope.loadCouponList = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/GetCouponList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.couponList = response.data;
        });
    };

    $scope.getCopunCustomerList = function (row, type) {
        $scope.selectedCoupon = row;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/GetCouponCustomersList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { couponId: row.Id, type: type }
        }).then(function (response) {
            fireCustomLoading(false);

            if (type === 0) {
                $scope.couponCustomerList = response.data;

                $scope.tableCouponCustomersParams = new NgTableParams({}, {
                    filterDelay: 0,
                    dataset: angular.copy($scope.couponCustomerList)
                });

                $('#mCouponCustomers').appendTo("body").modal('show');
            }
            else {
                $scope.couponCustomerAddList = response.data;

                $scope.tableCouponCustomersAddParams = new NgTableParams({}, {
                    filterDelay: 0,
                    dataset: angular.copy($scope.couponCustomerAddList)
                });
            }
        });
    };

    $scope.addCouponCustomers = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/SaveCouponCustomers",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { couponCustomers: row }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.getCopunCustomerList($scope.selectedCoupon, 1);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.changeStatus = function (row) {
        row.IsActive = !row.IsActive;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/SaveCouponCustomers",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { couponCustomers: row }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.getCopunCustomerList($scope.selectedCoupon, 0);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.deleteCustomerCoupon = function (row) {

        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {
                        row.Deleted = true;
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Admin/Coupon/SaveCouponCustomers",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: { couponCustomers: row }
                        }).then(function (response) {
                            fireCustomLoading(false);
                            $scope.getCopunCustomerList($scope.selectedCoupon, 0);
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

    $scope.deleteCoupon = function (row) {

        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {
                        row.Deleted = true;
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Admin/Coupon/SaveCoupon",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: { coupon: row }
                        }).then(function (response) {
                            fireCustomLoading(false);
                            $scope.clearAllValues();
                            $scope.loadCouponList();
                            $('.controls a:first').tab('show');
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

    $scope.saveCoupon = function () {
        //$('#approvalComment').prop('checked');

        $scope.selectedCoupon.IsIgnoreCampaign = $('#isIgnoreCampaign').prop('checked');
        $scope.selectedCoupon.IsCancelMainDisc = $('#isCancelMainDisc').prop('checked');
        $scope.selectedCoupon.IsCancelAdditionalDisc = $('#isCancelAdditionalDisc').prop('checked');
        $scope.selectedCoupon.IsCancelManuelDisc = $('#isCancelManuelDisc').prop('checked');
        $scope.selectedCoupon.IsAutoUse = $('#isAutoUse').prop('checked');
        $scope.selectedCoupon.IsOnlySelectedItemTotal = $('#isOnlySelectedItemTotal').prop('checked');
        $scope.selectedCoupon.IsJustAvailable = $('#isJustAvailable').prop('checked');
        $scope.selectedCoupon.IsCounter = $('#isCounter').prop('checked');
        $scope.selectedCoupon.IsOneUsed = $('#isOneUsed').prop('checked');
        $scope.selectedCoupon.Type = $("[name='customRadio']:checked").val();
        $scope.selectedCoupon.CalculateType = $("[name='customCalculateType']:checked").val();
        $scope.selectedCoupon.UserType = $("[name='customUserType']:checked").val();
        $scope.selectedCoupon.StartDate = $('#iCampaignStartDate').val();
        $scope.selectedCoupon.EndDate = $('#iCampaignEndDate').val();

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/SaveCoupon",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { coupon: $scope.selectedCoupon }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.clearAllValues();
            $scope.loadCouponList();
            $('.controls a:first').tab('show');
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.getCouponStatistics = function (row) {
        $scope.selectedCoupon = angular.copy(row);
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Coupon/GetCouponStatistics",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { couponId: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.couponStatistics = response.data;
            $('#mCouponStatistics').appendTo("body").modal('show');
        });
    };

    $scope.keypressEventSearchCoupon = function (e) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            $scope.selectItemProductCoupon();
            e.preventDefault();
        }
    };

    $scope.selectItemProductCoupon = function () {
        $scope.ProductItemSelectOpen(String.empty, "ItemProduct");
    };

    $scope.$watch('ItemProduct', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            var product = newValue;
            $scope.selectedCoupon.Product = product;
            $scope.selectedCoupon.ProductCode = product.Code;
        }

    }, true);

    $(document).ready(function () {

        $scope.loadCouponList();


        $('#iCampaignStartDate').datetimepicker({
            format: "DD.MM.YYYY",
            //defaultDate: "01/01/2017",
            locale: "tr"
        });

        $('#iCampaignEndDate').datetimepicker({
            format: 'DD.MM.YYYY',
            defaultDate: new Date(),
            locale: 'tr'
        });
    });

    setDate = function () {
        $scope.StartDate = $('#iCampaignStartDate').val();
        $scope.EndDate = $('#iCampaignEndDate').val();
    };

}]).directive('customItemProductSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Products/ProductsItemSelect",
        controller: 'ProductItemSelectController'
    };
});