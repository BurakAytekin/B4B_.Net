b2bApp.controller('b2bStaticReportController', ['$scope', '$http', '$sce', function ($scope, $http, $sce) {
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

    $(document).ready(function () {
        $scope.getOrderHeaderList();
    });

    $scope.getOrderDetail = function (item) {
        $http({
            method: "POST",
            url: "/B2bReport/GetOrderDetailList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { orderId: item.Id }

        }).then(function (response) {
            if (response.data.length === 0) {
                iziToast.warning({

                    message: 'Belirtilen Kriterlerde Sipariş Bulunamadı',
                    position: 'topCenter'
                });
            }
            else {
                $scope.orderDetailList = response.data;
                $('#modal-detail').modal('show');
            }
        });
    };

    $scope.trustDangerousSnippet = function (snippet) {
        return $sce.trustAsHtml(snippet);
    };

    $scope.getOrderHeaderList = function () {
        var dateStart = $('#dateStart').val();
        var datetEnd = $('#dateEnd').val();
        var searchText = $('#txtGeneralSearch').val();
        $http({
            method: "POST",
            url: "/B2bReport/GetOrderHeaderList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { dateStart: dateStart, datetEnd: datetEnd, searchText: searchText }

        }).then(function (response) {
            if (response.data.length === 0) {
                iziToast.warning({

                    message: 'Belirtilen Kriterlerde Sipariş Bulunamadı',
                    position: 'topCenter'
                });
            }
            else {
                $scope.orderHeaderList = response.data;
            }

        });

    };


}]).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});