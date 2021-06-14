b2bApp.controller('BackOrderController', function ($scope, $http) {


    $scope.loadYearOptions = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Order/GetBackOrderYear",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.yearList = response.data;
            if ($scope.yearList.length > 0) {
                $scope.loadBackOrder($scope.yearList[0].Id);
                $scope.selectedIem = $scope.yearList[0].Id;
            }
            
            //else bulunamadı
        });
    };


    $scope.loadBackOrder = function (yearItemId) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Order/GetBackOrderData",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { yearItemId: yearItemId }

        }).then(function (response) {
            $scope.backOrderList = response.data;
            fireCustomLoading(false);
        });
    };

    $scope.addBasket = function (item) {
        $http({
            method: "POST",
            url: "/Order/AddBasket",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { item: item }

        }).then(function (response) {
            var retVal = jQuery.parseJSON(response.data);
            if (retVal.statu === "success") {
                iziToast.success({
                    message: retVal.message,
                    position: 'topCenter'
                });
                $scope.deleteBackOrder(item,true);
            }
            else {
                iziToast.error({
                    //title: 'Hata',
                    message: retVal.message,
                    position: 'topCenter'
                });

            }

        });
    };

    $scope.deleteBackOrder = function (item, type) {

        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "silmek istediğinize emin misiniz?",
                buttons:
                    {
                        Ok: {
                            text: "Evet",
                            btnClass: 'btn-blue',
                            action: function () { $scope.deleteBackOrder(item, true); }
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
                    },
                //scrollToPreviousElement: false,
                //scrollToPreviousElementAnimate: false
            });
        }
        else {
            $http({
                method: "POST",
                url: "/Order/DeleteBackOrder",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { item: item, yearItemId: $scope.selectedIem }

            }).then(function (response) {
                var retVal = jQuery.parseJSON(response.data);
                if (retVal.statu === "success") {
                    iziToast.success({
                        message: retVal.message,
                        position: 'topCenter'
                    });
                    $scope.loadBackOrder($scope.selectedIem);
                }
                else {
                    iziToast.error({
                        //title: 'Hata',
                        message: retVal.message,
                        position: 'topCenter'
                    });

                }

                if ($scope.yearList.length > 0)
                    $scope.loadBackOrder($scope.yearList[0]);
            });
        }
    }

    $scope.exportToExcel = function () {


        if ($scope.backOrderList.length > 0) {
            var newData = [];
            $scope.backOrderList.forEach(function (e) {

                if (e.OrderDate == null || e.OrderDate == '' || e.OrderDate == undefined) {
                    e.OrderDate = '';
                } else {
                    e.OrderDate = moment(e.OrderDate).format('MM/DD/YYYY');
                };

                newData.push({
                    "Stok": e.AvailabilityText,
                    "Sipariş Tarihi": e.OrderDate,
                    "Ürün Kodu": e.ProductCode,
                    "Ürün Adı": e.ProductName,
                    "Sip. Mikt.": e.QuantityTotal,
                    "Sevk Mik.": e.QuantitySent,
                    "Kalan Mik.": e.QuantityRemaining
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                         "Stok",
                         "Sipariş Tarihi",
                         "Ürün Kodu",
                         "Ürün Adı",
                         "Sip. Mikt.",
                         "Sevk Mik.",
                         "Kalan Mik."
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 10 }, { wch: 15 }, { wch: 15 }, { wch: 60 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "BackOrder_Siparis_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "BackOrder_Siparis_Listesi.xlsx");
        }
    };

    $(document).ready(function () {
        $scope.loadYearOptions();
    });

});