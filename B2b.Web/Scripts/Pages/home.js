b2bApp.controller('HomeController', ['$scope', '$http', '$sce', function ($scope, $http, $sce) {

    $scope.suggestionList = [];

    $scope.getCampaignList = function () {
        $http({
            method: "POST",
            url: "/Home/GetCampaignList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.campaignList = response.data;
        });
    };

    // Ng repeat işlemi bittiği zaman bu method çalışır
    $scope.$on('ngRepeatCampaignResultFinished', function (ngRepeatCampaignResultFinished) {
        fireItemHoverAnimation('#campaign .item-hover');
    });

    $scope.trustDangerousSnippet = function (snippet) {
        return $sce.trustAsHtml(snippet);
    };

    $scope.getBannerList = function () {
        $http({
            method: "POST",
            url: "/Home/GetBannerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.bannerList = response.data;
        });
    };

    // Ng repeat işlemi bittiği zaman bu method çalışır
    $scope.$on('ngRepeatBannerListResultFinished', function (ngRepeatBannerListResultFinished) {

        fireItemHoverAnimation('#banner .item-hover');
    });

    $scope.getNewArrivalList = function () {
        $http({
            method: "POST",
            url: "/Home/GetNewStockArrivalList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.newArrivalList = response.data;
        });
    };

    // Ng repeat işlemi bittiği zaman bu method çalışır
    $scope.$on('ngRepeatNewArrivalResultFinished', function (ngRepeatNewArrivalResultFinished) {
        fireItemHoverAnimation('#newStock .item-hover');
    });

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
            }
            else {
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
                EVET: {
                    btnClass: 'btn-blue',
                    action: function () { $scope.addBasket(id, type); }
                },
                HAYIR: {
                    btnClass: 'btn-red any-other-class',
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
        else if (type === 3) {
            qtyStr = $('#qtyIk' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qtyIk' + id).attr('placeholder');
        }
        else if (type === 4) {
            qtyStr = $('#qtyIo' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qtyIo' + id).attr('placeholder');
        }
        else if (type === 5) {
            qtyStr = $('#qtyIy' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qtyIy' + id).attr('placeholder');
        }
        else if (type === 6) {
            qtyStr = $('#qtyIyy-' + id).val();
            if (qtyStr === '')
                qtyStr = $('#qtyIyy-' + id).attr('placeholder');
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

                    //loadCartCount();
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

    $scope.fireAnnouncementOpeningMessage = function (announcementType) {

        $http({
            method: "POST",
            url: "/Home/AnnouncementTypeTextById",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: announcementType }
        }).then(function (response) {
            switch (announcementType) {
                case 3:
                    if (response.data.length > 0)
                        $scope.showMultipleVisualMessage(response.data, 0);
                    else
                        $scope.fireAnnouncementOpeningMessage(4);
                    break;
                case 4:
                    if (response.data.length > 0)
                        $scope.showMultipleGeneralMessage(response.data, 0);
                    else
                        $scope.fireAnnouncementOpeningMessage(5);
                    break;
                case 5:
                    if (response.data.length > 0)
                        $scope.showMultipleSalesmanMessage(response.data, 0);
                    else
                        $scope.customerWaitingInBasket();
                    //Daha Fazla Duyuru eklenirse burdan devam edilmeli
                    break;

            }


        });



    }

    $scope.customerWaitingInBasket = function () {
        $http({
            method: "POST",
            url: "/Home/GetCustomerWaitingInBasket",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {

            if (response.data === 'True')
                $.fancybox.open('<div style="max-width: 80%;max-height: 50%;"><h3>BİLGİLENDİRME</h3><p>Sepetinizde ki ürünler sipariş verilmemesi durumunda 24 saat sonra silinecektir.</p></div>',
                     {
                         transitionEffect: "zoom-in-out"
                     }
                );
        });
    }
    $scope.showMultipleSalesmanMessage = function (data, index) {
        if (data.length <= index) return;
        var obj = data[index];

        $.fancybox.open('<div style="max-width: 80%;max-height: 50%;"><h3>' + obj.Header + '</h3><p>' + obj.Content + '</p></div>',
            {
                afterClose: function () {
                    if (data.length !== index + 1)
                        $scope.showMultipleSalesmanMessage(data, index + 1);
                    else
                        $scope.customerWaitingInBasket();

                    //Daha Fazla Duyuru eklenirse burdan devam edilmeli
                },
                transitionEffect: "zoom-in-out"

            }
        );
    }
    $scope.showMultipleGeneralMessage = function (data, index) {
        if (data.length <= index) return;
        var obj = data[index];

        $.fancybox.open('<div style="max-width: 80%;max-height: 50%;"><h3>' + obj.Header + '</h3><p >' + obj.Content + '</p></div>',
            {
                afterClose: function () {
                    if (data.length !== index + 1)
                        $scope.showMultipleGeneralMessage(data, index + 1);
                    else
                        $scope.fireAnnouncementOpeningMessage(5);
                },
                transitionEffect: "zoom-in-out"

            }
        );
    }
    $scope.showMultipleVisualMessage = function (data, index) {
        if (data.length <= index) return;
        var obj = data[index];

        $.fancybox.open('<div><h3>' + obj.Header + '</h3><p><image src="' + obj.PicturePath + '"/></p></div>',
            {
                afterClose: function () {
                    if (data.length !== index + 1)
                        $scope.showMultipleVisualMessage(data, index + 1);
                    else
                        $scope.fireAnnouncementOpeningMessage(4);
                },
                transitionEffect: "zoom-in-out"
            }
        );

    };

    $scope.saveKvkkContract = function () {
        $http({
            method: "POST",
            url: "/Home/SaveKvkkContract",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $.fancybox.close();

        });
    };

    $scope.checkKvkkContract = function () {
        $http({
            method: "POST",
            url: "/Home/CheckKvkkContract",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {

            if (response.data !== '""') {


                $.confirm({
                    title: 'Dikkat',
                    content: response.data,
                    columnClass: 'col-md-12',
                    buttons: {
                        formSubmit: {
                            text: 'Tüm Koşulları ONAYLIYORUM',
                            btnClass: 'btn-blue',
                            keys: ['enter'],
                            action: function () {
                                $scope.saveKvkkContract();
                                //checkAdblocker();
                            }

                        },
                        Vazgeç: function () {
                            window.location.href = "Login/Logout";
                        }
                    },
                    scrollToPreviousElement: false,
                    scrollToPreviousElementAnimate: false,
                    onOpen: function () {

                    }
                });

                /* $.fancybox.open({
                     maxWidth: 600,
                     transitionEffect: "zoom-in-out",
                     maxHeight: 600,
                     fitToView: false,
                     width: '70%',
                     height: '70%',
                     autoSize: false,
                     closeClick: false,
                     openEffect: 'none',
                     closeEffect: 'none',
                     enableEscapeButton: false,
                     'hideOnOverlayClick': false,
                     'helpers': {
                         overlay: { closeClick: false } // prevents closing when clicking OUTSIDE fancybox
                     },
                     content: response.data,
                     'keys': {
                         close: null
                     },
                     afterLoad: function () {
                         //$.fancybox.defaults.closeBtn = false;
                         //$.fancybox.defaults.closeClickOutside = false;
                     },
                 });
 
                 $('.fancybox-close-small').attr("style", "display:none")*/
            }
            else {
                $scope.fireAnnouncementOpeningMessage(3);
            }

        });
    };


    $scope.getSuggestionProductList = function () {

        //$scope.fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Search/GetCookieValue",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.searchValues = response.data + "SearchProducts";
            var cookieValue = localStorage.getItem($scope.searchValues);
            if (cookieValue === null || cookieValue === '' || cookieValue === undefined) {
                //$scope.fireCustomLoading(false);
                $scope.getBalanceInformation();
            }
            else {
                var cookieArray = cookieValue.split(',');

                $.each(cookieArray, function (index, value) {
                    var newdate = new Date();
                    newdate.setDate(newdate.getDate() - 2);

                    if (new Date(value.split('|')[1]) < newdate) {
                        cookieValue = cookieValue.replace(value + ",", '');
                    }

                });

                cookieArray = cookieValue.split(',');
                var productCodes = '';
                $.each(cookieArray, function (index, value) {


                    if (value.split('|')[0] !== '') {
                        productCodes += "'" + value.split('|')[0] + "'" + ",";
                    }

                });


                $http({
                    method: "POST",
                    url: "/Home/GetSuggestionProducts",
                    headers: { "Content-Type": "Application/json;charset=utf-8" },
                    data: { productCodes: productCodes }
                }).then(function (response) {
                    $scope.suggestionList = response.data;
                    //$scope.fireCustomLoading(false);
                    $scope.getBalanceInformation();

                });
            }



        });
    };

    $(document).ready(function () {
        $scope.getCampaignList();
        $scope.getSuggestionProductList();
        //  $scope.fireAnnouncementOpeningMessage(3);

        $scope.checkKvkkContract();
        // Slider Revolution
        jQuery('#slider-rev').revolution({
            delay: 8000,
            startwidth: 1170,
            startheight: 300,

            hideThumbs: 250,
            navigationHAlign: "center",
            navigationVAlign: "bottom",
            navigationHOffset: 0,
            navigationVOffset: 20,
            soloArrowLeftHalign: "left",
            soloArrowLeftValign: "center",
            soloArrowLeftHOffset: 0,
            soloArrowLeftVOffset: 0,
            soloArrowRightHalign: "right",
            soloArrowRightValign: "center",
            soloArrowRightHOffset: 0,
            soloArrowRightVOffset: 0,
            touchenabled: "on",
            stopAtSlide: -1,

            stopAfterLoops: -1,
            dottedOverlay: "none",
            fullWidth: "on",
            spinned: "spinner5",
            shadow: 0,
            hideTimerBar: "on"//,
            //navigationStyle:"preview4"
        });
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