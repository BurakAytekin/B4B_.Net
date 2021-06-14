adminApp.controller('homeController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {
    $scope.salesmanNoteList = null;

    $scope.keypressEventSalesmanNote = function (e) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 13) {
            $scope.addSalesmanNote();
        }
    };

    $scope.addSalesmanNote = function () {
        fireCustomTileLoading('#divSalesmanNote', true);
        $http({
            method: "POST",
            url: "/Admin/Home/AddSalesmanNote",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { salesmanNote: $scope.salesmanNote }
        }).then(function (response) {
            $scope.salesmanNote = '';
            $scope.loadSalesmanNote();
            fireCustomTileLoading('#divSalesmanNote', false);
        });
    };

    $scope.updateSalesmanNote = function (item, deleted) {
        fireCustomTileLoading('#divSalesmanNote', true);
        item.IsActive = angular.copy(!item.IsActive);
        item.Deleted = deleted;
        $http({
            method: "POST",
            url: "/Admin/Home/UpdateSalesmanNote",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { item: item }
        }).then(function (response) {
            $scope.loadSalesmanNote();
            fireCustomTileLoading('#divSalesmanNote', false);
        });
    };

    $scope.loadSalesmanNote = function () {
        fireCustomTileLoading('#divSalesmanNote', true);
        $http({
            method: "POST",
            url: "/Admin/Home/GetSalesmanNote",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.salesmanNoteList = response.data;

            fireCustomTileLoading('#divSalesmanNote', false);
        });
    };

    $scope.loadManufacturerReport = function (type) {
        fireCustomTileLoading('#divManufacturer', true);
        $http({
            method: "POST",
            url: "/Admin/Home/GetManufacturerReport",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: type }

        }).then(function (response) {
            $scope.manufacturerReportList = response.data;

            var colorData = ["#00a3d8", "#d9544f", "#ffc100", "#1693A5", "#16a085", "#a2d200"];

            var data = [];
            var i = 0;
            angular.forEach($scope.manufacturerReportList, function (value) {
                var item = { label: value.Manufacturer, value: value.Total, color: colorData[i] };
                data.push(item);
                i++;
            });

            $('#manufacturer-usage').empty();

            Morris.Donut({
                element: 'manufacturer-usage',
                data: data,
                resize: true
            });

            fireCustomTileLoading('#divManufacturer', false);
        });
    };

    $scope.loadHeaderInformation = function (type) {
        //fireCustomTileLoading('.card-container', true);
        $http({
            method: "POST",
            url: "/Admin/Home/GetHeaderInformation",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.headerInformation = response.data;
            //fireCustomTileLoading('.card-container', false);
        });
    };

    $scope.loadOrderPaymentCross = function () {
        fireCustomTileLoading('#divOrderPayment', true);
        $http({
            method: "POST",
            url: "/Admin/Home/GetOrderPaymentCross",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.orderPaymentCross = response.data;
            var listOrder = [];
            var listPayment = [];

            $.each($scope.orderPaymentCross, function (i, item) {
                if (item.Type === 0) {
                    var itemValue = [item.Month, item.Total];
                    listOrder.push(itemValue);
                }
                else {
                    var itemValue = [item.Month, item.Total];
                    listPayment.push(itemValue);
                }
            });

            var data = [{
                data: listPayment,
                label: 'Ödemeler',
                points: {
                    show: true,
                    radius: 4
                },
                splines: {
                    show: true,
                    tension: 0.45,
                    lineWidth: 4,
                    fill: 0
                }
            },
            {
                data: listOrder,
                label: 'Siparişler',
                bars: {
                    show: true,
                    barWidth: 0.6,
                    lineWidth: 0,
                    fillColor: { colors: [{ opacity: 0.3 }, { opacity: 0.8 }] }
                }
            }];

            var options = {
                colors: ['#e05d6f', '#61c8b8'],
                series: {
                    shadowSize: 0
                },
                legend: {
                    backgroundOpacity: 0,
                    margin: -7,
                    position: 'ne',
                    noColumns: 2
                },
                xaxis: {
                    tickLength: 0,
                    font: {
                        color: '#ffffff'
                    },
                    position: 'bottom',
                    ticks: [
                        [1, 'Oca'], [2, 'Şub'], [3, 'Mar'], [4, 'Nis'], [5, 'May'], [6, 'Haz'], [7, 'Tem'], [8, 'Ağu'], [9, 'Eyl'], [10, 'Eki'], [11, 'Kas'], [12, 'Ara']
                    ]
                },
                yaxis: {
                    tickLength: 0,
                    font: {
                        color: '#fff'
                    }
                },
                grid: {
                    borderWidth: {
                        top: 0,
                        right: 0,
                        bottom: 1,
                        left: 1
                    },
                    borderColor: 'rgba(255,255,255,.3)',
                    margin: 0,
                    minBorderMargin: 0,
                    labelMargin: 20,
                    hoverable: true,
                    clickable: true,
                    mouseActiveRadius: 6
                },
                tooltip: true,
                tooltipOpts: {
                    content: '%s: %y',
                    defaultTheme: false,
                    shifts: {
                        x: 0,
                        y: 20
                    }
                }
            };

            var plot = $.plot($("#statistics-chart"), data, options);

            $(window).resize(function () {
                // redraw the graph in the correctly sized div
                plot.resize();
                plot.setupGrid();
                plot.draw();
            });

            fireCustomTileLoading('#divOrderPayment', false);
        });
    };

    $(document).ready(function () {
        $scope.loadManufacturerReport(0);
        $scope.loadHeaderInformation();
        $scope.loadOrderPaymentCross();
        $scope.loadSalesmanNote();
    });

}]);