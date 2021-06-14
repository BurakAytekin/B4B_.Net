adminApp.controller('analysisController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {
    $scope.analysisControl = false;
    $scope.value = 0;

    $scope.fireProgressbar = function () {
        $scope.value += 10;
        $('#prStatus').attr('style', ('width:' + $scope.value + '%'));
    };

    $scope.setMessage = function (message, type) {
        if (type === 0)
            $('#logShow').append('<span style="font-weight:bold"> ' + message + '</span>');
        else if (type === 1) {
            $('#logShow').append('<span style="color:green;font-weight:bold">-- ' + message + '</span>');
        }
        else {
            $('#logShow').append('<span style="color:red;font-weight:bold">-- ' + message + '</span>');//prepend
        }

    };

    $scope.stopAnalaysis = function () {
        $scope.analysisControl = false;
        $scope.value = 0;
    };

    $scope.startAnalaysis = function () {
        $scope.value = 0;
        $scope.analysisControl = true;
        $('#logShow').empty();
        $scope.checkCompanySettings();
    };

    $scope.checkCompanySettings = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Firma Ayarları Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/CompanySettings/GetSettingList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.companySettings = response.data;
                if ($scope.companySettings.length >0) {
                    $scope.setMessage('Firma tanımları bulundu</br>', 1);
                    $scope.checkSyncControl();
                }
                else
                    $scope.setMessage('İşlemlerin devam edebilmesi için firma tanımlarınızın yapılması gerekmektedir.</br>', 2);

                $scope.fireProgressbar();
            });
        }
    };

    $scope.checkSyncControl = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Sync Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/Sync/GetActiveTrasnferTypeList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.syncList = response.data;
                if ($scope.syncList.length > 0) {
                    $scope.setMessage('Sync tanımları bulundu</br>', 1);
                }
                else
                    $scope.setMessage('Sync tanımları bulunamamıştır</br>', 2);
                $scope.fireProgressbar();
                $scope.connectWebService();
            });
        }
    };

    $scope.connectWebService = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Webservice Bağlantı Kurma Denemesi</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/CheckWebServiceConnection",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { settings: $scope.companySettings[0] }//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Webservice bağlantı kurma başarılı</br>', 1);
                }
                else
                    $scope.setMessage('Webservice bağlantı kurma başarısız. ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.connectWindowsService();
            });
        }
    };

    $scope.connectWindowsService = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Windows Service Bağlantı Kurma Denemesi</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/CheckWindowsServiceConnection",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { settings: $scope.companySettings[0] }//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Windowsservice bağlantı kurma başarılı</br>', 1);
                }
                else
                    $scope.setMessage('Windowsservice bağlantı kurma başarısız. ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.checkFileUpload();
            });
        }
    };

    $scope.checkFileUpload = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Dosya Yükleme Denemesi</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/UploadFileControl",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Dosya yükleme başarılı</br>', 1);
                }
                else
                    $scope.setMessage('Dosya yükleme başarısız. ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.checkRuleCount();
            });
        }
    };

    $scope.checkRuleCount = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Koşul Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/CheckRuleCount",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Koşul mevcutluğu tespit edildi</br>', 1);
                }
                else
                    $scope.setMessage('Koşul tespit edilemedi lütfen koşul tanımı yapınız. ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.checkRuleDublicateCount();
            });
        }
    };

    $scope.checkRuleDublicateCount = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Koşul Çiftleme Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/CheckRuleDublicateCount",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Koşul çiftlemesi tespit edilmedi</br>', 1);
                }
                else
                    $scope.setMessage('Koşul çiftlemesi tespit edildi. Lütfen koşul tanımlarınızı kontrol ediniz. Çiftleyen koşul sayısı : ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.checkCustomerAndProductRule();
            });
        }
    };

    $scope.checkCustomerAndProductRule = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Stok ve Cari Koşul Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/CheckCustomerAndProductRule",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Stok ve Cari koşul tanımları mevcut</br>', 1);
                }
                else
                    $scope.setMessage('Stok ve/veya Cari koşullarınızda eşleşmezlik tespit edildi. Lütfen koşul tanımlarınızı kontrol ediniz. ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.checkPrice();
            });
        }
    };

    $scope.checkPrice = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Fiyat Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/SystemAnalysis/CheckPriceControl",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}//, 
            }).then(function (response) {
                $scope.responseItem = response.data;
                if ($scope.responseItem.Result === true) {
                    $scope.setMessage('Fiyat tanımları mevcut</br>', 1);
                }
                else
                    $scope.setMessage('Fiyatı olmayan ürünler tespit edildi. Lütfen fiyat tanımlarınızı kontrol ediniz. Fiyatı olmayan ürün sayısı: ' + $scope.responseItem.Message + '</br>', 2);

                $scope.fireProgressbar();
                $scope.checkWebServiceSettings();
            });
        }
    };


    $scope.checkWebServiceSettings = function () {
        if ($scope.analysisControl) {
            $scope.setMessage('--> Webservice Kontrolü Başlatıldı</br>', 0);
            $http({
                method: "POST",
                url: "/Admin/ErpFunctions/GetErpFunctionTypeList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                async: false,
                data: {},
            }).then(function (response) {
                $scope.erpFunctionList = response.data;
                if ($scope.erpFunctionList.length > 0) {
                    $scope.setMessage('Webservice tanımları bulundu. Kontroller başlatılıyor</br>', 1);
                    angular.forEach($scope.erpFunctionList, function (value, key) {

                        $http({
                            method: "POST",
                            url: "/Admin/ErpFunctions/GetErpFunctionDetail",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            async: false,
                            data: { typeId: value.Id }//, 
                        }).then(function (response) {
                            $scope.erpFunctionListDetail = response.data;
                            if ($scope.erpFunctionListDetail.length > 0) {
                                $scope.setMessage(value.Name + ' tanımları bulundu</br>', 1);
                            }
                            else
                                $scope.setMessage(value.Name + ' tanımları bulunamamıştır</br>', 2);

                        });

                    });

                    $scope.fireProgressbar();

                }
                else {
                    $scope.setMessage('Webservice tanımları bulunamadı. Sistem yöneticinize başvurunuz</br>', 2);
                    $scope.fireProgressbar();
                }
               // $scope.connectWebService();
            });
        }
    };


}]);