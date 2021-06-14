adminApp.controller('campaignController', ['$scope', '$http', 'NgTableParams', '$filter', function ($scope, $http, NgTableParams, $filter) {

    $scope.campaignSearchCriteria = { Type: -1, T9Text: '' };

    $scope.lockUpdate = true;

    $scope.selectedProduct = {};
    $scope.gradualItemList = [];
    $scope.campaignPromotionList = [];

    $scope.deleteIds = '';//örnek format 1,2,3...
    $scope.deleteItem = {};

    $scope.checkAllCampaignList = function (checked) {
        angular.forEach($scope.campaignHeaderList, function (value) {
            value.Checked = checked;
            $scope.setDeleteIds(value);
        });
    };

    $scope.setDeleteIds = function (item) {
        if (item.Checked) {
            $scope.deleteIds += item.Id + ',';
        }
        else {
            $scope.deleteIds = $scope.deleteIds.replace((item.Id + ','), '');
        }
    };

    $scope.addgradualItem = function (gradualItem) {
        if ($scope.campaignSearchCriteria.Type === 4 && (gradualItem == undefined || gradualItem.PriceP == undefined || gradualItem.PriceP == "")) {
            iziToast.error({
                message: 'Lütfen Gerekli Alanları Doldurunuz ',
                position: 'topCenter'
            });
            return;
        }
        if ($scope.campaignSearchCriteria.Type === 5 && (gradualItem == undefined || gradualItem.Discount == undefined || gradualItem.Discount == "")) {
            iziToast.error({
                message: 'Lütfen Gerekli Alanları Doldurunuz ',
                position: 'topCenter'
            });
            return;
        }
        var item = angular.copy(gradualItem);

        $scope.gradualItemList.push(item);
        $scope.gradualItem = null;
    };
    $scope.removegradualItem = function (item) {
        var index = $scope.gradualItemList.indexOf(item);
        $scope.gradualItemList.splice(index, 1);
        if (item.Id != undefined && item.Id > 0) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Campaign/DeleteCampaignDetail",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: item.Id }

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
    $scope.clearValues = function () {
        $scope.selectedCampaign = { Id: 0, ProductId: 0, PriceP: 0, PriceV: 0, CurrencyP: 'TL', CurrencyV: 'TL', Discount: 0, MinOrder: 0, TotalQuantity: 0 };
        $scope.campaignPromotionList = [];
        $scope.gradualItemList = [];
        $scope.lockUpdate = true;
        $scope.deleteIds = '';
        firePriceFormat();
        firePriceFormatNumberOnly();
        $scope.getCurrencyList();
    };

    $scope.newRecord = function () {
        $scope.clearValues();
        $scope.lockUpdate = false;
    };

    $scope.setSelectedCampaign = function (row) {

        var item = angular.copy(row);



        item.StartDate = $scope.convertDate(row.StartDate);// $filter('date')(new Date(row.StartDate), 'dd/MM/yyyy');
        item.FinishDate = $scope.convertDate(row.FinishDate); //$filter('date')(new Date(row.FinishDate), 'dd/MM/yyyy');
        $scope.selectedCampaign = item;

        if ($scope.campaignSearchCriteria.Type === 6) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Campaign/GetCampaignPromotion",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { code: $scope.selectedCampaign.PromotionProductCode }

            }).then(function (response) {
                fireCustomLoading(false);
                $scope.campaignPromotionList = response.data;

            });
        }
        if ($scope.campaignSearchCriteria.Type === 4 || $scope.campaignSearchCriteria.Type === 5) {
            $scope.getCampaignDetail($scope.selectedCampaign.Id);
        }
    };
    $scope.getCampaignDetail = function (headerId) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Campaign/GetCampaigDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: headerId }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.gradualItemList = response.data;

        });
    };

    $scope.showDeleteModal = function () {
        if ($scope.deleteIds == undefined || $scope.deleteIds == '') {
            iziToast.error({
                message: 'Lütfen Silinecek Kampanya / lar ı Seçiniz.',
                position: 'topCenter'
            });
        } else {
            $scope.deleteItem.Name = 'Kampanya';
            $scope.deleteItem.Description = 'Seçilen kampanya / lar ı ';
            $scope.deleteItem.FunctionName = $scope.deleteCampaign;

            $('#modal-delete').modal('show');
        };
    };

    $scope.deleteCampaign = function () {

        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Admin/Campaign/DeleteCampaigns",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { DeleteIds: $scope.deleteIds }

        }).then(function (response) {
            fireCustomLoading(false);
            $('#modal-delete').modal('hide');
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getCampaignList();
            $scope.clearValues();
        });

    };

    $scope.convertDate = function (dateStr) {
        var dateString = dateStr.substr(0, 10);
        var currentTime = new Date(Date.parse(dateString));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var date = day + "." + month + "." + year;
        return date;
    };

    $scope.selectItemProduct = function () {
        $scope.ProductItemSelectOpen(String.empty, "ItemProduct");
    };

    $scope.saveCampaign = function () {
        if ($scope.selectedCampaign.ProductId === 0) {
            iziToast.error({
                message: 'Lütfen Ürün Seçiniz',
                position: 'topCenter'
            });
            return;
        }


        $scope.selectedCampaign.StartDate = $('#iCampaignStartDate').val();
        $scope.selectedCampaign.FinishDate = $('#iCampaignEndDate').val();
        $scope.selectedCampaign.Type = $scope.campaignSearchCriteria.Type;

        if (($scope.selectedCampaign.Type == 1 || $scope.selectedCampaign.Type == 2) && ($scope.selectedCampaign.CurrencyP == "" || $scope.selectedCampaign.CurrencyV == "" || $scope.selectedCampaign.CurrencyP == undefined || $scope.selectedCampaign.CurrencyV == undefined)) {
            iziToast.error({
                message: 'Lütfen Döviz Türü Seçiniz',
                position: 'topCenter'
            });
            return;
        }

        if ($scope.selectedCampaign.Type == 6) {
            $scope.selectedCampaign.PromotionProductCode = "";
            $.each($scope.campaignPromotionList, function (a, b) {
                $scope.selectedCampaign.PromotionProductCode += "'" + b.Code + ":" + b.TotalQuantity + ";" + b.TotalQuantityOnWay + "',";
            });
            if ($scope.selectedCampaign.PromotionProductCode.length > 1) {
                $scope.selectedCampaign.PromotionProductCode = $scope.selectedCampaign.PromotionProductCode.substr(0, $scope.selectedCampaign.PromotionProductCode.length - 1);
            }

        }
        fireCustomLoading(true);
        $scope.selectedCampaign.PriceP = parseFloat($('#iCampaignProductCashSalesPriceP').val());
        $scope.selectedCampaign.PriceV = parseFloat($('#iCampaignProductMaturitySalesPriceV').val());


        $http({
            method: "POST",
            url: "/Admin/Campaign/SaveCampaign",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { campaign: $scope.selectedCampaign, gradualItemList: $scope.gradualItemList }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getCampaignList();
            $scope.clearValues();


        });
    };



    $scope.$watch('ItemProduct', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            var product = newValue;
            $scope.selectedCampaign.ProductId = product.Id;
            $scope.selectedCampaign.ProductCode = product.Code;
            $scope.selectedCampaign.ProductName = product.Name;
            $scope.selectedCampaign.ProductManufacturer = product.Manufacturer;


        }

    }, true);


    $scope.selectItemProductDetail = function () {
        $scope.ProductItemSelectOpen(String.empty, "ItemProductDetail");
    };

    $scope.$watch('ItemProductDetail', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            var product = newValue;
            var item = angular.copy(product);
            $scope.campaignPromotionList.push(item);

        }

    }, true);

    $scope.removePromotionItem = function (item) {
        var index = $scope.campaignPromotionList.indexOf(item);
        $scope.campaignPromotionList.splice(index, 1);


    };


    $scope.exportToExcel = function () {

        if ($scope.campaignHeaderList.length > 0) {
            var newData = [];
            $scope.campaignHeaderList.forEach(function (e) {

                //Tüm tarihler için
                if (e.StartDate == null || e.StartDate == '' || e.StartDate == undefined) {
                    e.StartDate = '';
                } else {
                    e.StartDate = moment(e.StartDate).format('MM/DD/YYYY');
                }

                if (e.FinishDate == null || e.FinishDate == '' || e.FinishDate == undefined) {
                    e.FinishDate = '';
                } else {
                    e.FinishDate = moment(e.FinishDate).format('MM/DD/YYYY');
                }
                //Tüm tarihler için


                newData.push({
                    "Ürün Kodu": e.ProductCode,
                    "Ürün Adı": e.ProductName,
                    "Üretici": e.ProductManufacturer,
                    "Kod": e.Code,
                    "Peşin Satış Fiyatı": e.PriceP.toFixed(2) + ' TL',
                    "Vadeli Satış Fiyatı": e.PriceV.toFixed(2) + ' TL',
                    "Minimum Adet": e.MinOrder,
                    "Kampanyalı Ürün Miktarı": e.TotalQuantity,
                    "Satılan Ürün Miktarı": e.SaledQuantity,
                    "Baş. Tarihi": e.StartDate,
                    "Bitiş Tarihi": e.FinishDate,
                    "Açıklama": e.Notes,
                    "Aktif": e.IsActive == true ? 'Evet' : 'Hayır',
                    "Ana Ekranda Göster": e.BannerStatu == true ? 'Evet' : 'Hayır',
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                         "Ürün Kodu",
                         "Ürün Adı",
                         "Üretici",
                         "Kod",
                         "Peşin Satış Fiyatı",
                         "Vadeli Satış Fiyatı",
                         "Minimum Adet",
                         "Kampanyalı Ürün Miktarı",
                         "Satılan Ürün Miktarı",
                         "Baş. Tarihi",
                         "Bitiş Tarihi",
                         "Açıklama",
                         "Aktif",
                         "Ana Ekranda Göster"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 15 }, { wch: 70 }, { wch: 15 }, { wch: 15 }, { wch: 20 }, { wch: 20 }, { wch: 15 }, { wch: 20 }, { wch: 20 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 20 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Kampanya_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Kampanya_Listesi.xlsx");
        }
    };


    $(document).ready(function () {

    });

    $scope.getCurrencyList = function () {
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

    $scope.getCampaignList = function () {
        $scope.clearValues();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Campaign/GetCampaignHeader",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: $scope.campaignSearchCriteria.Type, t9Text: $scope.campaignSearchCriteria.T9Text }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.campaignHeaderList = response.data;
        });
    };

    var pnlCampaignElements = {
        pnlCampaignCriteriaSelection: $('#pnlCampaignCriteriaSelection'),
        pnlCampaignProductCode: $('#pnlCampaignProductCode'),
        pnlCampaignProductName: $('#pnlCampaignProductName'),
        pnlCampaignProductManufacturer: $('#pnlCampaignProductManufacturer'),
        pnlPackageResult: $('#pnlPackageResult'),
        pnlCampaignProductCashSalesPrice: $('#pnlCampaignProductCashSalesPrice'),
        pnlCampaignProductMaturitySalesPrice: $('#pnlCampaignProductMaturitySalesPrice'),
        pnlCampaignDiscount: $('#pnlCampaignDiscount'),
        pnlCampaignMinimumQuantity: $('#pnlCampaignMinimumQuantity'),
        pnlCampaignPlacedProductQuantity: $('#pnlCampaignPlacedProductQuantity'),
        pnlCampaignSoldProductQuantity: $('#pnlCampaignSoldProductQuantity'),
        pnlCampaignStartDate: $('#pnlCampaignStartDate'),
        pnlCampaignEndDate: $('#pnlCampaignEndDate'),
        pnlCampaignDescription: $('#pnlCampaignDescription'),
        pnlCampaignStatu: $('#pnlCampaignStatu'),
        pnlCampaignHomeVisible: $('#pnlCampaignHomeVisible'),
        pnlCampaignCode: $('#pnlCampaignCode'),
        pnlDiscountPassive: $('#pnlDiscountPassive'),
        pnlGradualResult: $('#pnlGradualResult'),
        pnlCampaignCode2: $('#pnlCampaignCode2'),
        pnlCampaignEndQuantity: $('#pnlCampaignEndQuantity'),
        pnlPromotionAllProduct: $('#pnlPromotionAllProduct')

    };

    function fireHideAnnouncementElements() {
        $.each(pnlCampaignElements, function (eName, eValue) {
            if (!eValue.hasClass("hide")) eValue.addClass("hide");
        });
    }

    $(window).load(function () {
        fireHideAnnouncementElements();

        //Initialize Mini Calendar Datepicker
        $('#iCampaignStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            //defaultDate: "01/01/2017",
            locale: "tr"
        });

        $('#iCampaignEndDate').datetimepicker({
            format: 'DD/MM/YYYY',
            defaultDate: new Date(),
            locale: 'tr'
        });

        $("#iCampaignType").change(function () {
            $scope.clearValues();
            $scope.getCampaignList();
            var elm = $(this),
                eText = elm.find("option:selected").text(),
                eValue = parseInt(elm.val());
            //alert("Element: " + eText + " - Değeri: " + eValue);

            fireHideAnnouncementElements();
            if (eValue != -1) {

                pnlCampaignElements.pnlCampaignProductCode.removeClass("hide");
                pnlCampaignElements.pnlCampaignProductName.removeClass("hide");
                pnlCampaignElements.pnlCampaignProductManufacturer.removeClass("hide");
                pnlCampaignElements.pnlCampaignStartDate.removeClass("hide");
                pnlCampaignElements.pnlCampaignEndDate.removeClass("hide");
                pnlCampaignElements.pnlCampaignDescription.removeClass("hide");
                pnlCampaignElements.pnlCampaignStatu.removeClass("hide");
                pnlCampaignElements.pnlCampaignMinimumQuantity.removeClass("hide");
                pnlCampaignElements.pnlCampaignPlacedProductQuantity.removeClass("hide");
                pnlCampaignElements.pnlCampaignSoldProductQuantity.removeClass("hide");
                pnlCampaignElements.pnlCampaignCode.removeClass("hide");
                pnlCampaignElements.pnlCampaignEndQuantity.removeClass("hide");
                pnlCampaignElements.pnlCampaignCode2.removeClass("hide");

            }

            switch (eValue) {
                case 1:
                case 2:
                    pnlCampaignElements.pnlCampaignProductCashSalesPrice.removeClass("hide");
                    pnlCampaignElements.pnlCampaignProductMaturitySalesPrice.removeClass("hide");
                    pnlCampaignElements.pnlCampaignHomeVisible.removeClass("hide");
                    break;
                case 4:
                    pnlCampaignElements.pnlCampaignHomeVisible.removeClass("hide");
                    pnlCampaignElements.pnlCampaignMinimumQuantity.addClass("hide");
                    pnlCampaignElements.pnlGradualResult.removeClass("hide");
                    break;

                case 3:
                    pnlCampaignElements.pnlCampaignDiscount.removeClass("hide");
                    pnlCampaignElements.pnlCampaignHomeVisible.removeClass("hide");
                    pnlCampaignElements.pnlDiscountPassive.removeClass("hide");

                    break;
                case 5:
                    pnlCampaignElements.pnlCampaignHomeVisible.removeClass("hide");
                    pnlCampaignElements.pnlCampaignMinimumQuantity.addClass("hide");
                    pnlCampaignElements.pnlDiscountPassive.removeClass("hide");
                    pnlCampaignElements.pnlGradualResult.removeClass("hide");
                    break;

                case 6:
                    pnlCampaignElements.pnlPackageResult.removeClass("hide");
                    pnlCampaignElements.pnlPromotionAllProduct.removeClass("hide");
                    break;


                default:
                    fireHideAnnouncementElements();
                    break;
            }
        });

        $("#iCampaignType").change();
    });

}]).directive('customItemProductSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Products/ProductsItemSelect",
        controller: 'ProductItemSelectController'
    };
}).directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return parseInt(val, 10);
            });

            ngModel.$formatters.push(function (val) {
                return '' + val;
            });
        }
    };
});