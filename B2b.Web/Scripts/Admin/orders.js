adminApp.controller("OrderController", ['$scope', 'NgTableParams', '$http', '$filter', function ($scope, NgTableParams, $http, $filter) {

    $scope.deleteItem = {};
    $scope.confirmItem = {};
    $scope.globalItem = {};

    $scope.orderSearchCriteria =
    {
        T9Text: '',
        OrderStatu: 0,
        StartDate: '',
        EndDate: ''
    };

    $scope.showPdf = function (order) {

        $http({
            method: "POST",
            url: "/Orders/GetOrderHeader",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: order.Id }
        }).then(function (response) {
            $scope.selectedOrder = response.data;

            $http({
                method: "POST",
                url: "/Orders/GetListOrderDetail",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: order.Id }
            }).then(function (response) {
                $scope.orderDetailList = response.data;

                fireCustomLoading(true);
                $http({
                    method: "POST",
                    url: "/Admin/Orders/SavePdf",
                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                    data: { header: $scope.selectedOrder, detail: $scope.orderDetailList }//, 
                }).then(function (response) {
                    $scope.frameUrl = response.data;

                    $('#mPdfShow').appendTo("body").modal('show');

                    fireCustomLoading(false);

                });

            });

        });
    }

    $scope.confirmOrder=function(order, status) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Orders/ChangeOrderStatus",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { order: order, status: status }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.OrderSearch($scope.orderSearchCriteria);
        });
    }

    $scope.showSystemNotes = function (item) {
        $scope.selectedOrder = item;
        $('#modal-systemNotes').modal('show');
       
    };

    $scope.OrderSearch = function (orderSearchCriteria) {
        $scope.orderSearchCriteria.StartDate = $('#iOrderStartDate').val();
        $scope.orderSearchCriteria.EndDate = $('#iOrderEndDate').val();
        
        //var detailbuttons = '';
        //detailbuttons = ' <div class="btn-group btn-group-xs mb-0 kendo-options-btn">';
        //detailbuttons += '         <button type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="true">';
        //detailbuttons += '                                                    <i class="fa fa-gears"></i> <span class="caret"></span>';
        //detailbuttons += '         </button>';
        //detailbuttons += '         <ul role="menu" class="dropdown-menu pull-right with-arrow animated littleFadeInUp">';
        //detailbuttons += '              <li ng-if="dataItem.Status !=2"  ng-click="deleteOrReturnOrder(dataItem.Id,2)"><a href="javascript:;"><i class="fa fa-trash-o"></i> Sil</a></li>';
        //detailbuttons += '               <li ng-if="dataItem.Status ==2"  ng-click="deleteOrReturnOrder(dataItem.Id,0)"><a href="javascript:;"><i class="fa fa-trash-o"></i> Geri Al</a></li>';
        //detailbuttons += '         </ul>';
        //detailbuttons += ' </div><div class="kendo-details-btn"><a href="javascript:;" ng-click="tableOrderDetail(dataItem)" class=\'btn btn-tiny btn-info btn-order-detail\' data-id=\'dataItem.Id\'>Detay</a></div>';
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Orders/GetListOrderHeader",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { orderSearchCriteria: orderSearchCriteria }
        }).then(function (response) {

            $scope.orderList = response.data;
            $scope.tableOrdersParams = new NgTableParams({
                count: $scope.orderList.length === 0 ? 1 : $scope.orderList.length // hides pager
            },
                {
                    filterDelay: 0,
                    counts: [],
                    dataset: angular.copy(response.data)
                });

            $scope.tableORdersOrjinalData = angular.copy(response.data);
            $scope.tableOrderdelete = $scope.tableKitdelete;


            // #region Kendo

            //    $scope.gridOptions = {
            //        dataSource:{
            //        data: $scope.orderList
            //        ,
            //        schema: {
            //            model: {
            //                fields: {
            //                    "Id": { type: "number" },
            //                    "Customer_Code": { type: "string", from: "Customer.Code" },
            //                    "Customer_Name": { type: "string", from: "Customer.Name" },
            //                    "ShipmentName": { type: "string" },
            //                    "CreateDate": { type: "date" },
            //                    "ConfirmDate": { type: "date" },
            //                    "TotalStr": { type: "string" },
            //                    "VatStr": { type: "string" },
            //                    "DiscountStr": { type: "string" },
            //                    "SenderName": { type: "string" }
            //                }
            //            }
            //        },
            //        pageSize: 30
            //    },

            //    height: 540,
            //    sortable: true,
            //    reorderable: true,
            //    groupable: true,
            //    resizable: true,
            //    filterable: true,
            //    columnMenu: true,
            //    pageable: true,
            //    toolbar: ["excel", "pdf"], //Çıktı işlemlerine eklenecekler bunun içine yazılıyor
            //    excel: {
            //        fileName: "Siparişler.xlsx"
            //    },
            //    pdf: {
            //        fileName: "Siparişler.pdf"
            //    },
            //    columns: [
            //    {
            //        field: "Id",
            //        title: "No",
            //        locked: false, //bu true olduğu zaman gridin en başına gelir ve sabitlenir
            //        lockable: false, //grid üzerinde kilitle seçeneği kapatır
            //        width: 80
            //    }, {
            //        field: "Customer_Code",
            //        title: "Kod",
            //        width: 150
            //    }, {
            //        field: "Customer_Name",
            //        title: "Adı",
            //        width: 450
            //    }, {
            //        field: "ShipmentName",
            //        title: "Gönderi Şekli",
            //        //   locked: true,
            //        width: 150
            //    }, {
            //        field: "CreateDate",
            //        title: "Sipariş T.",
            //        // lockable: false,
            //        width: 150,
            //        format: "{0:dd/MM/yyyy}"
            //    }, {
            //        field: "ConfirmDate",
            //        title: "Onay T.",
            //        width: 150,
            //        format: "{0:dd/MM/yyyy}"
            //    }, {
            //        field: "TotalStr",
            //        title: "Toplam",
            //        width: 150,
            //        encoded: false
            //    },
            //    {
            //        field: "VatStr",
            //        title: "Kdv",
            //        width: 150,
            //        encoded: false
            //    },
            //    {
            //        field: "DiscountStr",
            //        title: "İskonto",
            //        width: 150,
            //        encoded: false
            //    },
            //    {
            //        field: "SenderName",
            //        title: "Gönderen",
            //        width: 100
            //    },
            //    {
            //        title: "İşlemler",
            //        width: 120,
            //        template: function () {
            //            return detailbuttons;
            //        }
            //    }

            //    ]
            //}

            // #endregion
            fireCustomLoading(false);
        });

     

    };


    $scope.ConcatOrders = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Havuzda bekleyen tüm siparişleriniz birleştirilecektir. Devam etmek istediğinize emin misiniz ?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Orders/ConcatOrders",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: {}
                        }).then(function (response) {
                            fireCustomLoading(false);
                            iziToast.show({
                                message: response.data.Message,
                                position: 'topCenter',
                                color: response.data.Color,
                                icon: response.data.Icon
                            });
                            $scope.OrderSearch($scope.orderSearchCriteria);
                        });
                    }
                },
                Cancel: {
                    text: "Hayır",
                    btnClass: 'btn-red any-other-class',
                    action: function () {
                        iziToast.error({
                            message: 'işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });
                    }
                }
            }
        });

    };

    $scope.showModalCustomDeleteOrder = function (row, statu) {
        $scope.globalItem.Row = row;
        $scope.globalItem.Statu = statu;
        $scope.note = "";
        $('#modal-custom-delete').modal('show');
    };

    $scope.showModalConfirmation = function (row, statu) {
        $scope.globalItem.FunctionName = $scope.updateOrderStatu;
        $scope.globalItem.Message = 'Veri durumu güncellenecektir emin misiniz ?';
        $scope.globalItem.Row = row;
        $scope.globalItem.Statu = statu;
        $('#modal-global-confirmation').modal('show');
    };

    $scope.updateOrderStatu = function (/*order, status*/) {
        //$.confirm({
        //    title: 'Uyarı!',
        //    content: "Veri " + (status == 2 ? "silinecektir " : " durumu güncellenecektir ") + "emin misiniz?",
        //    buttons:
        //    {
        //        Ok: {DeleteOrReturnOrder
        //            text: "Evet",
        //            btnClass: 'btn-blue',
        //            action: function () {
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Orders/UpdateOrderStatu",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: {
                                order: $scope.globalItem.Row,
                                statu: $scope.globalItem.Statu,
                                note: $scope.note
                            }
                        }).then(function (response) {
                            $('#modal-global-confirmation').modal('hide');
                            $('#modal-custom-delete').modal('hide');
                            $scope.note = '';

                            fireCustomLoading(false);
                            iziToast.show({
                                message: response.data.Message,
                                position: 'topCenter',
                                color: response.data.Color,
                                icon: response.data.Icon
                            });
                            $scope.OrderSearch($scope.orderSearchCriteria);
                        });
                //    }
                //},
                //Cancel: {
                //    text: "Hayır",
                //    btnClass: 'btn-red any-other-class',
                //    action: function () {
                //        iziToast.error({
                //            message: 'işleminiz iptal edilmiştir.',
                //            position: 'topCenter'
                //        });
                //    }
                //}
            //}
        //});
    };

    $scope.ConvertDate = function (input) {
        if (input)
            return $filter('date')(new Date(input), 'dd/MM/yyyy');
        else
            return "-";
    };

    $scope.tableOrderDetail = function (row) {
        window.open("/Admin/Orders/OrderDetail/" + row.Id, '_blank');
    };

    $scope.keypressEventOrderSearch = function (e, orderSearchCriteria) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 46)
            e.returnValue = false;

        if (key === 13) {
            $scope.OrderSearch(orderSearchCriteria);
        }
    };

    function setDefaultDate() {
        var today = new Date();
        var dStart = (today.getDate() - 1) + '/' + (today.getMonth() + 1) + '/' + today.getFullYear();
        $('#iOrderStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            //defaultDate: dStart,
            locale: "tr"
        }).val(dStart);

        $('#iOrderEndDate').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    };

    $scope.clear = function () {
        $scope.orderSearchCriteria = {};
        $scope.orderSearchCriteria =
            {
                T9Text: '',
                OrderStatu: 0,
                StartDate: '',
                EndDate: ''
            };
        setDefaultDate();
    };

    $(window).load(function () {
     
    });


    $(document).ready(function () {
        setDefaultDate();
    });

    $scope.exportToExcel = function () {
        if ($scope.orderList.length > 0) {
            var newData = [];
            $scope.orderList.forEach(function (e) {

                //Örnek tarih formatı    
                if (e.CreateDate == null || e.CreateDate == '' || e.CreateDate == undefined) {
                    e.CreateDate = '';
                } else {
                    e.CreateDate = moment(e.CreateDate).format('DD/MM/YYYY');
                };

                if (e.ConfirmDate == null || e.ConfirmDate == '' || e.ConfirmDate == undefined) {
                    e.ConfirmDate = '';
                } else {
                    e.ConfirmDate = moment(e.ConfirmDate).format('DD/MM/YYYY');
                };


                newData.push({
                    "No": e.Id,
                    "Kod": e.Customer.Code,
                    "Adı": e.Customer.Name,
                    "Gönderi Şekli": e.ShipmentName,
                    "Sipariş Tarihi": e.CreateDate,
                    "Onay Tarihi": e.ConfirmDate,
                    "Yazdırıldı": e.PrintStatu == true ? 'Evet' : 'Hayır',
                    "Ödeme Tipi": e.PaymentType,
                    "Toplam": e.Total.toFixed(2) + ' TL',
                    "Kdv": e.Vat.toFixed(2) + ' TL',
                    "İskonto": e.Discount.toFixed(2) + ' TL',
                    "Gönderen": e.SenderName
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                         "No",
                         "Kod",
                         "Adı",
                         "Gönderi Şekli",
                         "Sipariş Tarihi",
                         "Onay Tarihi",
                         "Yazdırıldı",
                         "Ödeme Tipi",
                         "Toplam",
                         "Kdv",
                         "İskonto",
                         "Gönderen"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 5 }, { wch: 15 }, { wch: 60 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Siparis_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Siparis_Listesi.xlsx");
        }
    };


}]);
