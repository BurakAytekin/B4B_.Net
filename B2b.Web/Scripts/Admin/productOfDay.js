adminApp.controller('productOfDayController', ['$scope', '$http', 'NgTableParams', '$filter', function ($scope, $http, NgTableParams, $filter) {

    $scope.campaignSearchCriteria = { T9Text: '' };

    $scope.lockUpdate = true;

    $scope.selectedProduct = {};

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



    $scope.clearValues = function () {
        $scope.selectedCampaign = {
            Id: 0, ProductId: 0, Price: 0, Currency: 'TL', Discount: 0, MinOrder: 0, TotalQuantity: 0, SaledQuantity: 0, RisingQuantity: 1, IsUseProductPicture: true, IsActive: true, IsOnlyOneOrder: true, Product: {}
        };

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
            url: "/Admin/Campaign/DeleteProductOfDays",
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

        if ($scope.selectedCampaign.Currency == "" || $scope.selectedCampaign.Currency == undefined) {
            iziToast.error({
                message: 'Lütfen Döviz Türü Seçiniz',
                position: 'topCenter'
            });
            return;
        }


        $scope.selectedCampaign.Price = parseFloat($('#iCampaignProductCashSalesPriceP').val());


        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Campaign/SaveProductOfDay",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { campaign: $scope.selectedCampaign, imageBaseIcon: $scope.image_sourceIcon }
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
            $scope.selectedCampaign.Product.Id = product.Id;
            $scope.selectedCampaign.Product.Code = product.Code;
            $scope.selectedCampaign.Product.Name = product.Name;
            $scope.selectedCampaign.Product.Manufacturer = product.Manufacturer;


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
                    "Ürün Kodu": e.Product.Code,
                    "Ürün Adı": e.Product.Name,
                    "Üretici": e.Product.Manufacturer,
                    "Fiyat": e.PriceStr,
                    "Minimum Adet": e.MinOrder,
                    "Kampanyalı Ürün Miktarı": e.TotalQuantity,
                    "Satılan Ürün Miktarı": e.SaledQuantity,
                    "Artış Miktarı": e.RisingQuantity,
                    "Baş. Tarihi": e.StartDate,
                    "Bitiş Tarihi": e.FinishDate,
                    "Açıklama": e.Explanation,
                    "Aktif": e.IsActive == true ? 'Evet' : 'Hayır',
                    "Tek Sipariş": e.IsUseProductPicture == true ? 'Evet' : 'Hayır',
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                    "Ürün Kodu",
                    "Ürün Adı",
                    "Üretici",
                    "Fiyat",
                    "Minimum Adet",
                    "Kampanyalı Ürün Miktarı",
                    "Satılan Ürün Miktarı",
                    "Artış Miktarı",
                    "Baş. Tarihi",
                    "Bitiş Tarihi",
                    "Açıklama",
                    "Aktif",
                    "Ana Ekranda Göster",
                    "Tek Sipariş"
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

    $scope.setFileImage = function (element) {
        $scope.currentFile = element.files[0];
        var reader = new FileReader();
        reader.onload = function (event) {
            $scope.image_sourceIcon = event.target.result;
            $scope.$apply()
        }
        reader.readAsDataURL(element.files[0]);
    };

    $(document).ready(function () {

    });

    $scope.getCurrencyList = function () {
        $http({
            method: "POST",
            url: "/Admin/Products/GetCurrencyTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.currencyList = response.data;
        });
    };

    $scope.getCampaignList = function () {
        $scope.clearValues();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Campaign/GetProductOfDayList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { t9Text: $scope.campaignSearchCriteria.T9Text }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.campaignHeaderList = response.data;
        });
    };



    $(window).load(function () {
        $scope.clearValues();
        //Initialize Mini Calendar Datepicker
        $('#iCampaignStartDate').datetimepicker({
            format: "DD/MM/YYYY  HH:mm",
            //defaultDate: "01/01/2017",
            locale: "tr"
        });

        $('#iCampaignEndDate').datetimepicker({
            format: 'DD/MM/YYYY HH:mm',
            defaultDate: new Date(),
            locale: 'tr'
        });
        $scope.getCampaignList();
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