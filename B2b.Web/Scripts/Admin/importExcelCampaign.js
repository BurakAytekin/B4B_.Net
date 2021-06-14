adminApp.controller('importExcelCampaign', ['$scope', '$http', 'NgTableParams', '$filter', function ($scope, $http, NgTableParams, $filter) {

    $scope.campaignSearchCriteria = { Type: 1, T9Text: '' };
    $scope.lockUpdate = true;
    $scope.currencyList = {};
    $scope.tofasTypeList = {};
    $scope.uploadlist = {};

    $scope.clearValues = function () {
        $scope.selectedCampaign = {};
        $scope.campaignPromotionList = null;

    };
    $scope.dowlandFile = function () {
        var file = $scope.campaignSearchCriteria.Type !== 3 ? "toplu_kampanya.xlsx" : "toplu_kampanya_iskonto.xlsx";
        var url = "/files/download/" + file;
        window.open(window.location.origin + url, '_blank');
    };

    $scope.uploadCartExcelFile = function () {
        var fileUpload = document.getElementById('campaing-excel-selector');
        var files = fileUpload.files;

        if (files.length == 0) {
            iziToast.error({
                message: 'Lütfen dosya seçiniz',
                position: 'topCenter',
            });
            return;
        }

        var f = files[0];
        {
            fireCustomLoading(true);

            var reader = new FileReader();
            var name = f.name;
            reader.onload = function (e) {
                var data = e.target.result;
                var workbook = XLSX.read(data, { type: 'binary' });
                var jsonData;
                workbook.SheetNames.forEach(function (sheetName) {
                    jsonData = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName], { raw: true });
                });

                jsonData.forEach(function (e) {


                    //Ortak Ayarlar
                    e.ProductCode = e.STOK_KODU;
                    e.CampaignCode = e.KMP_KODU;
                    e.CampaignCode2 = e.KMP_KODU2;
                    e.MinOrder = e.MIN_MIKTAR;
                    e.TotalQuantity = e.KMP_MIKTAR;
                    e.EndQuantity = e.KMP_SON_MIKTAR;

                    //Net Fiyat Ve Kademeli Net Fiyat İçin Ayarlar
                    if ($scope.campaignSearchCriteria.Type === 1 || $scope.campaignSearchCriteria.Type === 4 || $scope.campaignSearchCriteria.Type === 2) {

                        e.PriceP = e.NET_FIYAT_PESIN;
                        e.PriceV = e.NET_FIYAT_VADELI;
                        e.CurrencyP = e.DOVIZ_TIP_PESIN;
                        e.CurrencyV = e.DOVIZ_TIP_VADELI;
                        e.Discount = 0;
                    }

                        // İskonto ve Kademeli iskonto İçin Ayarlar
                    else if ($scope.campaignSearchCriteria.Type === 3 || $scope.campaignSearchCriteria.Type === 5) {
                        e.Discount = e.ISKONTO;
                        e.DiscountPassive = e.ISK_SIFIRLA;
                    }

                  
                });
               
                $http({
                    method: "POST",
                    url: "/Campaign/UploadCampaing",
                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                    data: {
                        uploadList: jsonData,
                        campaignType: parseInt($scope.campaignSearchCriteria.Type),
                        startDate: $('#txtStartDate').val(),
                        endDate: $('#txtEndDate').val()

                    }

                }).then(function successCallback(response) {

                    $scope.uploadlist = response.data;
                    //    
                });
            }
        }

        fireCustomLoading(false);
        reader.readAsBinaryString(f);
    }
    $scope.saveCampaignExcel = function () {

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Campaign/SaveCampaignExcel",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { campaignList: $scope.uploadlist.SuccessList }

        }).then(function successCallback(response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.uploadlist = {};
        });
    }
    $(document).ready(function () {
        var date = new Date();
        $('.custom-datetime').datetimepicker({
            debug: false,
            format: 'DD/MM/YYYY',
            //maxDate: moment(),
            defaultDate: "01/01/" + date.getFullYear()

        });
    });


}]).directive('convertToNumber', function () {
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