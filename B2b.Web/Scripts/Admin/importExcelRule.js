adminApp.controller('importExcelRule', ['$scope', '$http', 'NgTableParams', '$filter', function ($scope, $http, NgTableParams, $filter) {


    $scope.lockUpdate = true;
    $scope.uploadlist = {};


    $scope.dowlandFile = function () {
        var file = "toplu_kosul.xlsx";
        var url = "/files/download/" + file;
        window.open(window.location.origin + url, '_blank');
    };

    $scope.uploadExcelFile = function () {
        var fileUpload = document.getElementById('rule-excel-selector');
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
                    e.Product = e.Stok;
                    e.Customer = e.Cari;
                    e.PaymentType = e.Odeme_Tipi;
                    e.Disc1 = e.Isk1;
                    e.Disc2 = e.Isk2;
                    e.Disc3 = e.Isk3;
                    e.Disc4 = e.Isk4;
                    e.PriceNumber = e.Stok_Fiyat_Num;
                    e.Rate = e.Zam_Orani;

                    delete e.Stok;
                    delete e.Cari;
                    delete e.Odeme_Tipi;
                    delete e.Isk1;
                    delete e.Isk2;
                    delete e.Isk3;
                    delete e.Isk4;
                    delete e.Stok_Fiyat_Num;
                    delete e.Zam_Orani;

                });

                $http({
                    method: "POST",
                    url: "/Admin/BatchProcessing/UploadRule",
                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                    data: {
                        uploadList: jsonData
                       
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
    $scope.saveRuleExcel = function () {
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Admin/BatchProcessing/SaveRuleExcel",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { ruleList: $scope.uploadlist }

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