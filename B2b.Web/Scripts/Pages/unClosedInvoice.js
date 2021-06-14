b2bApp.controller('UnClosedIncoiceController', function ($scope, $http) {


    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Finance/GetUnClosedData",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.unClosedList = response.data;
            fireCustomLoading(false);
        });
    };

       $scope.exportToExcel = function () {


        if ($scope.unClosedList.length > 0) {
            var newData = [];
            $scope.unClosedList.forEach(function (e) {

                if (e.Date == null || e.Date == '' || e.Date == undefined) {
                    e.Date = '';
                }
                else{
                    e.Date = moment(e.Date).format('DD/MM/YYYY');
                };

                if (e.DueDate == null || e.DueDate == '' || e.DueDate == undefined) {
                    e.DueDate = '';
                }
                else {
                    e.DueDate = moment(e.DueDate).format('DD/MM/YYYY');
                };

                newData.push({
                    "Tarih": e.Date,
                    "Vade Tarihi": e.DueDate,
                    "Evrak No": e.DocumentNo,
                    "İşlem Türü": e.TransactionType,
                    "Açıklama": e.Explanation,
                    "Borç": e.Debt.toFixed(2) + ' TL',
                    "Kalan": e.Remaining.toFixed(2) + ' TL',
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
                         "Kalan"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 10 }, { wch: 15 }, { wch: 15 }, { wch: 20 }, { wch: 20 }, { wch: 20 }, { wch: 20 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Kapatilmamis_Faturalar_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Kapatilmamis_Faturalar_Listesi.xlsx");
        }
       };


    $(document).ready(function () {
        $scope.loadData();
    });
});