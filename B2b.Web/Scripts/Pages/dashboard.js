b2bApp.controller('dashboardController', function ($scope, $http, B2BServices, $filter) {

    $scope.paymentTotal = '0 ₺';
    $scope.balanceInfo = { UnClosedBalanceStr: '0 ₺', BalanceStr: '0 ₺' };

    $scope.responseLocation = function (location) {
        window.location = location;
    };

    $scope.fireDashboardLoading = function (cntrl, item) {

        if (cntrl === undefined || cntrl === null || cntrl === "")
            cntrl = false;

        if (cntrl) {
            if (!$('.' + item).hasClass("search-loading"))
                $('.' + item).addClass("search-loading");
        }
        else {
            if ($('.' + item).hasClass("search-loading"))
                $('.' + item).removeClass("search-loading");
        }
    };

    $scope.responseOrderDetail = function (orderId) {
        $http({
            method: "POST",
            url: "/Order/ResponseOrderDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { orderId: orderId }

        }).then(function (response) {
            window.location = "Order/OrderDetail";
        });
    };

    $scope.getOrderList = function () {
        $scope.fireDashboardLoading(true, 'loading3');
        $http({
            method: "POST",
            url: "/Dashboard/GetOrderList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.orderHeaderList = response.data;
            $scope.fireDashboardLoading(false, 'loading3');
            $scope.getPaymentTotal();

        });
    };

    $scope.getInvoiceList = function () {
        $scope.fireDashboardLoading(true, 'loading4');
        $http({
            method: "POST",
            url: "/Dashboard/GetInvoiceList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.financeList = response.data;
            $scope.fireDashboardLoading(false, 'loading4');
        });
    };

    $scope.getPaymentTotal = function () {
        $scope.fireDashboardLoading(true, 'loading2');
        $http({
            method: "POST",
            url: "/Dashboard/GetPaymentTotal",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.paymentTotal = response.data;
            $scope.fireDashboardLoading(false, 'loading2');

        });
    };

    $scope.getBalanceInformation = function () {
        $scope.fireDashboardLoading(true, 'loading1');
        $http({
            method: "POST",
            url: "/Dashboard/GetBalanceInformation",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.balanceInfo = response.data;
            $scope.fireDashboardLoading(false, 'loading1');
            $scope.getInvoiceList();
        });
    };


    $scope.keyPressedBasket = function (e, id, type) {
        var key, brwsr, keyCtrl = false, keyShift = false, keyAlt = false, keyMeta = false;

        if (window.event) { // Internet Explorer
            if (window.event.shiftKey) keyShift = true;
            if (window.event.ctrlKey) keyCtrl = true;
            if (window.event.metaKey) keyMeta = true;
            if (window.event.altKey) keyAlt = true;
            key = window.event.keyCode;
            brwsr = true;
        }
        else if (e) { // Firefox
            if (e.shiftKey) keyShift = true;
            if (e.ctrlKey) keyCtrl = true;
            if (e.metaKey) keyMeta = true;
            if (e.altKey) keyAlt = true;
            key = e.which;
            brwsr = false;
        }
        else { // Other Browsers
            if (event.shiftKey) keyShift = true;
            if (event.ctrlKey) keyCtrl = true;
            if (event.metaKey) keyMeta = true;
            if (event.altKey) keyAlt = true;
            key = event.keyCode;
            brwsr = false;
        }

        function fireReturnFalse(e) {
            if (e.preventDefault) {
                e.preventDefault();
                e.stopPropagation();
            }
            else
                e.returnValue = false;

            if (brwsr === true) {
                window.event.returnValue = false;
                event.keyCode = 0;
            }
            else {
                e.cancelbubble = true;
                e.returnvalue = false;
                e.keycode = false;
            }

            return false;
        }

        function fireReturnTrue() {
            return true;
        }

        if (keyShift || keyCtrl || keyAlt || keyMeta || (key >= 112 && key <= 123)) {
            fireReturnFalse(e);
        }
        else if (key === 13) {
            $scope.askAvailable(id, type);
            fireReturnFalse(e);
        }
        else if (key === 8 || key === 9 || key === 37 || key === 39) {
            fireReturnTrue();
        }
        else if (key < 48 || key > 57) {
            fireReturnFalse(e);
        }
        else {
            fireReturnTrue();
        }
    };

    $scope.askAvailable = function (id, type) {
        $http({
            method: "POST",
            url: "/Search/GetAvailable",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }

        }).then(function (response) {
            if (response.data !== "[]") {
                var qnt = response.data;
                if (qnt !== "")
                    $scope.askForAdd(id, qnt, type);
                else
                    $scope.addBasket(id, type);
            } else {
                $scope.addBasket(id, type);
            }
        });
    };

    $scope.askForAdd = function (id, qnt, type) {
        $.confirm({
            title: 'Uyarı!',
            content: "Bu üründen sepetinizde " + qnt + " adet bulunmaktadır. Sepetinize eklemek istiyor musunuz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-success',
                    keys: ['enter'],
                    action: function () { $scope.addBasket(id, type); }

                },
                Cancel: {
                    text: "Hayır",
                    btnClass: 'btn-danger',
                    keys: ['esc'],
                    action: function () {
                        if (type === 0)
                            $('#qty' + id).val('');
                        if (type === 1)
                            $('#qtyP' + id).val('');
                        if (type === 2)
                            $('#qtyD' + id).val('');
                        if (type === 3)
                            $('#qtyM').val('');

                        iziToast.error({
                            message: 'Ekleme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }

            }
        });
    };

    $scope.addBasket = function (id, type) {
        var qtyStr;
        if (type === 0) {
            qtyStr = $('#qty' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qty' + id).attr('placeholder');
        }
        else if (type === 1) {
            qtyStr = $('#qtyP' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qtyP' + id).attr('placeholder');
        }
        else if (type === 2) {
            qtyStr = $('#qtyD' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qtyD' + id).attr('placeholder');
        }
        else {
            qtyStr = $('#qtyM').val();
            if (qtyStr === '')
                qtyStr = $('#qtyM' + id).attr('placeholder');
        }
        if ($.isNumeric(qtyStr)) {
            $http({
                method: "POST",
                url: "/Search/AddBasket",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: id, qty: qtyStr }

            }).then(function (response) {
                var retVal = jQuery.parseJSON(response.data);
                if (retVal.statu === "success") {
                    iziToast.success({
                        message: retVal.message,
                        position: 'topCenter'
                    });

                    B2BServices.getBasketCount();

                    if (retVal.cmpAvailableQuantity > 0)
                        $scope.showCampaignMessage(id, retVal.cmpAvailableQuantity);
                }
                else {
                    iziToast.error({
                        //title: 'Hata',
                        message: retVal.message,
                        position: 'topCenter'
                    });

                }
                $('#qty' + id).val('');
                $('#qtyP' + id).val('');
                $('#qtyD' + id).val('');
            });
        }
    };

    $scope.showCampaignMessage = function (id, qty) {

        iziToast.show({
            color: 'dark',
            icon: 'fa fa-gift',
            timeout: 20000,
            title: 'Dikkat !!',
            message: 'Bu üründen ' + qty + ' adet daha alımınızda kampanyadan faydalanacaksınız. Almak ister misiniz ? ',
            position: 'center', // bottomRight, bottomLeft, topRight, topLeft, topCenter, bottomCenter
            progressBarColor: 'rgb(220, 20, 60)',
            buttons: [
                ['<button>Ekle</button>', function (instance, toast) {

                    $http({
                        method: "POST",
                        url: "/Search/AddBasket",
                        headers: { "Content-Type": "Application/json;charset=utf-8" },
                        data: { id: id, qty: qty }

                    }).then(function (response) {
                        var retVal = jQuery.parseJSON(response.data);
                        if (retVal.statu === "success") {
                            iziToast.success({
                                message: retVal.message,
                                position: 'topCenter'
                            });

                            B2BServices.getBasketCount();
                            instance.hide({
                                transitionOut: 'fadeOutUp',
                                onClosing: function (instance, toast, closedBy) {
                                    console.info('closedBy: ' + closedBy); // The return will be: 'closedBy: buttonName'
                                }
                            }, toast, 'buttonName');

                        }
                        else {
                            iziToast.error({
                                //title: 'Hata',
                                message: retVal.message,
                                position: 'topCenter'
                            });
                        }
                    });


                }, true], // true to focus
                ['<button>Kapat</button>', function (instance, toast) {
                    instance.hide({
                        transitionOut: 'fadeOutUp',
                        onClosing: function (instance, toast, closedBy) {
                            console.info('closedBy: ' + closedBy); // The return will be: 'closedBy: buttonName'
                        }
                    }, toast, 'buttonName');
                }]
            ]
        });
    };

    $(document).ready(function () {
        $scope.getOrderList();

    });

});