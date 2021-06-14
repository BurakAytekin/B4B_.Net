b2bApp.controller('financeController', ['$scope', '$http', '$element', function ($scope, $http, $element) {
    $scope.financeYearList = {};

    $scope.loadFinanceData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Finance/GetFinanceYear",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.financeYearList = response.data;
            fireCustomLoading(false);
            //angular.forEach($scope.fileList, function (data) {
              
            //});
        });
    };


    $scope.checkAuthorized = function () {
        $http({
            method: "POST",
            url: "/Finance/CheckAuthorized",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.response = response.data;
            $scope.listFinance($scope.response);
        });
    };

    $scope.listFinance = function (type) {
        if (type) {
            $.confirm({
                title: 'Cari Hesap Şifresi',
                content: '' +
           '<form action="" class="formName">' +
           '<div class="form-group">' +
           '<input type="password"  placeholder="şifre" class="fixedPriceValue form-control eryaz-numeric-input-onlynumber" required value="" />' +
           '</div>' +
           '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Listele',
                        btnClass: 'btn-blue',
                        keys: ['enter'],
                        action: function () {
                            var password = this.$content.find('.fixedPriceValue').val();
                            if (!password) {
                                return false;
                            }
                            fireCustomLoading(true);
                            var startDate = $('#datetimepicker1').val();
                            var endDate = $('#datetimepicker2').val();
                            var year = $('#financeYear-select').val();

                            if (year === null) {
                                fireCustomLoading(false); return;
                            }

                            $http({
                                method: "POST",
                                url: "/Finance/GetFinanceList",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { startDate: startDate, endDate: endDate, year: year, password: password }
                            }).then(function (response) {
                                if (response.data.Result)
                                    $scope.financeList = response.data.List;
                                else {
                                    $scope.financeList = {};
                                    iziToast.show({
                                        message: "Şifreniz yanlış lütfen tekrar işlem deneyiniz.",
                                        position: 'topCenter',
                                        color: "error",
                                        icon: "fa fa-ban"
                                    });
                                    fireCustomLoading(false);
                                    return false;
                                }

                                $scope.getSubTotals($scope.financeList);
                                $scope.getFinanceMonth();
                                $scope.getUnClosedBalanceMonth();
                            });
                        }

                    },
                    Vazgeç: function () {
                        //close
                    }
                },
                scrollToPreviousElement: false,
                scrollToPreviousElementAnimate: false,
                onOpen: function () {
                }
            });
        }
        else {
            fireCustomLoading(true);
            var startDate = $('#datetimepicker1').val();
            var endDate = $('#datetimepicker2').val();
            var year = $('#financeYear-select').val();

            if (year === null) {
                fireCustomLoading(false); return;
            }

            $http({
                method: "POST",
                url: "/Finance/GetFinanceList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { startDate: startDate, endDate: endDate, year: year, password: '-1' }
            }).then(function (response) {
                console.log(response.data);
                if (response.data.Result)
                    $scope.financeList = response.data.List;
                else {
                    $scope.financeList = {};
                    iziToast.show({
                        message: "Şifreniz yanlış lütfen tekrar işlem deneyiniz.",
                        position: 'topCenter',
                        color: "error",
                        icon: "fa fa-ban"
                    });
                    fireCustomLoading(false);
                    return false;
                }

                $scope.getSubTotals($scope.financeList);
                //$scope.getFinanceMonth();
               // $scope.getUnClosedBalanceMonth();
            });
        }

    };


    $scope.getSubTotals = function (list) {
        
        $http({
            method: "POST",
            url: "/Finance/GetSubTotals",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { list: list}
        }).then(function (response) {
            $scope.subTotalItem = response.data;
            fireCustomLoading(false);
        });

    };

   $scope.$on('ngRepeatFinanceYearFinished', function (ngRepeatFinanceYearFinishedEvent) {
        
        $('.selectbox-finance').SumoSelect();
        $('.selectbox-finance')[0].sumo.reload();
        $('.selectbox-finance').on('sumo:closed', function (sumo) {

        });

   });


    
    $scope.exportToExcel = function () {


        if ($scope.financeList.length > 0) {
            var newData = [];
            $scope.financeList.forEach(function (e) {

                if (e.Date == null || e.Date == '' || e.Date == undefined) {
                    e.Date = '';
                }
                else{
                    e.Date = moment(e.Date).format('MM/DD/YYYY');
                };

                if (e.DueDate == null || e.DueDate == '' || e.DueDate == undefined) {
                    e.DueDate = '';
                }
                else {
                    e.DueDate = moment(e.DueDate).format('MM/DD/YYYY');
                };

                newData.push({
                    "Tarih": e.Date,
                    "Vade Tarihi": e.DueDate,
                    "Evrak No": e.DocumentNo,
                    "İşlem Türü": e.TransactionType,
                    "Açıklama": e.Explanation,
                    "Borç": e.Debt.toFixed(2) + ' TL',
                    "Alacak": e.Credit.toFixed(2) + ' TL',
                    "Bakiye": e.Balance.toFixed(2) + ' TL',
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                         "Tarih",
                         "Vade Tarihi",
                         "Evrak No",
                         "İşlem Türü",
                         "Açıklama",
                         "Borç",
                         "Alacak",
                         "Bakiye"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 10 }, { wch: 15 }, { wch: 15 }, { wch: 20 }, { wch: 20 }, { wch: 20 }, { wch: 20 }, { wch: 20 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "CariHesap_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "CariHesap_Listesi.xlsx");
        }
    };



    $(document).ready(function () {
        $scope.loadFinanceData();
    });

 


}]).directive('onFinishRender', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {//ng repeat dönerken son kayıtmı diye bakıyorum
                $timeout(function () {
                    scope.$emit(attr.onFinishRender);
                });
            }
        }
    };
});