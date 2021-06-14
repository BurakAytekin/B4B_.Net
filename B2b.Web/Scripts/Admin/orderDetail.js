adminApp.controller("OrderDetailController", [
    '$scope', 'NgTableParams', '$http', '$filter', '$sce', function ($scope, NgTableParams, $http, $filter, $sce) {
        var mOrderDetailRowEdit = "#mOrderDetailRowEdit";
        var mOrderDetailInformation = "#mOrderDetailInformation";
        $scope.globalItem = {};

        $scope.showPdf = function () {
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
        }

        $scope.trustDangerousSnippet = function (snippet) {
            return $sce.trustAsHtml(snippet);
        };

        $scope.getURLParameter = function () {
            var sPageURL = window.location.href;
            var indexOfLastSlash = sPageURL.lastIndexOf("/");

            if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
                return sPageURL.substring(indexOfLastSlash + 1);
            else
                return 0;
        };

        $(window).load(function () {
            $scope.loadHeader();
            $scope.loadOrderDetail($scope.getURLParameter());
        });
        $scope.loadPaymentDetail=function ()
        {
            $http({
                method: "POST",
                url: "/Orders/GetListOrderPayment",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { orderId: $scope.getURLParameter() }
            }).then(function (response) {
                $scope.OrderPaymentTypeList = response.data;

            });
            
        }

        $scope.openImage=function(image){
            $scope.openImage = image;
            $('#ImageModal').modal('show');

        }
        $scope.loadHeader = function () {
         
            $http({
                method: "POST",
                url: "/Orders/GetOrderHeader",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: $scope.getURLParameter() }
            }).then(function (response) {
                $scope.selectedOrder = response.data;
                $scope.loadPaymentDetail();

            });
        }
        $scope.confirmOrder = function (order, status) {
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


    //$scope.showModalCustomDeleteOrder = function (row, statu) {
    //    $scope.selectedOrder.Row = angular.copy(row);
    //    $scope.selectedOrder.Statu = statu;
    //    $scope.note = "";
    //    $('#modal-custom-delete').modal('show');
    //};

        $scope.showModalDeleteAndReason = function (row, statu) {
            $scope.globalItem.DeleteNote = "";
            $scope.globalItem.Row = row;
            $scope.globalItem.Statu = statu;
            $scope.globalItem.FunctionName = $scope.updateOrderStatu;
            $('#modal-delete-reason').modal('show');
        };

    $scope.showModalConfirmation = function (row, statu) {
        $scope.selectedOrder.FunctionName = $scope.updateOrderStatu;
        $scope.selectedOrder.Message = 'Veri durumu güncellenecektir emin misiniz ?';
        $scope.selectedOrder.Row = row;
        $scope.selectedOrder.Statu = statu;
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
        console.log("girdi");
      
        $scope.selectedOrder.row = angular.copy($scope.selectedOrder);
                        $http({
                            method: "POST",
                            url: "/Orders/UpdateOrderStatu",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: {
                                order: $scope.selectedOrder.row,
                                statu: $scope.selectedOrder.Statu,
                                note: $scope.selectedOrder.note
                            }
                        }).then(function (response) {
                            $('#modal-global-confirmation').modal('hide');
                            $('#modal-delete-reason').modal('hide');
                            $scope.note = '';

                            fireCustomLoading(false);
                            iziToast.show({
                                message: response.data.Message,
                                position: 'topCenter',
                                color: response.data.Color,
                                icon: response.data.Icon
                            });
                            $scope.loadHeader();
                            $scope.loadOrderDetail($scope.getURLParameter());
                            //$scope.OrderSearch($scope.orderSearchCriteria);
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



        $scope.deleteOrReturnOrder = function (orderId, status) {
        console.log(orderId);

            $.confirm({
                title: 'Uyarı!',
                content: "Veri " + (status == 2 ? "silinecektir " : " durumu güncellenecektir ") + "emin misiniz?",
                buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () {
                            fireCustomLoading(true);
                            $http({
                                method: "POST",
                                url: "/Orders/DeleteOrReturnOrder",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { order: orderId, status: status }
                            }).then(function (response) {
                                fireCustomLoading(false);
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                                $scope.loadHeader();
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


        $scope.redirectUafForm = function () {
            window.open("/ProductResourceForm/Index/" + $scope.getURLParameter(), '_blank');
        };

        $scope.loadOrderDetail = function (id) {
            $http({
                method: "POST",
                url: "/Orders/GetListOrderDetailHeader",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: id }
            }).then(function (response) {
               
                $scope.orderHeader = response.data;

                $http({
                    method: "POST",
                    url: "/Orders/GetListOrderDetail",
                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                    data: { id: id }
                }).then(function (response) {
                    $scope.orderDetailList = response.data;
                    $scope.tableOrderDetailParams = new NgTableParams({
                        count: $scope.orderDetailList.length === 0 ? 1 : $scope.orderDetailList.length // hides pager
                    },
                        {
                            filterDelay: 0,
                            counts: [],
                            dataset: angular.copy( $scope.orderDetailList)
                        });

                    $scope.originalDataDetail = angular.copy($scope.orderDetailList);
                    $scope.cancelDetail = $scope.cancelDetail;
                    $scope.deleteDetail = $scope.deleteDetail;
                    $scope.saveDetail = $scope.saveDetail;
                    $scope.addDetail = $scope.addDetail;
                    $scope.cancelChangesDetail = $scope.cancelChangesDetail;
                    $scope.hasChangesDetail = $scope.hasChangesDetail;

                    $scope.getCustomerBalanceInfo();
                    fireCustomLoading(false);
                });
            });
        };

        $scope.showExplanation = function (row) {
            $scope._itemExplanation = row.ItemExplanation;
            $(mOrderDetailInformation).appendTo("body").modal('show');
        };

        $scope.getCustomerBalanceInfo = function () {

            $http({
                method: "POST",
                url: "/Orders/GetCustomerBalanceInfo",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { customerCode: $scope.orderHeader.Customer.Code }
            }).then(function (response) {
                $scope.customerBalanceInfo = response.data;
            });

        };

        // #region OrderDetailRowEdit Code
        $scope.hasChangesDetail = function (detailRow) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Products/GetCurrencyTypeList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}
            }).then(function (response) {
                fireCustomLoading(false);
                $scope.currencyList = response.data;
                $(mOrderDetailRowEdit).appendTo("body").modal('show');
                $scope.detailChangeItem = detailRow;
                $scope.detailChangeItemOrijinal = detailRow;
            });
        };


        $scope.getCurrencyList = function () {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Products/GetCurrencyTypeList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}
            }).then(function (response) {
                fireCustomLoading(false);
                $scope.currencyList = response.data;
            });
        };

        $scope.updateOrderDetail = function (orderRow) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Orders/UpdateOrderDetail",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { orderRow: orderRow, orderOrijinalRow: $scope.detailChangeItemOrijinal }
            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $scope.loadOrderDetail($scope.getURLParameter());
            });
        };


        $scope.deleteDetail = function (orderRow) {
            $.confirm({
                title: 'Uyarı!',
                content: "Veri silinecektir emin misiniz?",
                buttons:
                    {
                        Ok: {
                            text: "Evet",
                            btnClass: 'btn-blue',
                            action: function () {

                                fireCustomLoading(true);
                                $http({
                                    method: "POST",
                                    url: "/Orders/DeleteOrderDetail",
                                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                                    data: { detailId: orderRow.Id, orderRow: orderRow }
                                }).then(function (response) {
                                    fireCustomLoading(false);
                                    iziToast.show({
                                        message: response.data.Message,
                                        position: 'topCenter',
                                        color: response.data.Color,
                                        icon: response.data.Icon
                                    });
                                    $scope.loadOrderDetail($scope.getURLParameter());
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


        $scope.exportToExcel = function () {
            //ersoy
            if ($scope.orderDetailList > []) {
                var newData = [];
                $scope.orderDetailList.forEach(function (e) {

                    //if (e.CreateDate == null || e.CreateDate == '' || e.CreateDate == undefined) {
                    //    e.CreateDate = '';
                    //} else {
                    //    e.CreateDate = moment(e.CreateDate).format('MM/DD/YYYY');
                    //}

                    //if (e.ConfirmDate == null || e.ConfirmDate == '' || e.ConfirmDate == undefined) {
                    //    e.ConfirmDate = '';
                    //} else {
                    //    e.ConfirmDate = moment(e.ConfirmDate).format('MM/DD/YYYY');
                    //}


                    newData.push({
                        "Ürün Kodu": e.ProductCode,
                        "Ürün Adı": e.ProductName,
                        "Üretici": e.Manufacturer,
                        //"Miktar": e.Total.toFixed(2) + ' TL',
                        "Miktar": e.Quantity,
                        "Birim": e.Unit,
                        "Fiyat": e.Price.toFixed(2) + ' TL',
                        "Isk1": e.Disc1,
                        "Isk2": e.Disc2,
                        "Isk3": e.Disc3,
                        "Isk4": e.Disc4,
                        "Cmp Isk.": e.DiscCampaign,
                        "Cep Isk.": e.DiscSpecial,
                        "Kupon Isk.": e.DiscCoupon,
                        "Tutar": e.Amount.toFixed(2) + ' TL',
                        "Net Fiyat": e.NetPrice.toFixed(2) + ' TL',
                        "Net Tutar": e.NetAmount.toFixed(2) + ' TL',
                        "Kdv": e.VatAmount.toFixed(2) + ' TL',
                        "Net Tutar ( Kdv )": e.NetAmountWithVat + ' TL'

                    });
                });

                var ws = XLSX.utils.json_to_sheet(newData, {
                    header: [
                             "Ürün Kodu",
                             "Ürün Adı",
                             "Üretici",
                             "Miktar",
                             "Birim",
                             "Fiyat",
                             "Isk1",
                             "Isk2",
                             "Isk3",
                             "Isk4",
                             "Cmp Isk.",
                             "Cep Isk.",
                             "Kupon Isk.",
                             "Tutar",
                             "Net Fiyat",
                             "Net Tutar",
                             "Kdv",
                             "Net Tutar ( Kdv )"
                    ]
                });

                //Set Columns Size !
                var wscols = [{ wch: 20 }, { wch: 60 }, { wch: 15 }, { wch: 5 }, { wch: 5 }, { wch: 10 }, { wch: 5 }, { wch: 5 }, { wch: 5 }, { wch: 5 }, { wch: 10 }, { wch: 10 }, { wch: 10 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
                ws['!cols'] = wscols;

                var wb = XLSX.utils.book_new();
                XLSX.utils.book_append_sheet(wb, ws, "Siparis_Detay_Listesi");


                XLSX.utils.shee;
                var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
                saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Siparis_Detay_Listesi.xlsx");
            }
        };


        // #endregion
    }
]).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(item.replace('T', ' '));
        }
        return "";
    };
});