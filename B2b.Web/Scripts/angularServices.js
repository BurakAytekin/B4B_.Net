b2bApp.controller('LayoutController', function ($scope, $http, B2BServices) {



    $(document).ready(function () {

        $scope.controlRightPanel = false;
        $scope.showLeftPanel(-1);
        $scope.showRightPanel(-1);
        // Fade Menu
        $('#fadeLeft').on('click', function () {
            $('#leftContainer').removeClass('show');
            $('#fadeLeft').fadeOut(500);
        });
        // Fade Menu
        $('#fadeRight').on('click', function () {
            $('#rightContainer').removeClass('show-right');
            $('#rightContainer').removeClass('menu-active');
            $('#fadeRight').fadeOut(500);
        });
        //Right Menu Close Button
        $('#RightMenuClose').on('click', function () {
            $('#rightContainer').removeClass('show-right');
            $('#rightContainer').removeClass('menu-active');
            $('#fadeRight').fadeOut(500);
        });

        B2BServices.getBasketCount();

    });



    $scope.changControl = function (item) {
        if (item.Quantity < item.MinOrder || item.Quantity === "Nan" || item.Quantity === undefined) {
            item.Quantity = item.MinOrder;
            iziToast.error({
                message: 'Minumum sipariş miktarından az sipariş veremezsiniz.',
                position: 'topCenter'
            });
        }
        else {

            var qty = parseFloat(item.Quantity) - item.MinOrder;
            var k = qty / item.RisingQuantity;
            if (qty % item.RisingQuantity == 0)
                qty = parseInt(k) * item.RisingQuantity;
            else
                qty = (parseInt(Math.floor(k)) + 1) * item.RisingQuantity;

            item.Quantity = qty + item.MinOrder;


            item.SendDisabled = true;

        }
    }

    $scope.showLeftPanel = function (type) {
        $http({
            method: "POST",
            url: "/Home/GetProductOfDayList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: (type === -1 ? 0 : type) }
        }).then(function (response) {
            $scope.productOfDayList = response.data;
            if ($scope.productOfDayList.length === 0)
                $('.menu-button_container-left').addClass("hidden");
            else
                $('.menu-button_container-left').removeClass("hidden");

            if (type !== -1) {
                $('#leftContainer').addClass('show');
                $('#fadeLeft').fadeIn(400);
            }

        });

    }



    $scope.showRightPanel = function (type) {
        $http({
            method: "POST",
            url: "/Home/GetNotificationsList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: (type === -1 ? 0 : type) }
        }).then(function (response) {
            $scope.notificationList = response.data;
            if ($scope.notificationList.length === 0)
                $('.menu-button_container-right').addClass("hidden");
            else
                $('.menu-button_container-right').removeClass("hidden");

            if (type !== -1) {
                //$('#rightContainer').addClass('show-right');
                //$('#rightContainer').addClass('menu-active');
                //$('#fadeRight').fadeIn(400);
                $scope.controlRightPanel = true;
            }
        });

    }

    $scope.closeNoti = function (item) {

        $http({
            method: "POST",
            url: "/Home/CloseNotification",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { item: item }
        }).then(function (response) {
            $scope.showRightPanel(1);
        });

    };

    $scope.sendOrder = function (item) {
        var message = '';

        $scope.selectedProductOfDay = angular.copy(item);

        if (item.TotalQuantity !== 0 && (item.SaledQuantity + item.Quantity) > item.TotalQuantity) {

            message = 'Kampanya adetinden fazla istemektesiniz. İşleme devam etmeniz durumunda ' + (item.TotalQuantity - item.SaledQuantity) + ' adet sipariş geçilecektir. İşleme devam etmek istediğinize emin misiniz ?'
            $scope.selectedProductOfDay.Quantity = ($scope.selectedProductOfDay.TotalQuantity - $scope.selectedProductOfDay.SaledQuantity);
        }
        else
            message = $scope.selectedProductOfDay.Product.Code + ' kodlu üründen ' + $scope.selectedProductOfDay.Quantity + ' adet sipariş geçilecektir. İşleme devam etmek istediğinize emin misiniz ?'

        $.confirm({
            title: 'Günün Ürünü',
            content: message,
            buttons: {
                formSubmit: {
                    text: 'Gönder',
                    btnClass: 'btn-blue',
                    action: function () {
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Home/SendProductOfDayOrder",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: { productOfDay: $scope.selectedProductOfDay }

                        }).then(function (response) {
                            fireCustomLoading(false);
                            $('#leftContainer').removeClass('show');
                            $('#fadeLeft').fadeOut(500);
                            $scope.showLeftPanel(1);
                            iziToast.show({
                                message: response.data.Message,
                                position: 'topCenter',
                                color: response.data.Color,
                                icon: response.data.Icon
                            });
                        });
                    }

                },
                Vazgeç: function () {
                    //close
                }
            },
            scrollToPreviousElement: false,
            scrollToPreviousElementAnimate: false,
            onOpen: function () {

            }
        });


        //confirm

    }

    $scope.$on('ngRepeatProductOfDayFinished', function (ngRepeatProductOfDayFinishedEvent) {
        firePriceFormatNumberOnly();

        $http({
            method: "POST",
            url: "/Search/GetCookieValue",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {

            var cookieValue = $.cookie(response.data + 'Information');
            if (cookieValue === null || cookieValue === '' || cookieValue === undefined) {
                $.cookie((response.data + 'Information'), "true", { expires: 1 });
                if ($scope.productOfDayList.length > 0) {
                    $('#leftContainer').addClass('show');

                    $('#fadeLeft').fadeIn(400);
                }

            }
        });

    });


    $scope.$on('ngRepeatProductOfDayFinished2', function (ngRepeatProductOfDayFinished2Event) {
        firePriceFormatNumberOnly();


        $http({
            method: "POST",
            url: "/Search/GetCookieValue",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {

            var cookieValue = $.cookie(response.data + 'Information');
            if (cookieValue === null || cookieValue === '' || cookieValue === undefined) {
                $.cookie((response.data + 'Information'), "true", { expires: 1 });

                //if ($scope.notificationList.length > 0) {
                //    $('#rightContainer').addClass('show-right');
                //    $('#fadeRight').fadeIn(400);
                //}
            }
        });

        if ($scope.controlRightPanel) {
            $('#rightContainer').addClass('show-right');
            $('#fadeRight').fadeIn(400);
            $('#rightContainer').addClass('menu-active');
        }
    });


}).directive('onFinishRender', function ($timeout) {
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