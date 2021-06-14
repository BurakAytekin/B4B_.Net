b2bApp.controller('OrderHeaderController', function ($scope, $http) {
   
    $scope.getOrderHeaderList = function () {
        var dateStart = $('#dateStart').val();
        var datetEnd = $('#dateEnd').val();
        var searchText = $('#txtGeneralSearch').val();
        $http({
            method: "POST",
            url: "/Order/GetOrderHeaderList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {  dateStart:dateStart,  datetEnd:datetEnd, searchText:searchText }

        }).then(function (response) {
            if (response.data.length === 0) {
                iziToast.warning({
                 
                    message: 'Belirtilen Kriterlerde Sipariş Bulunamadı',
                    position: 'topCenter'
                });
            }
              else
            {
                $scope.orderHeaderList = response.data;
            }
            
        });

    };
    $scope.keypressEvent = function (e) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 13) {
            $scope.getOrderHeaderList();
        }
    };
    $scope.resetValue = function () {
        $('#txtGeneralSearch').val('');
        var date = new Date();
        date.setDate(date.getDate() - 30);

        $('#dateStart').val($scope.getFormattedDate(date));
        $('#dateEnd').val($scope.getFormattedDate(new Date()));
    
    };
    $scope.getFormattedDate = function (date) {
        var year = date.getFullYear();

        var month = (1 + date.getMonth()).toString();
        month = month.length > 1 ? month : '0' + month;

        var day = date.getDate().toString();
        day = day.length > 1 ? day : '0' + day;

        return day + '/' + month + '/' + year;
    };
    $scope.responseOrderDetail = function (orderId) {
        console.log(orderId);
        $scope.OrderIds = orderId;
        $http({
            method: "POST",
            url: "/Order/ResponseOrderDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { orderId:orderId}

        }).then(function (response) {
            //window.location = "Order/OrderDetail";
            window.open('Order/OrderDetail', '_blank');
        });
    };
 

    $scope.exportToExcel = function () {


        if ($scope.orderHeaderList.length > 0) {
            var newData = [];
            $scope.orderHeaderList.forEach(function (e) {

                if (e.CreateDate == null || e.CreateDate == '' || e.CreateDate == undefined) {
                    e.CreateDate = '';
                } else {
                    e.CreateDate = moment(e.CreateDate).format('MM/DD/YYYY');
                };

                if (e.ConfirmDate == null || e.ConfirmDate == '' || e.ConfirmDate == undefined) {
                    e.ConfirmDate = '';
                } else {
                    e.ConfirmDate = moment(e.ConfirmDate).format('MM/DD/YYYY');
                };

                newData.push({
                    "Sipariş Tarihi": e.CreateDate,
                    "Onay Tarihi": e.ConfirmDate,
                    "Durum": e.StatusStr,
                    "Genel Toplam": e.GeneralTotal.toFixed(2) + ' TL',
                    "Siparşi Notu": e.Notes,
                    "Gönderen": e.SenderName
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                         "Sipariş Tarihi",
                         "Onay Tarihi",
                         "Durum",
                         "Genel Toplam",
                         "Siparşi Notu",
                         "Gönderen"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Siparis_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Siparis_Listesi.xlsx");
        }
    };

     $(document).ready(function () {
        $scope.getOrderHeaderList();
    });

}).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});
