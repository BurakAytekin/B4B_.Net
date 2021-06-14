b2bApp.controller('indexController', function ($scope, $http, $compile, B2BServices) {
    // #region Veriables
    $scope.basketNotId = -1;
    $scope.lastItem = 0;
    $scope.selectedCopunCode = '';
    $scope.manuelCopunUse = false;
    $scope.basketAllCheck = false;
    $scope.OrderPaymentTypeList = [];
    $scope.SelectedShipmentDifferent = '0';

    // #endregion
    // #region MinOrder Controls

    $scope.openColorInformation = function () {
        $('#modal-color-information').modal('show');
    };

    $scope.showCustomerCoupon = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Account/GetCouponList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: 0 }
        }).then(function (response) {
            $scope.couponList = response.data;
            $('#modal-coupon').modal('show');
            fireCustomLoading(false);
        });
    };

    $scope.showInformationModal = function (row) {
        $scope.informationList = {};

        var array = "[";
        for (var i = 0; i < row.Product.SystemNotes.length; i++) {
            array += "{";
            array += "\"Note\":\"" + row.Product.SystemNotes[i] + "\"";

            array += "},";
        }
        array = array.substring(0, array.length - 1);
        array += "]";


        $scope.informationList = JSON.parse(array);


        //$scope.informationList = angular.copy(row.Product.SystemNotes);
        $('#modal-information').modal('show');
    };

    $scope.showProductDetailModal = function (row) {

        $.ajax({
            type: "GET",
            url: "/Search/ProductDetail?productId=" + row.Product.Id + "&groupId=" + row.Product.GroupId + "&productCode=" + row.Product.Code + "&campaignType=" + parseInt(row.Product.Campaign.Type),
            data: '',
            datatype: "html",
            beforeSend: function () {

            },
            success: function (viewHTML) {

                $('.productDetailValue').empty().append(viewHTML);

                //$compile(html)($scope);
                $('#modal-productDetail').modal('show');
            },
            error: function (errorData) {

            }
            //complete: function () {
            //    $scope.markFunction($scope.txtGeneralSearchText);
            //}
        });


    };

    $scope.fireConfirm = function (t, c, b) {
        //Örn: "Başlık", "İçerik", b[n, t, c, f]
        var btns = {};

        $.each(b, function (eName, eValue) {
            btns[eValue["n"]] = {
                text: eValue.t,
                btnClass: eValue.c,
                action: eValue.f
            }
        });

        $.confirm({
            title: t,
            content: c,
            buttons: btns,
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false
        });
    };

    $scope.arrowUp = function (id, minorder) {
        var total = $('#quantity_' + id).val();
        $('#quantity_' + id).val(++total);
    };

    $scope.arrowDown = function (id, minorder) {
        var total = $('#quantity_' + id).val();
        if (--total >= minorder)
            $('#quantity_' + id).val(total);
        else {
            $('#quantity_' + id).val(minorder);
            iziToast.error({
                //title: 'Hata',
                message: 'Minumum sipariş miktarından az sipariş veremezsiniz.',
                position: 'topCenter'
            });
        }
    };

    $scope.minOrderControl = function (id, minorder) {
        var total = $('#quantity_' + id).val();
        if (total >= minorder)
            $('#quantity_' + id).val(total);
        else {
            $('#quantity_' + id).val(minorder);
            iziToast.error({
                //title: 'Hata',
                message: 'Minumum sipariş miktarından az sipariş veremessiniz.',
                position: 'topCenter'
            });
        }
    };
    // #endregion

    $scope.getShipmentTypeList = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Cart/GetShipmentTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.shipmentTypeList = response.data;
            fireCustomLoading(false);
        });
    };

    $scope.askItemFixedPrice = function (id, priceValue, priceDecimal) {
        $.confirm({
            title: 'Fiyat Sabitleme',
            content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<input type="text"  placeholder="0" class="fixedPriceValue form-control eryaz-numeric-input-onlynumber" required value="' + priceValue + '" />' +
                '<input type="text"  placeholder="0" class="fixedPriceDecimal form-control eryaz-numeric-input-onlynumber" required value="' + priceDecimal + '" />' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () {
                        var fixedPriceValue = this.$content.find('.fixedPriceValue').val();
                        var fixedPriceDecimal = this.$content.find('.fixedPriceDecimal').val();
                        if (!fixedPriceValue || !fixedPriceDecimal) {
                            return false;
                        }
                        $scope.updateItemFixedPrice(id, fixedPriceValue, fixedPriceDecimal, true);
                    }

                },
                Cancel: {
                    text: 'Sabitlemeyi İptal Et',
                    btnClass: 'btn-orange',
                    keys: ['esc'],
                    action: function () {
                        $scope.updateItemFixedPrice(id, 0, 0, false);
                    }
                },
                Vazgeç: function () {
                    //close
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false,
            onOpen: function () {
                firePriceFormatNumberOnly();
                this.$content.find('.fixedPriceValue').val(priceValue);
                this.$content.find('.fixedPriceDecimal').val(priceDecimal)
            }
        });
    };

    $scope.askItemDiscSpecial = function (basket) {
        $.confirm({
            title: 'Özel İskonto',
            content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<input type="text"  placeholder="0" class="discSpecialValue form-control eryaz-numeric-input" required value="0" />' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () {
                        var discSpecialValue = this.$content.find('.discSpecialValue').val();
                        if (!discSpecialValue) {
                            return false;
                        }
                        basket.DiscSpecial = parseFloat(discSpecialValue);
                        $scope.updateItemDiscSpecial(basket);
                    }

                },
                Vazgeç: function () {
                    //close
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false,
            onOpen: function () {
                firePriceFormat();
                this.$content.find('.discSpecialValue').val(basket.DiscSpecial);
            }
        });

    };

    $scope.updateItemDiscSpecial = function (basket) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Cart/UpdateItemDiscSpecial",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { basket: basket }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.loadBasketData(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.updateItemFixedPrice = function (id, fixedPriceValue, fixedPriceDecimal, isActive) {
        $http({
            method: "POST",
            url: "/Cart/UpdateItemFixedPrice",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, fixedPriceValue: fixedPriceValue, fixedPriceDecimal: fixedPriceDecimal, isActive: isActive }

        }).then(function (response) {
            $scope.loadBasketData(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.basketNoteChange = function (id) {
        $.each($scope.basketNotesList, function (index, value) {
            if (value.Id === parseInt(id)) {
                $scope.basketNotId = value.Id;
                $('#txtOrderNote').val(value.Note);
                return false;
            }
        });
    };
    function ltrim(str) {
        return str.replace(/^\s+/, "");
    }
    $scope.saveAndUpdateBasketNote = function () {
        var note = $('#txtOrderNote').val();

        if (ltrim(note) === "") {
            iziToast.error({
                message: 'Lütfen sipariş notu giriniz.',
                position: 'topCenter'
            });
            return;
        }

        $http({
            method: "POST",
            url: "/Cart/SaveBasketNote",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.basketNotId, note: note }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.loadBasketNotes();
        });
    };

    $scope.deleteBasketNote = function () {
        if ($scope.basketNotId !== -1) {
            var note = $('#txtOrderNote').val();
            $http({
                method: "POST",
                url: "/Cart/DeleteBasketNote",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: $scope.basketNotId, note: note }

            }).then(function (response) {
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $scope.basketNotId = -1;
                $scope.loadBasketNotes();
                $('#txtOrderNote').val('');
            });
        }
    };

    $scope.loadBasketNotes = function () {
        $http({
            method: "POST",
            url: "/Cart/LoadBasketNotes",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.basketNotesList = response.data;
        });
    };

    $scope.askItemNotes = function (item) {
        $.confirm({
            title: 'Sepet Notunuz',
            content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<input type="text"  placeholder="Yeni Not Giriniz.." class="note form-control" required value="' + item.Notes + '" />' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () {
                        var note = this.$content.find('.note').val();
                        if (!note) {
                            return false;
                        }
                        item.Notes = note;
                        $scope.updateItemNotes(item.Id, item.Notes);
                    }
                },
                Vazgeç: function () {
                    //close
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false
        });
    };

    $scope.addDeliveryDate = function (basketItem) {
        $.confirm({
            onOpen: function () {
                $('.deliveryDate').datetimepicker({
                    format: 'DD/MM/YYYY',
                    defaultDate: new Date(),
                    locale: 'tr'

                }).val(basketItem.DeliveryDate);;

            },
            onClose: function () {
                $(".deliveryDate").datetimepicker("destroy");
            },
            title: 'Teslim Tarihi',
            content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<input type="text"  placeholder="Teslim Tarihi Giriniz" value="' + basketItem.DeliveryDate + '" class="deliveryDate form-control" required/>' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () {
                        var deliveryDate = this.$content.find('.deliveryDate').val();
                        if (!deliveryDate) {
                            return false;
                        }
                        $scope.updateItemdeliveryDate(basketItem.Id, deliveryDate);
                    }
                },
                Vazgeç: function () {
                    //close
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false,
            columnClass: 'col-md-4 col-md-offset-4 col-sm-6 col-sm-offset-3 col-xs-10 col-xs-offset-1 jconfirm-none-overflow'
        });


    };

    $scope.updateItemNotes = function (id, note) {
        $http({
            method: "POST",
            url: "/Cart/UpdateItemNotes",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, note: note }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };
    $scope.basketTransfer = function (basketItem) {
        var basketType = (($('#basketTypeControlSwitch').length) ? (($('#basketTypeControlSwitch').prop("checked")) ? 0 : 1) : -1);
        $http({
            method: "POST",
            url: "/Cart/UpdateBasketAddType",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { basketItem: basketItem, basketType: basketType }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            B2BServices.getBasketCount();
            $scope.loadBasketData(false);

        });
    };

    $scope.cancelCampaign = function (row, result) {
        row.IsCancelCampaign = result;
        $http({
            method: "POST",
            url: "/Cart/UpdateBasketItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { basket: row }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.loadBasketData(false);
        });
    };

    $scope.updateItemdeliveryDate = function (id, deliveryDate) {
        $http({
            method: "POST",
            url: "/Cart/UpdateItemDeliveryDate",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, deliveryDate: deliveryDate }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.deleteBasketItem = function (id) {
        $http({
            method: "POST",
            url: "/Cart/DeleteBasketItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            B2BServices.getBasketCount();
            $scope.loadBasketData(false);

        });
    };

    $scope.deleteBasketSelected = function () {
        $http({
            method: "POST",
            url: "/Cart/DeleteSelected",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            B2BServices.getBasketCount();
            $scope.loadBasketData(false);

        });
    };

    $scope.deleteBasketAll = function () {
        $http({
            method: "POST",
            url: "/Cart/DeleteAll",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            B2BServices.getBasketCount();
            $scope.loadBasketData(false);

        });
    };

    $scope.askForDelete = function (id) {
        $.confirm({
            title: 'Uyarı!',
            content: "Ürünü silmek istiyor musunuz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () { $scope.deleteBasketItem(id); }
                },
                Cancel: {
                    text: "Hayır",
                    btnClass: 'btn-red any-other-class',
                    keys: ['esc'],
                    action: function () {
                        iziToast.error({
                            message: 'Silme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false
        });
    };

    $scope.askDeleteSelected = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Seçili Ürünleri silmek istiyor musunuz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () { $scope.deleteBasketSelected(); }
                },
                Cancel: {
                    text: "Hayır",
                    btnClass: 'btn-red any-other-class',
                    keys: ['esc'],
                    action: function () {
                        iziToast.error({
                            message: 'Silme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false
        });
    };

    $scope.askDeleteAll = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Sepetteki tüm ürünleri silmek istiyor musunuz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () { $scope.deleteBasketAll(); }
                },
                Cancel: {
                    text: "Hayır",
                    btnClass: 'btn-red any-other-class',
                    keys: ['esc'],
                    action: function () {
                        iziToast.error({
                            message: 'Silme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false
        });
    };

    $scope.checkCouponForCustomer = function (couponCode) {

        $http({
            method: "POST",
            url: "/Cart/CheckCouponForCustomer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { couponCode: couponCode }
        }).then(function (response) {
            var result = response.data;
            if (result.ResultId > 0) {
                $scope.selectedCopunCode = couponCode;
                $scope.loadBasketData(true);
                $scope.manuelCopunUse = true;
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
            }
            else {
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
            }
        });
    }

    // #region PAymentMethods

    $scope.AddOrderPaymentType = function (valid) {
        $scope.OrderPaymentType.PaymentDate = $('#datetimepicker1').val();
        $scope.OrderPaymentType.Total = parseFloat($scope.OrderPaymentType.Total.toString() + "." + $scope.OrderPaymentType.TotalDouble.toString());
        console.log(valid);
        if (valid) {
            $http({
                method: "POST",
                url: "/Cart/AddOrderPayment",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { payment: $scope.OrderPaymentType }
            }).then(function (response) {
                $scope.OrderPaymentTypeList = response.data;
                $scope.OrderPaymentType.Total = "";
                $scope.OrderPaymentType.TotalDouble = "00";

                $scope.OrderPaymentType.ReceiptNo = "";
                $scope.OrderPaymentType.BankName = "";
                $scope.OrderPaymentType.Base64 = "";

                $scope.getTotal();
            });
        }

    }
    $scope.removeImage = function () {
        $scope.OrderPaymentType.Base64 = "";

    }
    $scope.addNextPrice = function () {

        //var type = parseFloat($scope.basketListTotals.GeneralTotal.replace("&nbsp;<i class='fa fa-try' aria-hidden='true'></i>", "").replace(".", "").replace(",", ".")) - $scope.Gtotal;
        //console.log(type.toString())
        //var virgul = type.toString().indexOf(".");
        //console.log(virgul);
        //var decimal = type.toString().substring(0, virgul)
        //console.log(decimal);
        //var double = type.toString().substring((virgul + 1), (virgul + 3))
        //console.log(double);


        var finalValue = $scope.basketListTotals.GeneralTotalDouble - $scope.Gtotal;

        $scope.OrderPaymentType.Total = finalValue.toFixed(2).split('.')[0];
        $scope.OrderPaymentType.TotalDouble = finalValue.toFixed(2).split('.')[1];

    }
    $scope.RemoveOrderPaymentType = function (getPayment) {


        $http({
            method: "POST",
            url: "/Cart/RemoveOrderPayment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { payment: getPayment }
        }).then(function (response) {
            $scope.OrderPaymentTypeList = response.data;
            $scope.OrderPaymentType.Total = "";
            $scope.OrderPaymentType.TotalDouble = "00";

            $scope.OrderPaymentType.ReceiptNo = "";
            $scope.OrderPaymentType.BankName = "";
            $scope.getTotal();
        });
    }
    $scope.saveOrderPaymentType = function () {

        if ($scope.OrderPaymentTypeList.length > 0) {
            if ($scope.OrderPaymentTypeList[0].IsPaymentOk) {
                $('#modal-orderPayment').modal('hide');
                $scope.checkOrdersForSend(0);
            } else {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Lütfen Sepet Tutarı Kadar Ödeme Bilgisi Giriniz. ',
                    position: 'topCenter'
                });
            }
        }
        else {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen Ödeme Bilgisi Giriniz. ',
                position: 'topCenter'
            });
        }


    };

    $scope.drpChange = function () {
        $scope.OrderPaymentType.Total = "";
        $scope.OrderPaymentType.TotalDouble = "00";

        $scope.OrderPaymentType.ReceiptNo = "";
        $scope.OrderPaymentType.BankName = "";
    }
    $scope.Gtotal = 0;
    $scope.getTotal = function () {
        var total = 0;
        $scope.Gtotal = 0;

        for (var i = 0; i < $scope.OrderPaymentTypeList.length; i++) {
            $scope.Gtotal += ($scope.OrderPaymentTypeList[i].Total);
        }
        return total;
    }

    // #endregion


    $scope.loadBasketData = function (coupon) {
        fireCustomLoading(true);
        //var basketType = ($('#basketTypeControl').val() === undefined || $('#basketTypeControl').val() === "") ? -1 : $('#basketTypeControl').val();
        var basketType = (($('#basketTypeControlSwitch').length) ? (($('#basketTypeControlSwitch').prop("checked")) ? 1 : 0) : -1);
        $http({
            method: "POST",
            url: "/Cart/LoadBasketDataJsonResult",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { basketType: parseInt(basketType), coupon: coupon, couponCode: $scope.selectedCopunCode }
        }).then(function (response) {
            $scope.basketList = response.data;
            $scope.loadBasketTotals();

            var takimCount = 0;
            $.each($scope.basketList, function (index, item) {
                if (item.Product.Unit == 'TAKIM') {
                    takimCount += item.Quantity;
                }
            });

            $scope.showFreeShipping = takimCount >= 10;
        });

    };

    $scope.loadBasketTotals = function () {
        $http({
            method: "POST",
            url: "/Cart/LoadGeneralTotals",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.basketListTotals = response.data;
            fireCustomLoading(false);
            $scope.addNextPrice();

        });
    };

    $scope.basketItemChecked = function ($event, id) {
        var checkbox = $event.target;
        $http({
            method: "POST",
            url: "/Cart/BasketItemChecked",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, checkedValue: checkbox.checked }
        }).then(function (responses) {
            $scope.loadBasketData(false);
        });
    };

    $scope.basketAllCheckControl = function ($event) {
        var checkbox = $event.target;

        $http({
            method: "POST",
            url: "/Cart/BasketAllChecked",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { checkedValue: checkbox.checked },
            async: false
        }).then(function (responses) {
            $scope.loadBasketData(false);
        });
    }
    $scope.checkUpdateQuantity = function (id, totalQuantity, saledQuantity) {
        $('#updateIcon_' + id).addClass("fa-spin");
        var quantity = $('#quantity_' + id).val();
        if (totalQuantity !== 0 && quantity > (totalQuantity - saledQuantity)) {
            $.confirm({
                title: 'Kampanya Uyarısı',
                content: '<form action="" class="formName">İstemiş olduğunuz miktar kampanya miktarından fazladır. Fazla ürünler normal fiyatlandırma üzerinden eklensin mi ? </form>',
                buttons: {
                    formSubmit: {
                        text: 'EVET EKLENSİN',
                        btnClass: 'btn-custom',
                        keys: ['enter'],
                        action: function () {
                            $scope.updateQuantityValue(id, quantity);
                        }

                    },
                    Cancel: {
                        text: 'FAZLASINI İPTAL ET',
                        btnClass: 'btn-warning',
                        keys: ['esc'],
                        action: function () {
                            $scope.updateQuantityValue(id, (totalQuantity - saledQuantity));
                        }
                    }
                },
                scrollToPreviousElement: false,
                scrollToPreviousElementAnimate: false
            });
        }
        else {
            $scope.updateQuantityValue(id, quantity);
        }
    };

    $scope.showCampaignMessage = function (id, qty, quantityOrj) {

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
                        url: "/Cart/UpdateQuantityValue",
                        headers: { "Content-Type": "Application/json;charset=utf-8" },
                        data: { id: id, quantity: (qty + parseInt(quantityOrj)) }

                    }).then(function (response) {
                        iziToast.show({
                            message: response.data.Message,
                            position: 'topCenter',
                            color: response.data.Color,
                            icon: response.data.Icon
                        });
                        $scope.loadBasketData(false);
                        instance.hide({
                            transitionOut: 'fadeOutUp',
                            onClosing: function (instance, toast, closedBy) {
                                console.info('closedBy: ' + closedBy); // The return will be: 'closedBy: buttonName'
                            }
                        }, toast, 'buttonName');
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


    $scope.updateQuantityValue = function (id, quantity) {
        $('#updateIcon_' + id).addClass("fa-spin");
        $http({
            method: "POST",
            url: "/Cart/UpdateQuantityValue",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, quantity: quantity }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.loadBasketData(false);
            $('#updateIcon_' + id).removeClass("fa-spin");

            if (response.data.ResultId > 0) {
                $scope.showCampaignMessage(id, response.data.ResultId, quantity)
            }

        });
    };
    $scope.checkPaymentTypeModelOpen = function () {
        var shipmentValue = $('#shipmentType-select').val();
        var note = $('#txtOrderNote').val();
        if (shipmentValue === '? undefined:undefined ?' || shipmentValue === "0") {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen Gönderi Şekli Seçiniz ',
                position: 'topCenter'
            });
            return;
        }

        var shipmentPerson = '';
        var shipmentAddress = '';
        var shipmentTel = '';
        var shipmentCity = '';
        var shipmentTown = '';

        $scope.SelectedShipmentDifferent = $('#shipmentDifferent-select').val();

        shipmentPerson = $('#txtShipmentPerson').val().trim();
        shipmentTel = $('#txtShipmentTel').val().trim();


        if ($scope.SelectedShipmentDifferent === '1') {
            shipmentAddress = $('#txtShipmentAddress').val().trim();
            shipmentCity = $('#txtShipmentCity').val().trim();
            shipmentTown = $('#txtShipmentTown').val().trim();

            if (shipmentPerson === '' || shipmentAddress === '' || shipmentTel === '' || shipmentCity === '' || shipmentTown === '') {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Lütfen Sevk alanlarının hepsini doldurunuz',
                    position: 'topCenter'
                });
                return;
            }
        }
        else {
            if (shipmentPerson === '' || shipmentTel === '') {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Lütfen Sevk alanlarının hepsini doldurunuz',
                    position: 'topCenter'
                });
                return;
            }
        }

        $('#modal-orderPayment').modal();
    }
    $scope.checkOrdersForSend = function (type) {
        var shipmentValue = $('#shipmentType-select').val();
        if (shipmentValue === '? undefined:undefined ?' || shipmentValue === "0") {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen Gönderi Şekli Seçiniz ',
                position: 'topCenter'
            });
        }
        else {
            $('#btnSend').addClass('hidden');
            $('#btnSendPayment').addClass('hidden');
            $('#spinnerBasket').removeClass('hidden');
            $http({
                method: "POST",
                url: "/Cart/CheckOrderForSend",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}

            }).then(function (response) {
                $scope.reasonList = response.data;
                if ($scope.reasonList.length <= 0)
                    $scope.sendOrder();
            });
        }
    };

    $scope.fireTooltips = function (e, o) {
        var screen = $("body");
        var screenWidth = screen.width();
        var screenHeight = screen.height();
        $(e).tooltipster({
            position: (screenWidth <= 1024) ? 'bottom' : 'left',
            speed: 100,
            maxWidth: (screenWidth <= 1024) ? 300 : null,
            theme: 'tooltipster-light',
            animation: 'fade', // fade, grow, swing, slide, fall
            multiple: true,
            trigger: (screenWidth <= 1024) ? 'click' : 'hover',
            contentAsHTML: true,
            contentCloning: false,
            positionTracker: true,
            triggerOpen: {
                click: true,  // For mouse
                tap: true    // For touch device
            },
            triggerClose: {
                click: true,  // For mouse
                tap: true    // For touch device
            }
        });
        if ($(e).is(':hover'))
            $(e).tooltipster('open');
    };

    $scope.titleWarehouseValue = function ($event, productItem) {
        var e = $event.currentTarget;
        if (!$(e).hasClass("tooltipstered")) {

            $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            $scope.fireTooltips(e, true);

            $http({
                method: "POST",
                url: "/Partial/WarehouseView",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { productItem: productItem, productId: -1 }
            }).then(function (viewHTML) {

                if (viewHTML.data !== "") {
                    $(e).tooltipster('content', viewHTML.data);
                }
                else {
                    $(e).tooltipster('destroy', true);
                }

                $scope.lastItem++;
            });
        }

    };

    $scope.openKitDetail = function ($event, productItem) {
        var e = $event.currentTarget;
        if (!$(e).hasClass("tooltipstered")) {

            $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            //  $scope.fireTooltips(e, true);

            $(e).tooltipster({
                position: 'right',
                speed: 100,
                //maxWidth:  null,
                theme: 'tooltipster-light',
                animation: 'fade', // fade, grow, swing, slide, fall
                multiple: true,
                trigger: 'hover',
                contentAsHTML: true,
                contentCloning: false,
                positionTracker: true,
                triggerOpen: {
                    click: true,  // For mouse
                    tap: true    // For touch device
                },
                triggerClose: {
                    click: true,  // For mouse
                    tap: true    // For touch device
                }
            });
            if ($(e).is(':hover'))
                $(e).tooltipster('open');

            $http({
                method: "POST",
                url: "/Partial/KitDetailView",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { productItem: productItem }
            }).then(function (viewHTML) {
                if (viewHTML.data !== "") {
                    $(e).tooltipster('content', viewHTML.data);
                }
                else {
                    $(e).tooltipster('destroy', true);
                }

                $scope.lastItem++;
            });
        }

    };

    $scope.titleValue = function ($event, item) {
        var e = $event.currentTarget;
        var basket = item;
        if (!$(e).hasClass("tooltipstered")) {
            $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            $scope.fireTooltips(e);
            $http({
                method: "POST",
                url: "/Partial/PriceView",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { basket: basket }
            }).then(function (viewHTML) {
                var html = viewHTML;
                $(e).tooltipster('content', html.data);
                $scope.lastItem++;
            });

            //$.ajax({
            //    type: "POST",
            //   // headers: { "Content-Type": "Application/json;charset=utf-8" },
            //    url: "/Partial/PriceView",
            //    data: { basket},
            //    //datatype: "html",
            //    beforeSend: function () {
            //        $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            //        $scope.fireTooltips(e);
            //    },
            //    success: function (viewHTML) {
            //        var html = viewHTML;
            //        $(e).tooltipster('content', html);
            //    },
            //    error: function (errorData) {
            //        //console.log(errorData);
            //    },
            //    complete: function () {
            //        $scope.lastItem++;

            //        //if ($scope.lastItem == $scope.basketList.length)
            //        //    $scope.fireTooltips();
            //    }
            //});
        }
    };

    $scope.reasonItemChange = function (value, id) {
        $http({
            method: "POST",
            url: "/Cart/ReasonItemChange",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, value: value }
        }).then(function (response) {
        });
    };

    $scope.sendOrder = function () {
        var shipmentValue = $('#shipmentType-select').val();
        var note = $('#txtOrderNote').val();
        var freeshipping = $('#freeShipping').val();
        var paymentNote = $('#txtOrderPaymentNote').val();
        var shipmentInformation = $('#shipmentInformation').val();

        if (shipmentValue === '? undefined:undefined ?' || shipmentValue === "0") {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen Gönderi Şekli Seçiniz ',
                position: 'topCenter'
            });
            return;
        }

        var shipmentPerson = '';
        var shipmentAddress = '';
        var shipmentTel = '';
        var shipmentCity = '';
        var shipmentTown = '';

        $scope.SelectedShipmentDifferent = $('#shipmentDifferent-select').val();


        shipmentPerson = $('#txtShipmentPerson').val().trim();
        shipmentTel = $('#txtShipmentTel').val().trim();

        if ($scope.SelectedShipmentDifferent === '1') {

            shipmentAddress = $('#txtShipmentAddress').val().trim();
            shipmentCity = $('#txtShipmentCity').val().trim();
            shipmentTown = $('#txtShipmentTown').val().trim();

            if (shipmentPerson === '' || shipmentAddress === '' || shipmentTel === '' || shipmentCity === '' || shipmentTown === '') {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Lütfen Sevk alanlarının hepsini doldurunuz',
                    position: 'topCenter'
                });
                return;
            }
        }
        else {
            if (shipmentPerson === '' || shipmentTel === '') {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Lütfen Sevk alanlarının hepsini doldurunuz',
                    position: 'topCenter'
                });
                return;
            }
        }


        $http({
            method: "POST",
            url: "/Cart/CheckPayment",
            //url: "/Cart/SendOrder",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { shipmentValue: shipmentValue, note: note, paymentNote: paymentNote, shipmentPerson: shipmentPerson, shipmentAddress: shipmentAddress, shipmentTel: shipmentTel, shipmentCity: shipmentCity, shipmentTown: shipmentTown, freeshipping: parseInt(freeshipping), shipmentInformation: shipmentInformation }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            if (response.data.Statu === "payment")
                window.location = 'Payment';

            $scope.loadBasketData(false);
        });
    };

    // #region Excel Upload
    $scope.openUploadModal = function () {
        $('#modal_upload_cartfromexcel').modal();
    };


    $scope.ExportExcelbasket = function () {

        if ($scope.basketList.length > 0) {
            var newData = [];
            $scope.basketList.forEach(function (e) {
                newData.push({
                    "Depo": e.Product.TotalQuantity > 0 ? 'Var' : 'Yok',
                    "Ürün Kodu": e.Product.Code,
                    "Ürün Adı": e.Product.Name,
                    "Üretici": e.Product.Manufacturer,
                    "Miktar": e.Quantity,
                    "Birim": e.Product.Unit,
                    "Fiyat": e.Product.Price.toFixed(2),
                    "İsk.": e.Product.DiscountStr,
                    "Net Fiyat": e.Product.PriceNet.Value.toFixed(2),
                    "Tutar": e.TotalPrice,
                    "Net Fiyat Kdv Dahil": e.TotalCostWithVAT.toFixed(2),
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                    "Depo",
                    "Ürün Kodu",
                    "Ürün Adı",
                    "Üretici",
                    "Miktar",
                    "Birim",
                    "Fiyat",
                    "İsk.",
                    "Net Fiyat",
                    "Tutar",
                    "Net Fiyat Kdv Dahil",
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 5 }, { wch: 25 }, { wch: 40 }, { wch: 8 }, { wch: 10 }, { wch: 10 }, { wch: 10 }, { wch: 10 }, { wch: 10 },
            { wch: 10 }, { wch: 15 }
            ];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Liste");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Sepet.xlsx");
        }
    }























    $scope.uploadCartExcelFile = function () {
        var fileUpload = document.getElementById('cart-excel-selector');
        var files = fileUpload.files;

        if (files.length === 0) {
            iziToast.error({
                message: 'Lütfen dosya seçiniz',
                position: 'topCenter',
            });
            return;
        }

        var f = files[0];
        {
            fireCustomLoading(true);

            var reader = new FileReader();
            var name = f.name;
            reader.onload = function (e) {
                var data = e.target.result;
                var workbook = XLSX.read(data, { type: 'binary' });
                var jsonData;
                workbook.SheetNames.forEach(function (sheetName) {
                    jsonData = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName], { raw: true });
                });

                jsonData.forEach(function (e) {
                    e.Code = e.KOD;
                    e.Quantity = e.MİKTAR;
                    delete e.KOD;
                    delete e.MİKTAR;
                });

                var basketType = (($('#basketTypeControlSwitch').length) ? (($('#basketTypeControlSwitch').prop("checked")) ? 1 : 0) : -1);
                $http({
                    method: "POST",
                    url: "/Cart/UploadExcelJson",
                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                    data: { excelData: jsonData, basketType: parseInt(basketType) }

                }).then(function successCallback(response) {
                    var status = response.data[0].Value;

                    if (status) {
                        // success
                        //if (response.data[3].Value.length > 0) {
                        $scope.excelSuccessMessage = response.data[1].Value;
                        $scope.excelErrorList = response.data[2].Value;
                        $scope.excelSuccessList = response.data[4].Value;
                        $scope.excelNotFoundCodes = response.data[3].Value;
                        //  }
                    }
                    else {
                        $scope.excelErrorMessage = response.data[1].Value;
                    }

                    $scope.loadBasketData(false);
                }, function errorCallback(response) {
                    $scope.loadBasketData(false);
                });


                //    .then(function (response) {
                //    alert(retponse.data);
                //    fireCustomLoading(false);

                //    
                //});
            }
        }
        reader.readAsBinaryString(f);
    }

    $scope.exportExcelFile = function () {

        if ($scope.excelNotFoundCodes.length <= 0)
            return;
        $scope.excelNotFoundCodes.forEach(function (e) {
            e.KOD = e.Code;
            e.MİKTAR = e.Quantity;
            delete e.Code;
            delete e.Quantity;
        });

        var ws = XLSX.utils.json_to_sheet($scope.excelNotFoundCodes);
        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, "Bulunamayan Kodlar");
        var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
        saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Bulunamayan Kodlar.xlsx");

    }


    // #endregion

    $scope.$on('ngRepeatBasketListFinished', function (ngRepeatBasketListFinishedEvent) {
        if ($scope.basketList.length > 0) {
            var chks = $('.table-basket input');
            var allC = true;

            for (var i = 0; i < chks.length; i++) {
                if (chks[i].type === "checkbox" && !chks[i].checked) {
                    allC = false;
                }
            }
            $scope.basketAllCheck = allC;
            $('#chckallCheck').prop('checked', allC);


        }
    });
    $scope.$on('ngRepeatReasonListFinished', function (ngRepeatReasonListFinishedEvent) {
        if ($scope.reasonList.length > 0) {
            $('#modal-reason').on('show.bs.modal', function () {
                $('.selectbox-reasonItem').SumoSelect();
            })
            $('#modal-reason').on('hidden.bs.modal', function () {
                $('#spinnerBasket').addClass('hidden');
                $('#btnSend').removeClass('hidden');
                $('#btnSendPayment').removeClass('hidden');
                return false;
            });
            $('#modal-reason').modal('show');
        }
    });
    $scope.$on('ngRepeatShipmentTypeFinished', function (ngRepeatShipmentTypeFinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        $('.selectbox-shipmentType').SumoSelect();
        $('.selectbox-shipmentType')[0].sumo.reload();
        $('.selectbox-shipmentType').on('sumo:closed', function (sumo) {
            //if ($('.selectbox-shipmentType').val() !== "Seçiniz" && $scope.SelectedProductGroup1 !== "Seçiniz") {
            //} else {
            //    $('.selectbox-shipmentType').val("Seçiniz");
            //}
        });
    });
    $scope.$on('ngRepeatBasketNotesFinished', function (ngRepeatBasketNotesFinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        $('.selectbox-basketNotes').SumoSelect();
        $('.selectbox-basketNotes')[0].sumo.reload();

        $('.selectbox-basketNotes').on('sumo:closed', function (sumo) {
            if ($('.selectbox-basketNotes').val() !== "Seçiniz") {
                $scope.basketNoteChange($('.selectbox-basketNotes').val());
            }
            else {
                $('.selectbox-basketNotes').val("Seçiniz");
            }
        });
    });
    $(document).ready(function () {
        $scope.loadBasketData(false);
        $scope.getShipmentTypeList();
        $scope.loadBasketNotes();

        var today = new Date();
        var strDate = 'd/m/Y'
            .replace('d', today.getDate())
            .replace('m', today.getMonth() + 1)
            .replace('Y', today.getFullYear());

        $('.datetime').datetimepicker({
            format: 'DD/MM/YYYY',
            defaultDate: new Date(),
            locale: 'tr'

        }).val(strDate);

        $('#radioBtn a').on('click', function () {
            var sel = $(this).data('title');
            var tog = $(this).data('toggle');
            $('#' + tog).prop('value', sel);
            $('a[data-toggle="' + tog + '"]').not('[data-title="' + sel + '"]').removeClass('active').addClass('notActive');
            $('a[data-toggle="' + tog + '"][data-title="' + sel + '"]').removeClass('notActive').addClass('active');
            $scope.loadBasketData(false);

        });


        //$.getJSON("/Content/il-bolge.json", function (sonuc) {
        //    $.each(sonuc, function (index, value) {
        //        var row = "";
        //        row += '<option value="' + value.il + '">' + value.il + '</option>';
        //        $("#il").append(row);

        //        $("#il").SumoSelect();

        //        $("#il")[0].sumo.reload();
        //    })
        //});
        //$("#il").on("change", function () {
        //    var il = $(this).val();
        //    $("#ilce").attr("disabled", false).html("<option value=''>Seçin..</option>");
        //    $.getJSON("/Content/il-ilce.json", function (sonuc) {
        //        $.each(sonuc, function (index, value) {
        //            var row = "";
        //            if (value.il == il) {
        //                row += '<option value="' + value.ilce + '">' + value.ilce + '</option>';
        //                $("#ilce").append(row);
        //            }
        //        });
        //    });
        //});




    });
}).directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;


            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });


            ctrl.$parsers.unshift(function (viewValue) {

                elem.priceFormat({
                    prefix: '',
                    centsSeparator: ',',
                    thousandsSeparator: '.'
                });

                return elem[0].value;
            });
        }
    };
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

(function ($) { $.fn.priceFormat = function (options) { var defaults = { prefix: 'US$ ', suffix: '', centsSeparator: '.', thousandsSeparator: ',', limit: false, centsLimit: 2, clearPrefix: false, clearSufix: false, allowNegative: false, insertPlusSign: false }; var options = $.extend(defaults, options); return this.each(function () { var obj = $(this); var is_number = /[0-9]/; var prefix = options.prefix; var suffix = options.suffix; var centsSeparator = options.centsSeparator; var thousandsSeparator = options.thousandsSeparator; var limit = options.limit; var centsLimit = options.centsLimit; var clearPrefix = options.clearPrefix; var clearSuffix = options.clearSuffix; var allowNegative = options.allowNegative; var insertPlusSign = options.insertPlusSign; if (insertPlusSign) allowNegative = true; function to_numbers(str) { var formatted = ''; for (var i = 0; i < (str.length); i++) { char_ = str.charAt(i); if (formatted.length == 0 && char_ == 0) char_ = false; if (char_ && char_.match(is_number)) { if (limit) { if (formatted.length < limit) formatted = formatted + char_ } else { formatted = formatted + char_ } } } return formatted } function fill_with_zeroes(str) { while (str.length < (centsLimit + 1)) str = '0' + str; return str } function price_format(str) { var formatted = fill_with_zeroes(to_numbers(str)); var thousandsFormatted = ''; var thousandsCount = 0; if (centsLimit == 0) { centsSeparator = ""; centsVal = "" } var centsVal = formatted.substr(formatted.length - centsLimit, centsLimit); var integerVal = formatted.substr(0, formatted.length - centsLimit); formatted = (centsLimit == 0) ? integerVal : integerVal + centsSeparator + centsVal; if (thousandsSeparator || $.trim(thousandsSeparator) != "") { for (var j = integerVal.length; j > 0; j--) { char_ = integerVal.substr(j - 1, 1); thousandsCount++; if (thousandsCount % 3 == 0) char_ = thousandsSeparator + char_; thousandsFormatted = char_ + thousandsFormatted } if (thousandsFormatted.substr(0, 1) == thousandsSeparator) thousandsFormatted = thousandsFormatted.substring(1, thousandsFormatted.length); formatted = (centsLimit == 0) ? thousandsFormatted : thousandsFormatted + centsSeparator + centsVal } if (allowNegative && (integerVal != 0 || centsVal != 0)) { if (str.indexOf('-') != -1 && str.indexOf('+') < str.indexOf('-')) { formatted = '-' + formatted } else { if (!insertPlusSign) formatted = '' + formatted; else formatted = '+' + formatted } } if (prefix) formatted = prefix + formatted; if (suffix) formatted = formatted + suffix; return formatted } function key_check(e) { var code = (e.keyCode ? e.keyCode : e.which); var typed = String.fromCharCode(code); var functional = false; var str = obj.val(); var newValue = price_format(str + typed); if ((code >= 48 && code <= 57) || (code >= 96 && code <= 105)) functional = true; if (code == 8) functional = true; if (code == 9) functional = true; if (code == 13) functional = true; if (code == 46) functional = true; if (code == 37) functional = true; if (code == 39) functional = true; if (allowNegative && (code == 189 || code == 109)) functional = true; if (insertPlusSign && (code == 187 || code == 107)) functional = true; if (!functional) { e.preventDefault(); e.stopPropagation(); if (str != newValue) obj.val(newValue) } } function price_it() { var str = obj.val(); var price = price_format(str); if (str != price) obj.val(price) } function add_prefix() { var val = obj.val(); obj.val(prefix + val) } function add_suffix() { var val = obj.val(); obj.val(val + suffix) } function clear_prefix() { if ($.trim(prefix) != '' && clearPrefix) { var array = obj.val().split(prefix); obj.val(array[1]) } } function clear_suffix() { if ($.trim(suffix) != '' && clearSuffix) { var array = obj.val().split(suffix); obj.val(array[0]) } } $(this).bind('keydown.price_format', key_check); $(this).bind('keyup.price_format', price_it); $(this).bind('focusout.price_format', price_it); if (clearPrefix) { $(this).bind('focusout.price_format', function () { clear_prefix() }); $(this).bind('focusin.price_format', function () { add_prefix() }) } if (clearSuffix) { $(this).bind('focusout.price_format', function () { clear_suffix() }); $(this).bind('focusin.price_format', function () { add_suffix() }) } if ($(this).val().length > 0) { price_it(); clear_prefix(); clear_suffix() } }) }; $.fn.unpriceFormat = function () { return $(this).unbind(".price_format") }; $.fn.unmask = function () { var field = $(this).val(); var result = ""; for (var f in field) { if (!isNaN(field[f]) || field[f] == "-") result += field[f] } return result } })(jQuery);