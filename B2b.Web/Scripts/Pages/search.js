
b2bApp.controller('IndexController', function ($scope, $http, $compile, $rootScope, B2BServices) {
    // #region Veriables
    $scope.selectedManufacturer = "";
    $scope.selectedProductGroup1 = "";
    $scope.selectedProductGroup2 = "";
    $scope.selectedProductGroup3 = "";
    $scope.selectedProductGroup4 = "";

    $scope.selectedvehicleBrand = "";
    $scope.selectedvehicleModel = "";
    $scope.chkCampanign = false;
    //$scope.chkNewArrival = false;
    $scope.chkComparison = false;
    $scope.chkNewProduct = false;
    $scope.chkOnQuantity = false;
    $scope.chkOnWay = false;
    $scope.chkIsPicture = false;
    $scope.searchResult = "";
    $scope.dataCount = 0;
    $scope.vDataCount = 24;
    $scope.repeatCount = 0;
    $scope.lock = false;
    $scope.isSearchBanner = false;
    // #region CustomCalculate
    $scope.calculatePriceList = 0;
    $scope.calculateDisc1 = 0;
    $scope.calculateDisc2 = 0;
    $scope.calculateDisc3 = 0;
    $scope.calculateDisc4 = 0;
    $scope.calculatePriceNet = 0;
    $scope.calculateQty = 1;
    $scope.calculateTotal = 0;
    $scope.searchHide = 1;
    $scope.searchTab = 1;

    // #endregion
    $scope.isTableDisplay = true;



    //categorydiagram



    //$scope.selectProductGroup1 = function (item) {
    //    $scope.selectedProductGroup1 = angular.copy(item);
    //    console.log(item);
    //    $scope.getProductGroup2List($scope.selectedProductGroup1.Name);
    //    $scope.searchHide = 2;
    //    $scope.selectedProductGroup2 = {};
    //    $scope.selectedProductGroup3 = {};
    //    $scope.selectedProductGroup4 = {};
    //    $scope.txtGeneralSearchText = '';
    //    $scope.searchResult = '';

    //    if ($scope.getProductGroup2List.length == 0) {
    //        $scope.search(1);
    //        $scope.searchHide = 5;
    //        if ($scope.getProductGroup3List.length == 0) {
    //            $scope.search(1);
    //            $scope.searchHide = 5;
    //            console.log("aradi")


    //        }


    //    }
    //}

    //$scope.selectProductGroup2 = function (item) {
    //    $scope.selectedProductGroup2 = angular.copy(item);

    //    //$scope.selectedProductGroup2($scope.selectedProductGroup1.Name);

    //    $scope.searchHide = 3;
    //    $scope.getProductGroup3List($scope.selectedProductGroup1.Name, $scope.selectedProductGroup2.Name);
    //    $scope.selectedProductGroup3 = {};
    //            $scope.selectedProductGroup4 = {};

    //    $scope.txtGeneralSearchText = '';
    //    $scope.searchResult = '';
    //    console.log($scope.getProductGroup3List.length);
    //    if ($scope.getProductGroup3List.length == 0) {
    //        $scope.search(1);
    //        $scope.searchHide = 5;
    //        console.log("aradi")


    //    }
    //}

    //$scope.selectProductGroup3 = function (item) {
    //    $scope.selectedProductGroup3 = angular.copy(item);
    //    $scope.selectedProductGroup4 = {};

    //    $scope.getProductGroup4List($scope.selectedProductGroup1.Name, $scope.selectedProductGroup2.Name, $scope.selectedProductGroup3.Name);

    //    //$scope.selectedProductGroup2($scope.selectedProductGroup1.Name);
    //    $scope.searchHide = 4;
    //    $scope.searchResult = '';

    //    if ($scope.getProductGroup3List.length == 0) {
    //        $scope.search(1);
    //        $scope.searchHide = 5;
    //        if ($scope.getProductGroup3List.length == 0) {
    //            $scope.search(1);
    //            $scope.searchHide = 5;
    //            console.log("aradi")


    //        }

    //    }
    //}

    //$scope.selectProductGroup4 = function (item) {
    //    $scope.selectedProductGroup4 = angular.copy(item);

    //    //$scope.selectedProductGroup2($scope.selectedProductGroup1.Name);
    //    $scope.searchHide = 5;
    //    $scope.searchResult = '';

    //    $scope.txtGeneralSearchText = '';
    //    $scope.search(1);



    //}


    $scope.selectProductGroup1 = function (item) {
        $scope.selectedProductGroup1 = angular.copy(item);
        $scope.getProductGroup2List($scope.selectedProductGroup1.Name);


        $scope.selectedProductGroup2 = {};
        $scope.selectedProductGroup3 = {};
        $scope.selectedProductGroup4 = {};

        $scope.txtGeneralSearchText = '';
        $scope.searchResult = '';
        $scope.searchHide = 2;


    }

    $scope.selectProductGroup2 = function (item) {
        $scope.selectedProductGroup2 = angular.copy(item);

        //$scope.selectedProductGroup2($scope.selectedProductGroup1.Name);
        $scope.getProductGroup3List($scope.selectedProductGroup1.Name, $scope.selectedProductGroup2.Name);
        $scope.selectedProductGroup3 = {};
        $scope.selectedProductGroup4 = {};

        $scope.txtGeneralSearchText = '';
        $scope.searchResult = '';
        $scope.searchHide = 3;

    }

    $scope.selectProductGroup3 = function (item) {
        $scope.selectedProductGroup3 = angular.copy(item);

        //$scope.selectedProductGroup2($scope.selectedProductGroup1.Name);
        $scope.searchTab = 4;

        $scope.searchHide = 5;
        $scope.search(1);

        $scope.selectedProductGroup4 = {};
        $scope.txtGeneralSearchText = '';
        $scope.searchResult = '';

    }

    $scope.selectProductGroup4 = function (item) {
        $scope.selectedProductGroup4 = angular.copy(item);

        //$scope.selectedProductGroup2($scope.selectedProductGroup1.Name);
        $scope.searchHide = 5;
        $scope.searchResult = '';
        $scope.search(1);
        $scope.txtGeneralSearchText = '';
        $scope.searchTab = 5;
    }



    //diagramfinish

    $scope.titleCampaignMinOrder = function ($event, productItem, productId) {
        var e = $event.currentTarget;

        if (!$(e).hasClass("tooltipstered")) {

            $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            $scope.fireTooltips(e, true);

            $http({
                method: "POST",
                url: "/Partial/CampaignMinOrderView",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { productItem: productItem, productId: productId }
            }).then(function (viewHTML) {

                if (viewHTML.data !== "") {
                    $(e).tooltipster('content', viewHTML.data);
                }
                else {
                    $(e).tooltipster('destroy', true);
                }
            });
        }

    };
    $scope.getProductGroup4List = function (group1Name, group2Name, group3Name) {
        fireCustomPanelLoading(true);
        $scope.selectedProductGroup4 = {};
        $scope.searchTab = 4;
        $scope.searchHide = 5;

        $scope.search(1);
        $http({
            method: "POST",
            url: "/Search/GetProductGroup4List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { group1Name: group1Name, group2Name: group2Name, group3Name: group3Name }

        }).then(function (response) {
            $scope.productGroup4List = response.data;
            fireCustomPanelLoading(false);

            if ($scope.productGroup4List.length === 0) {

            }
            else if ($scope.productGroup4List.length === 1) {

                $scope.selectedProductGroup4 = $scope.productGroup3List[0];
                $scope.selectProductGroup4($scope.productGroup4List[0]);
                // $scope.searchHide = 4;
                // $scope.search(0);
            }
        });
    };

    // #endregion
    $scope.getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };
    // #region SearchAndSearchResultfunctions
    $scope.keypressEvent = function (e) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 13) {
            $scope.search(1);
        }
    };

    $scope.changeDisplaStyle = function () {
        $scope.isTableDisplay = !$scope.isTableDisplay;
        $scope.search(1);
    }
    function fireLodingBar(status) {
        if (status)
            $("#SearchLoadingBar").show();
        else
            $("#SearchLoadingBar").hide();
    }
    //$scope.search = function (tb) {  // eski tabloyu temizle
    //    if ($scope.lock)
    //        return;
    //    fireLodingBar(true);
    //    $scope.lock = true;

    //    if (tb === 1) {
    //        $scope.searchResult = '';
    //        $scope.dataCount = 0;
    //        $scope.repeatCount = 0;
    //    }
    //    var dataCount = $scope.dataCount;

    //    var manufacturer = null;
    //    var vehicleBrand = null;
    //    var vehicleModel = null;
    //    var productGroup1 = null;
    //    var productGroup2 = null;
    //    var productGroup3 = null;
    //    var campaign = false;
    //    var newArrival = false;
    //    var comparsionProduct = false;
    //    var newProduct = false;
    //    var onQuantity = false;
    //    var onWay = false;
    //    var image = false;
    //    var t9Text = '';

    //    var index;
    //    index = $("#manufacturer-select")[0].selectedIndex;
    //    if (index > 0) manufacturer = $("#manufacturer-select").val().toString();

    //    index = $("#vehicleBrand-select")[0].selectedIndex;
    //    if (index > 0) {
    //        vehicleBrand = $("#vehicleBrand-select").val();
    //        index = $("#vehicleModel-select")[0].selectedIndex;
    //        if (index > 0) vehicleModel = $("#vehicleModel-select").val();
    //    }

    //    index = $("#productGroup-select")[0].selectedIndex;
    //    if (index > 0) {
    //        productGroup1 = $("#productGroup-select").val();
    //        index = $("#productGroup2-select")[0].selectedIndex;
    //        if (index > 0) {
    //            productGroup2 = $("#productGroup2-select").val();
    //            index = $("#productGroup3-select")[0].selectedIndex;
    //            if (index > 0) productGroup3 = $("#productGroup3-select").val();
    //        }
    //    }
    //    campaign = $scope.chkCampanign;
    //    //newArrival = $scope.chkNewArrival;
    //    comparsionProduct = $scope.chkComparison;
    //    newProduct = $scope.chkNewProduct;
    //    onQuantity = $scope.chkOnQuantity;
    //    onWay = $scope.chkOnWay;
    //    image = $scope.chkIsPicture;

    //    if ($scope.txtGeneralSearchText !== '') t9Text = $scope.txtGeneralSearchText;

    //    if (t9Text !== '') {
    //        var cookieValue = $.cookie(searchCookieValue);
    //        if (cookieValue === null || cookieValue === '' || cookieValue === undefined)
    //            $.cookie(searchCookieValue, t9Text + ",", { expires: 7 });
    //        else {
    //            if (cookieValue.indexOf(t9Text + ",") === -1) {
    //                $.cookie(searchCookieValue, (cookieValue + t9Text + ","), { expires: 7 });
    //            }
    //        }
    //    }

    //    $http({
    //        method: "POST",
    //        url: "/Search/SearchProduct",
    //        headers: { "Content-Type": "Application/json;charset=utf-8" },
    //        data: { dataCount: dataCount, manufacturer: manufacturer, vehicleBrand: vehicleBrand, vehicleModel: vehicleModel, productGroup1: productGroup1, productGroup2: productGroup2, productGroup3: productGroup3, t9Text: t9Text, campaign: campaign, newArrival: newArrival, newProduct: newProduct, comparsionProduct: comparsionProduct, onQuantity: onQuantity, onWay: onWay, image: image }

    //    }).then(function (response) {
    //        if (dataCount > 0) {
    //            $scope.searchResult = $scope.searchResult.concat(response.data);
    //        }
    //        else
    //            $scope.searchResult = response.data;
    //        $scope.dataCount = $scope.dataCount + 24;
    //        $scope.lock = false;

    //        $scope.addSearchCookie();




    //        $scope.resultCount = response.data.length;
    //        if ($scope.searchResult.length === 0) {
    //            iziToast.error({
    //                //title: 'Hata',
    //                message: 'Aradığınız kriterlerde sonuç bulunamadı',
    //                position: 'topCenter'
    //            });
    //        }

    //        //if (response.data.length <= 0 || response.data.length < 24) {
    //        //    //if ($scope.isTableDisplay)
    //        //        $('#tbResult').infiniteScrollHelper('destroy');
    //        //   // else
    //        //        $('#divSearchCatalog').infiniteScrollHelper('destroy');
    //        //    if (response.data.length === 0) {
    //        //        iziToast.error({
    //        //            //title: 'Hata',
    //        //            message: 'Aradığınız kriterlerde sonuç bulunamadı',
    //        //            position: 'topCenter'
    //        //        });
    //        //    }
    //        //}

    //        fireLodingBar(false);
    //    });

    //};
    $scope.search = function (tb) {  // eski tabloyu temizle
        if ($scope.lock)
            return;
        fireLodingBar(true);
        $scope.lock = true;

        if (tb === 1) {
            $scope.searchResult = {};
            $scope.dataCount = 0;
            $scope.repeatCount = 0;
        }
        var dataCount = $scope.dataCount;

        var manufacturer = null;
        var vehicleBrand = null;
        var vehicleModel = null;
        var productGroup1 = null;
        var productGroup2 = null;
        var productGroup3 = null;
        var productGroup4 = null;
        var campaign = false;
        var newArrival = false;
        var comparsionProduct = false;
        var newProduct = false;
        var onQuantity = false;
        var onWay = false;
        var image = false;
        var t9Text = '';

        //var index;
        //index = $("#manufacturer-select")[0].selectedIndex;
        //if (index > 0) manufacturer = $("#manufacturer-select").val().toString();

        //index = $("#vehicleBrand-select")[0].selectedIndex;
        //if (index > 0) {
        //    vehicleBrand = $("#vehicleBrand-select").val();
        //    index = $("#vehicleModel-select")[0].selectedIndex;
        //    if (index > 0) vehicleModel = $("#vehicleModel-select").val();
        //}

        //index = $("#productGroup-select")[0].selectedIndex;
        //if (index > 0) {
        //    productGroup1 = $("#productGroup-select").val();
        //    index = $("#productGroup2-select")[0].selectedIndex;
        //    if (index > 0) {
        //        productGroup2 = $("#productGroup2-select").val();
        //        index = $("#productGroup3-select")[0].selectedIndex;
        //        if (index > 0) productGroup3 = $("#productGroup3-select").val();
        //    }
        //}

        productGroup1 = $scope.selectedProductGroup1.Name;
        productGroup2 = $scope.selectedProductGroup2.Name;
        productGroup3 = $scope.selectedProductGroup3.Name;
        productGroup4 = $scope.selectedProductGroup4.Name;

        campaign = $scope.chkCampanign;
        //newArrival = $scope.chkNewArrival;
        comparsionProduct = $scope.chkComparison;
        newProduct = $scope.chkNewProduct;
        onQuantity = $scope.chkOnQuantity;
        onWay = $scope.chkOnWay;
        image = $scope.chkIsPicture;

        if ($scope.txtGeneralSearchText !== '') t9Text = $scope.txtGeneralSearchText;

        if (t9Text !== '') {
            var cookieValue = $.cookie(searchCookieValue);
            if (cookieValue === null || cookieValue === '' || cookieValue === undefined)
                $.cookie(searchCookieValue, t9Text + ",", { expires: 7 });
            else {
                if (cookieValue.indexOf(t9Text + ",") === -1) {
                    $.cookie(searchCookieValue, (cookieValue + t9Text + ","), { expires: 7 });
                }
            }
        }

        $http({
            method: "POST",
            url: "/Search/SearchProduct",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { dataCount: dataCount, manufacturer: manufacturer, vehicleBrand: vehicleBrand, vehicleModel: vehicleModel, productGroup1: productGroup1, productGroup2: productGroup2, productGroup3: productGroup3, productGroup4: productGroup4, t9Text: t9Text, campaign: campaign, newArrival: newArrival, newProduct: newProduct, comparsionProduct: comparsionProduct, onQuantity: onQuantity, onWay: onWay, image: image }

        }).then(function (response) {
            if (dataCount > 0) {
                $scope.searchResult = $scope.searchResult.concat(response.data);
            }
            else
                $scope.searchResult = response.data;
            $scope.dataCount = $scope.dataCount + 24;
            $scope.lock = false;

            $scope.addSearchCookie();




            $scope.resultCount = response.data.length;
            if ($scope.searchResult.length === 0) {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Aradığınız kriterlerde sonuç bulunamadı',
                    position: 'topCenter'
                });
            }

            //if (response.data.length <= 0 || response.data.length < 24) {
            //    //if ($scope.isTableDisplay)
            //        $('#tbResult').infiniteScrollHelper('destroy');
            //   // else
            //        $('#divSearchCatalog').infiniteScrollHelper('destroy');
            //    if (response.data.length === 0) {
            //        iziToast.error({
            //            //title: 'Hata',
            //            message: 'Aradığınız kriterlerde sonuç bulunamadı',
            //            position: 'topCenter'
            //        });
            //    }
            //}

            fireLodingBar(false);
        });

    };
    $scope.addSearchCookie = function () {
        if ($scope.searchResult.length <= 5) {

            angular.forEach($scope.searchResult, function (item) {
                if (item.Code !== '') {
                    var d = new Date();
                    var strDate = d.getFullYear() + "/" + (d.getMonth() + 1) + "/" + d.getDate();

                    var cookieValue = localStorage.getItem($scope.searchValues);
                    if (cookieValue === null || cookieValue === '' || cookieValue === undefined)
                        localStorage.setItem($scope.searchValues, item.Code + "|" + strDate + ",");
                    else {
                        var cookieArray = cookieValue.split(',');

                        $.each(cookieArray, function (index, value) {
                            var newdate = new Date();
                            newdate.setDate(newdate.getDate() - 2);

                            if (new Date(value.split('|')[1]) < newdate) {
                                cookieValue = cookieValue.replace(value + ",", '');
                            }

                        });
                        localStorage.setItem($scope.searchValues, cookieValue);
                        if (cookieValue.indexOf(item.Code + "|" + strDate + ",") === -1) {
                            localStorage.setItem($scope.searchValues, (cookieValue + item.Code + "|" + strDate + ","));
                        }
                    }
                }
            });
        }
    }


    $scope.detailFormatter = function (index, id, groupId, productCode, searchItem) {


        $http({
            method: "POST",
            url: "/Search/LogSearchOpenDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, productCode: productCode }

        });

        var colspanValue = $('#tr' + id)[0].cells.length;

        if ($('#trChild' + id).length > 0) {
            $('#trChild' + id).remove();
            //glyphicon glyphicon-plus icon-plus
            $('#ic' + id).removeClass("glyphicon glyphicon-minus icon-minus").addClass("glyphicon glyphicon-plus icon-plus");
        }
        else {


            var text = '';
            text = '<tr id="trChild' + id + '" class="detail-view"><td colspan="' + colspanValue + '" class="text-center"><div id="divDetail' + productCode + '" class="detail-view-loader"><i class="fa fa-spinner fa-spin fa-3x fa-fw" ></i></div></td></tr>';
            $.ajax({
                type: "GET",
                url: "/Search/ProductDetail?productId=" + id + "&groupId=" + groupId + "&productCode=" + productCode + "&campaignType=" + parseInt(searchItem.Campaign.Type),
                data: '',
                datatype: "html",
                beforeSend: function () {
                    $('#tr' + id).closest('tr').after(text);
                },
                success: function (viewHTML) {
                    $('#trChild' + id).remove();
                    var html = '<tr id="trChild' + id + '" class="detail-view"><td colspan="' + colspanValue + '">' + viewHTML + '</td></tr>';

                    $('#tr' + id).closest('tr').after(html);
                    $compile(html)($scope);
                },
                error: function (errorData) {
                    $('#tr' + id).closest('tr').after('<tr id="trChild' + id + '" class="detail-view"><td colspan="' + colspanValue + '">' + errorData + '</td></tr>');
                    console.log(errorData);
                }
                //complete: function () {
                //    $scope.markFunction($scope.txtGeneralSearchText);
                //}
            });
            //$('#tr' + id).closest('tr').after(text);
            $('#ic' + id).removeClass("glyphicon glyphicon-plus icon-plus").addClass("glyphicon glyphicon-minus icon-minus");
        }
    };
    // Ng repeat işlemi bittiği zaman bu method çalışır
    $scope.$on('ngRepeatsearchResultFinished', function (ngRepeatSearchResultFinishedEvent) {

        if ($scope.resultCount === 24) {
            if ($scope.isTableDisplay) {
                $('#tbResult').infiniteScrollHelper({
                    bottomBuffer: 150,
                    triggerInitialLoad: true,
                    loadingClass: false,
                    loadMore: function (page, done) {
                        if ($scope.repeatCount > 0 && $scope.vDataCount === 24 && $scope.lock === false) {
                            $scope.search(0);
                        }
                        $scope.repeatCount++;
                        done();
                    }
                });
            } else {
                $('#divSearchCatalog').infiniteScrollHelper({
                    bottomBuffer: 150,
                    triggerInitialLoad: true,
                    loadingClass: false,
                    loadMore: function (page, done) {
                        if ($scope.repeatCount > 0 && $scope.vDataCount === 24 && $scope.lock === false) {
                            $scope.search(0);
                        }
                        $scope.repeatCount++;
                        done();
                    }
                });
            }
        }



        if ($scope.resultCount < 24) {
            //if ($scope.isTableDisplay)
            $('#tbResult').infiniteScrollHelper('destroy');
            // else
            $('#divSearchCatalog').infiniteScrollHelper('destroy');
            if ($scope.searchResult.length === 0) {
                iziToast.error({
                    //title: 'Hata',
                    message: 'Aradığınız kriterlerde sonuç bulunamadı',
                    position: 'topCenter'
                });
            }
        }


        $scope.fireTooltips('.tooltip-box', false);
        $scope.fireTooltips('.tooltip-name', false);
        $scope.markFunction($scope.txtGeneralSearchText);
    });
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
        if (o)
            if ($(e).is(':hover'))
                $(e).tooltipster('open');


    };
    $scope.markFunction = function (keyword) {
        var options = "separateWordSearch";

        $(".context").unmark({
            done: function () {
                $(".context").mark(keyword, options);
            }
        });

    };
    $scope.fireProductSlider = function (id, code) {
        $http({
            method: "POST",
            url: "/Search/GetProductImages",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }

        }).then(function (response) {

            var imgListArray = [];
            $.each(response.data, function () {
                var img = {
                    src: this.Path,
                    opts: {
                        caption: code
                    }

                };

                imgListArray.push(img);
            });

            $.fancybox.open(imgListArray, {
                loop: true,
                hash: "test",
                errorTpl: '<div class="fancybox-error"><p>İstenen içerik yüklenemiyor. <br /> Lütfen daha sonra tekrar deneyiniz.<p></div>'
            });
            if ($('.fancybox-button--thumbs').length > 0)
                $('.fancybox-button--thumbs').click();


        });
    };
    // #endregion
    // #region Manufacturerfuncitons
    $scope.getManufacturerList = function () {
        $http({
            method: "POST",
            url: "/Search/GetManufaturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}


        }).then(function (response) {
            $scope.manufacturerList = response.data;
            $scope.selectedManufacturer = response.data[0].Name;
            $scope.getProductGroup1List();

        });
    };
    $scope.$on('ngRepeatManufacturerFinished', function (ngRepeatManufacturerFinishedEvent) {

        /*Ng repeat işlemi bittiği zaman bu method çalışır */


        $('.selectbox-manufacturer')[0].sumo.reload();

        $('.selectbox-manufacturer').SumoSelect(
            {
                csvDispCount: 1,
                captionFormat: '{0} Üretici Seçili',
                floatWidth: 640,
                okCancelInMulti: true,
                triggerChangeCombined: false,
                forceCustomRendering: false,
                selectAll: false,
                search: true,
                searchText: 'Üretici seçiniz.'
            }
        );

        $('.selectbox-manufacturer').on('sumo:closed', function (sumo) {

        });

        //console.log("üretici bitti");
        $scope.urlSearchControl();

    });
    // #endregion
    // #region ProductGroupsfunctions
    // #region ProductMainGroupfunction
    $scope.getProductGroup1List = function () {
        $scope.searchHide = 1;
        $scope.searchTab = 1;

        $scope.selectedProductGroup2 = {};
        $scope.selectedProductGroup3 = {};
        $scope.selectedProductGroup4 = {};
        fireCustomPanelLoading(true);
        $scope.clear();
        $http({
            method: "POST",
            url: "/Search/GetProductGroup1List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {

            $scope.productGroup1List = response.data;
            fireCustomPanelLoading(false);
            if ($scope.productGroup1List.length === 1) {

                $scope.selectedProductGroup1 = $scope.productGroup1List[0];
                $scope.selectProductGroup1($scope.productGroup1List[0]);

            }

            //$scope.selectedProductGroup1 = response.data[0].Name;
            //$scope.getVehicleBrandList();
        });
    };
    $scope.selectProductGroupChanged = function () {
        //if ($('.selectbox-productgroup1').val() !== "Seçiniz") {
        //    $("#productGroup2-select").prop('disabled', false);
        //    $scope.selectedProductGroup1 = $('.selectbox-productgroup1').val();
        //    $scope.getProductGroup2List($('.selectbox-productgroup1').val());
        //    $scope.selectedProductGroup2 = "Seçiniz";
        //    $scope.selectedProductGroup3 = "Seçiniz";

        //}
    };
    $scope.$on('ngRepeatGroup1Finished', function (ngRepeatGroup1FinishedEvent) {
        /////*Ng repeat işlemi bittiği zaman bu method çalışır */
        ////$('.selectbox-productgroup1')[0].sumo.reload();
        ////$('.selectbox-productgroup1').SumoSelect();
        ////$scope.selectedProductGroup1 = "";
        ////$('.selectbox-productgroup1').on('sumo:closed', function (sumo) {

        ////    if ($('.selectbox-productgroup1').val() !== "Seçiniz") {
        ////        $scope.selectedProductGroup1 = $('.selectbox-productgroup1').val();
        ////        $scope.getProductGroup2List($('.selectbox-productgroup1').val());
        ////        $scope.selectedProductGroup2 = "Seçiniz";
        ////        $scope.selectedProductGroup3 = "Seçiniz";
        ////    }
        ////});
    });
    // #endregion
    // #region ProductSubGroup2function
    $scope.getProductGroup2List = function (group1Name) {
        fireCustomPanelLoading(true);
        $scope.selectedProductGroup2 = {};
        $scope.searchTab = 2;


        $http({
            method: "POST",
            url: "/Search/GetProductGroup2List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { group1Name: group1Name }

        }).then(function (response) {
            $scope.productGroup2List = response.data;
            fireCustomPanelLoading(false);
            if ($scope.productGroup2List.length === 0) {
                $scope.searchHide = 5;
                $scope.search(1);
            }
            else if ($scope.productGroup2List.length === 1) {

                $scope.selectedProductGroup2 = $scope.productGroup2List[0];
                $scope.selectProductGroup2($scope.productGroup2List[0]);
            }

            //$scope.selectedProductGroup2 = response.data[0].Name;
        });
    };
    $scope.selectProductGroup2Changed = function () {
        //if ($('.selectbox-productgroup2').val() !== "Seçiniz" && $scope.selectedProductGroup1 !== "Seçiniz") {
        //    $scope.getProductGroup3List($scope.selectedProductGroup1, $('.selectbox-productgroup2').val());

        //}
    };
    $scope.$on('ngRepeatGroup2Finished', function (ngRepeatGroup2FinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        //$("#productGroup2-select").prop('disabled', false);
        //$('.selectbox-productgroup2').SumoSelect();
        //$('.selectbox-productgroup2')[0].sumo.reload();
        //$('.selectbox-productgroup2').on('sumo:closed', function (sumo) {

        //    if ($('.selectbox-productgroup2').val() !== "Seçiniz" && $scope.selectedProductGroup1 !== "Seçiniz") {
        //        $("#productGroup3-select").prop('disabled', false);
        //        $scope.getProductGroup3List($scope.selectedProductGroup1, $('.selectbox-productgroup2').val());

        //    } else {
        //        $('.selectbox-productgroup2').val("Seçiniz");
        //    }

        //});

    });
    // #endregion
    // #region ProductSubGroup3function
    $scope.getProductGroup3List = function (group1Name, group2Name) {
        fireCustomPanelLoading(true);
        $scope.selectedProductGroup3 = {};
        $scope.searchTab = 3;

        $http({
            method: "POST",
            url: "/Search/GetProductGroup3List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { group1Name: group1Name, group2Name: group2Name }

        }).then(function (response) {
            $scope.productGroup3List = response.data;
            fireCustomPanelLoading(false);
            if ($scope.productGroup3List.length === 0) {
                $scope.searchHide = 5;

                $scope.search(1);
            }
            else if ($scope.productGroup3List.length === 1) {

                $scope.selectedProductGroup3 = $scope.productGroup3List[0];
                $scope.selectProductGroup3($scope.productGroup3List[0]);
                // $scope.searchHide = 4;
                // $scope.search(0);
            }
        });
    };
    $scope.selectProductGroup3Changed = function () {

    };
    $scope.$on('ngRepeatGroup3Finished', function (ngRepeatGroup3FinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        //$scope.selectedProductGroup3 = "Seçiniz";
        //$("#productGroup3-select").prop('disabled', false);
        //$('.selectbox-productgroup3').SumoSelect();
        //$('.selectbox-productgroup3')[0].sumo.reload();

    });
    $scope.getProductGroup4List = function (group1Name, group2Name, group3Name) {
        fireCustomPanelLoading(true);
        $scope.selectedProductGroup4 = {};
        $scope.searchTab = 4;

        $http({
            method: "POST",
            url: "/Search/GetProductGroup4List",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { group1Name: group1Name, group2Name: group2Name, group3Name: group3Name }

        }).then(function (response) {
            $scope.productGroup4List = response.data;
            fireCustomPanelLoading(false);
            if ($scope.productGroup4List.length === 0) {
                $scope.searchHide = 5;

                $scope.search(1);
            }
            else if ($scope.productGroup4List.length === 1) {

                $scope.selectedProductGroup4 = $scope.productGroup3List[0];
                $scope.selectProductGroup4($scope.productGroup4List[0]);
                // $scope.searchHide = 4;
                // $scope.search(0);
            }
        });
    };

    // #endregion
    // #endregion
    $scope.urlSearchControl = function () {

        var q = '';
        if (window.location.href.indexOf('?') !== -1) {
            q = window.location.href.slice(window.location.href.indexOf('?') + 1);
            q = q.split(' ').join('');
        }

        if (q !== '') {
            $scope.fireSliderBannerSearch(q);

        }

    }

    // #region VehicleBrandfunction
    $scope.getVehicleBrandList = function () {
        $http({
            method: "POST",
            url: "/Search/GetVehicleBrandList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.vehicleBrandlList = response.data;
            $scope.selectedvehicleBrand = response.data[0].Brand;
            fireCustomPanelLoading(false);
        });
    };
    $scope.selectVehicleBrandChanged = function () {
        if ($('.selectbox-vehicleBrand').val() !== "Seçiniz") {
            $scope.getVehicleModelList($('.selectbox-vehicleBrand').val());
            $(".selectbox-vehicleModel").prop('disabled', false);

        } else {
            $('.selectbox-vehicleBrand').val("Seçiniz");
        }
    };
    $scope.$on('ngRepeatVehicleBrandFinished', function (ngRepeatVehicleBrandFinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        $scope.selectedvehicleModel = "Seçiniz";
        $('.selectbox-vehicleBrand')[0].sumo.reload();
        $('.selectbox-vehicleBrand').SumoSelect();
        $('.selectbox-vehicleBrand').on('sumo:closed', function (sumo) {

            if ($('.selectbox-vehicleBrand').val() !== "Seçiniz") {
                $scope.getVehicleModelList($('.selectbox-vehicleBrand').val());

            } else {
                $('.selectbox-vehicleBrand').val("Seçiniz");
            }

        });

    });
    // #endregion
    // #region VehicleModelfunction
    $scope.getVehicleModelList = function (brand) {
        $http({
            method: "POST",
            url: "/Search/GetVehicleModelList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { brand: brand }

        }).then(function (response) {
            $scope.vehicleModelList = response.data;
            $scope.selectedvehicleModel = response.data[0].Model;
        });
    };
    $scope.$on('ngRepeatVehicleModelFinished', function (ngRepeatVehicleModelFinishedEvent) {
        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        $(".selectbox-vehicleModel").prop('disabled', false);
        $('.selectbox-vehicleModel').SumoSelect({
            csvDispCount: 1,
            // captionFormat: '{0} selected',
            floatWidth: 640,
            okCancelInMulti: true,
            triggerChangeCombined: false,
            forceCustomRendering: false,
            selectAll: false,
            search: true
        });
        $('.selectbox-vehicleModel')[0].sumo.reload();
    });
    // #endregion
    // #region AddBasket Function
    //type=0 normal gridden gelen 1:katalog görünümünden gelen 2:Alternatif için  3:Mobilden açılan modal içi ve detaylı sepete ekle
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


    // #endregion
    // #region CustomMethod
    $scope.getCustomAddBasket = function (product) {

        $.ajax({
            type: "GET",
            url: "/Search/CustomAddBasket?productId=" + product.Id + "&productCode=" + product.Code,
            data: '',
            datatype: "html",
            beforeSend: function () {
                $scope.fireModal(true, "#modal-text", "Sepete Ekle", "Load", "", false, "", "");
                //function(s,m,mH,mB,mF,f,fE,fN)
            },
            success: function (viewHtml) {
                $scope.fireModal(false, "#modal-text", "Sepete Ekle", viewHtml, "", false, "", $scope.fireModalSlider);
            },
            error: function (errorData) {
                console.log(errorData);
            }

        });

    };
    $scope.getCustomReturnProduct = function (product) {
        $http({
            method: "POST",
            url: "/Search/ReturnProduct",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { code: product.Code }

        }).then(function (response) {

            if (response.data.length > 0) {
                setTimeout("location.href='ReturnProduct?ProductCode=" + product.Code, 1);
            } else {
                iziToast.error({
                    title: 'Hata',
                    message: 'Bu ürünü daha önce satın almamışsınız.',
                    position: 'topCenter'
                });
            }
        });

    };
    $scope.getCustomFollowProduct = function (product) {
        if (product.FollowId === 0) {
            $.ajax({
                type: "GET",
                url: "/Search/FollowProducts?productId=" + product.Id,
                data: '',
                datatype: "html",
                beforeSend: function () {
                    $scope.fireModal(true, "#modal-text", "Takipteki Ürünler", "Load", "", false, "", "");
                },
                success: function (viewHtml) {
                    $('#follow' + product.Id).html('<i class="fa fa-pencil fa-fw"></i> Takipten Çıkar');
                    product.FollowId = -1;
                    //$scope.fireModalDataFill("Takipteki Ürünler", viewHtml);
                    //$compile(viewHtml)($scope);
                    //$scope.fireModalOpen(false);
                    $scope.fireModal(false, "#modal-text", "Takipteki Ürünler", viewHtml, "", false, "", "");
                },
                error: function (errorData) {

                    console.log(errorData);
                }
            });
        } else {
            $http({
                method: "POST",
                url: "/Search/FollowProductOrComparisonRemove",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { Id: product.FollowId }
            }).then(function (response) {
                product.FollowId = 0;
                $('#followout' + product.Id).html('<i class="fa fa-pencil fa-fw"></i> Takip Et');
                iziToast.success({
                    message: 'Ürün Takipten Çıkarıldı. ',
                    position: 'topCenter'
                });
            });
        }


    };
    $scope.getCustomComparison = function (product) {
        if (product.ComparisonId === 0) {
            $http({
                method: "POST",
                url: "/Search/ProducrComparisonCount",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}
            }).then(function (response) {
                var status = response.data.Count < 5 ? true : false;
                var comparisonProduct = status ? product.Id : -1;
                $.ajax({
                    type: "GET",
                    url: "/Comparison/ComparisonProducts?productId=" + comparisonProduct,
                    data: '',
                    datatype: "html",
                    beforeSend: function () {
                        $scope.fireModal(true, "#modal-text", "Karşılaştırmaya Ekle", "Load", "", false, "", "");
                    },
                    success: function (viewHtml) {

                        if (response.data.Count >= 1) {
                            //$scope.fireModalDataFill("Karşılaştırmaya Ekle", viewHtml, '<a href="Comparison" class="btn btn-custom">Karşılaştır</a>');
                            $scope.fireModal(false, "#modal-text", "Karşılaştırmaya Ekle", viewHtml, '<a href="Comparison" class="btn btn-custom">Karşılaştır</a>', false, "", "");
                        }
                        else {
                            //$scope.fireModalDataFill("Karşılaştırmaya Ekle", viewHtml, '');
                            $scope.fireModal(false, "#modal-text", "Karşılaştırmaya Ekle", viewHtml, "", false, "", "");
                        }

                    },
                    error: function (errorData) {
                        console.log(errorData);
                    },
                    complete: function () {

                        if (!status) {
                            iziToast.error({
                                message: 'Karşılaştırmaya en fazla 5 adet ürün eklenebilir.',
                                position: 'topRight',
                                transitionIn: 'bounceInLeft'
                            });
                        }
                        else {
                            product.ComparisonId = -1;
                            $('#comparison' + product.Id).html('<i class="fa fa-pencil fa-fw"></i> Karşılaştırmadan Çıkar');
                        }
                    }
                });
            });


        } else {
            $http({
                method: "POST",
                url: "/Search/FollowProductOrComparisonRemove",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { Id: product.ComparisonId }
            }).then(function (response) {
                product.ComparisonId = 0;
                $('#comparisonout' + product.Id).html('<i class="fa fa-pencil fa-fw"></i> Karşılaştırmaya Ekle');
                iziToast.success({
                    message: 'Ürün Karşılaştırmadan Çıkarıldı. ',
                    position: 'topCenter'
                });
            });
        }


    };
    $scope.getCustomDataInput = function (product) {

        $.ajax({
            type: "GET",
            url: "/Search/OemOrRivalInsert?productId=" + product.Id,
            data: '',
            datatype: "html",
            beforeSend: function () {
                $scope.fireModal(true, "#modal-text", "Veri Girişi", "", "", false, "", "");
            },
            success: function (viewHtml) {
                $scope.fireModal(false, "#modal-text", "Veri Girişi", viewHtml, "", false, "", "");
            },
            error: function (errorData) {

                console.log(errorData);
            }
        });

    };
    $scope.getCustomCalculate = function (product) {

        if (product.PriceListCustomerStr === "-")
            return;

        $scope.customHandleSliderDestroy("#calculate-disc1");
        $scope.customHandleSliderDestroy("#calculate-disc2");
        $scope.customHandleSliderDestroy("#calculate-disc3");
        $scope.customHandleSliderDestroy("#calculate-disc4");
        $scope.calculatePriceList = product.PriceListCustomerStr.split('&')[0].replace(',', '.');
        $scope.calculatePriceListStr = angular.copy($scope.calculatePriceList.replace('.', ','));
        $scope.calculateCurrency = product.PriceListCustomerStr.split(';')[1];
        $scope.cost = $scope.calculatePriceList;
        $scope.calculateDisc1 = product.Rule.Disc1;
        $scope.calculateDisc2 = product.Rule.Disc2;
        $scope.calculateDisc3 = product.Rule.Disc3;
        $scope.calculateDisc4 = product.Rule.Disc4;
        $scope.calculatePriceNet = product.PriceNetCustomerStr.split('&')[0].replace(',', '.');
        $scope.calculatePriceNetStr = angular.copy($scope.calculatePriceNet.replace('.', ','));
        $scope.calculateQty = product.MinOrder;

        $("#calculate-disc1").slider({ precision: 2, value: product.Rule.Disc1 });
        $("#calculate-disc1").on("slideStop", function (slideEvt) {
            $scope.calculateDisc1 = slideEvt.value;
            $scope.calculate();
            $("#calculate-disc1Val").html(slideEvt.value);
        });
        $("#calculate-disc2").slider({ precision: 2, value: product.Rule.Disc2 });
        $("#calculate-disc2").on("slide", function (slideEvt) {
            $scope.calculateDisc2 = slideEvt.value;
            $scope.calculate();
            $("#calculate-disc2Val").html(slideEvt.value);
        });
        $("#calculate-disc3").slider({ precision: 2, value: product.Rule.Disc3 });
        $("#calculate-disc3").on("slide", function (slideEvt) {
            $scope.calculateDisc3 = slideEvt.value;
            $scope.calculate();
            $("#calculate-disc3Val").html(slideEvt.value);
        });
        $("#calculate-disc4").slider({ precision: 2, value: product.Rule.Disc3 });
        $("#calculate-disc4").on("slide", function (slideEvt) {
            $scope.calculateDisc3 = slideEvt.value;
            $scope.calculate();
            $("#calculate-disc4Val").html(slideEvt.value);
        });

        $scope.calculateTotal = parseFloat($scope.calculateQty * parseFloat($scope.calculatePriceNet)).toFixed(4).replace('.', ',');
        $scope.fireModal(true, "#modal-calculate", "", "", "", false, "", "");
        firePriceFormat();
        firePriceFormatNumberOnly()
        //$('#modal-calculate').modal();
    };
    $scope.calculate = function () {
        $scope.calculatePriceList = angular.copy($scope.calculatePriceListStr.replace(',', '.'));
        $scope.cost = parseFloat($scope.calculatePriceList);
        $scope.cost *= (1 - ($scope.calculateDisc1 / 100));
        $scope.cost *= (1 - ($scope.calculateDisc2 / 100));
        $scope.cost *= (1 - ($scope.calculateDisc3 / 100));
        $scope.cost *= (1 - ($scope.calculateDisc4 / 100));
        $scope.calculatePriceNet = parseFloat($scope.cost);
        $scope.calculatePriceNetStr = angular.copy($scope.calculatePriceNet.toFixed(4).replace('.', ','));
        $scope.calculateTotal = $scope.calculateQty * parseFloat($scope.calculatePriceNet);
        $('#calculate-priceNet').val(parseFloat($scope.calculatePriceNet).toFixed(4).replace('.', ','));
        $('#calculate-total').val(parseFloat($scope.calculateTotal).toFixed(4).replace('.', ','));
        firePriceFormat();
        firePriceFormatNumberOnly()
    };
    $scope.customHandleSliderDestroy = function (id) {
        $(id).slider();
        $(id).slider('destroy');
    };
    $scope.fireModal = function (s, m, mH, mB, mF, f, fE, fN) {
        if (fE === "")
            fE = "show";

        if (mB === "Load")
            mB = '<div class="row"><div class="col-md-12 text-center"><i class="fa fa-spinner fa-spin fa-3x fa-fw" ></i></div></div>';

        if (mB !== "") {
            $(m).find('.modal-title').html(mH);
            $(m).find('.modal-body').html(mB);
            $(m).find('.modal-footer').html(mF);
        }

        if (f) {
            $(m).on(fE + '.bs.modal', function (e) {
                if (fN !== null && fN !== undefined && fN !== "")
                    fN();
            });
        }
        else {
            if (fN !== null && fN !== undefined && fN !== "")
                fN();
        }

        if (s)
            $(m).modal();
    };
    $scope.fireModalSlider = function () {
        $('.flexslider2').flexslider({
            animation: "slide",
            controlNav: "thumbnails",
            directionNav: false,
            touch: true
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
    // #endregion
    // #region Clear function 
    $scope.clear = function () {
        $scope.selectedManufacturer = "";
        $scope.selectedProductGroup1 = "";
        $scope.selectedProductGroup2 = "";
        $scope.selectedProductGroup3 = "";
        $scope.selectedvehicleBrand = "";
        $scope.selectedvehicleModel = "";
        $scope.txtGeneralSearchText = "";
        $scope.chkCampanign = false;
        //$scope.chkNewArrival = false;
        $scope.chkComparison = false;
        $scope.chkNewProduct = false;
        $scope.chkOnQuantity = false;
        $scope.chkOnWay = false;
        $scope.chkIsPicture = false;
        $scope.searchResult = "";
        $scope.dataCount = 0;
        $scope.vDataCount = 24;
        $scope.repeatCount = 0;
        $scope.lock = false;
        $('#chkCampanign').prop("checked", false);
        //$('#chkNewArrival').prop("checked", false);
        $('#chkComparison').prop("checked", false);
        $('#chkNewProduct').prop("checked", false);
        $('#chkOnQuantity').prop("checked", false);
        $('#chkOnWay').prop("checked", false);
        $('#chkIsPicture').prop("checked", false);
        $('#txtGeneralSearch').val('');
        $('#tbResult').infiniteScrollHelper('destroy');


        $("#vehicleBrand-select")[0].selectedIndex = 0;
        $('#vehicleBrand-select')[0].sumo.reload();
        $('#vehicleBrand-select')[0].sumo.unSelectAll();
        $("#vehicleModel-select")[0].selectedIndex = 0;
        $('#vehicleModel-select').SumoSelect();
        $('#vehicleModel-select')[0].sumo.disable();
        $('#vehicleModel-select')[0].sumo.reload();
        $('#vehicleModel-select')[0].sumo.unSelectAll();
        $("#productGroup-select")[0].selectedIndex = 0;
        $('#productGroup-select').SumoSelect();
        $('#productGroup-select')[0].sumo.reload();
        $('#productGroup-select')[0].sumo.unSelectAll();
        $("#productGroup2-select")[0].selectedIndex = 0;
        $('#productGroup2-select').SumoSelect();
        $('#productGroup2-select')[0].sumo.disable();
        $('#productGroup2-select')[0].sumo.reload();
        $('#productGroup2-select')[0].sumo.unSelectAll();
        $("#productGroup3-select")[0].selectedIndex = 0;
        $('#productGroup3-select').SumoSelect();
        $('#productGroup3-select')[0].sumo.disable();
        $('#productGroup3-select')[0].sumo.reload();
        $('#productGroup3-select')[0].sumo.unSelectAll();

        if (!$scope.isSearchBanner) {
            setTimeout(function () {
                $('#manufacturer-select')[0].sumo.reload();
                $('#manufacturer-select')[0].sumo.unSelectAll();
            }, 200);
            $scope.isSearchBanner = false;
        } else {
            $scope.isSearchBanner = false;
        }





    };
    $scope.fireShortcuts = function () {
        shortcut.add("F2", function () {
            $scope.search(1);
        });
        shortcut.add("F1", function () {
            $scope.search(1);
        });
        shortcut.add("F3", function () {
            $scope.clear();
            $('#txtGeneralSearch').focus();
            return false;
        });

        shortcut.add("F4", function () {
            $scope.clear();
            $('#txtProductCode').focus();
        });
    }
    // #endregion
    // #region Warehouse Quantity
    $scope.titleWarehouseValue = function ($event, productItem, productId) {
        var e = $event.currentTarget;
        if (!$(e).hasClass("tooltipstered")) {

            $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            $scope.fireTooltips(e, true);

            $http({
                method: "POST",
                url: "/Partial/WarehouseView",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { productItem: productItem, productId: productId }
            }).then(function (viewHTML) {

                if (viewHTML.data !== "") {
                    $(e).tooltipster('content', viewHTML.data);
                }
                else {
                    $(e).tooltipster('destroy', true);
                }

                $scope.lastItem++;
            });

            //$.ajax({
            //    type: "POST",
            //    url: "/Partial/WarehouseView",
            //    data: { productId },
            //            datatype: "html",
            //            beforeSend: function () {
            //        $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            //                $scope.fireTooltips(e, true);
            //            },
            //    success: function (viewHtml) {
            //        if (viewHtml !== "") {
            //            var html = viewHtml;
            //            $(e).tooltipster('content', html);
            //        }
            //        else {
            //            $(e).tooltipster('destroy', true);
            //        }

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

    $scope.titlePriceValue = function ($event, product, productId) {

        var e = $event.currentTarget;
        // var productId = id;
        if (!$(e).hasClass("tooltipstered")) {

            $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            $scope.fireTooltips(e, true);

            $http({
                method: "POST",
                url: "/Partial/ProdcutPriceView",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { product: product, productId: productId }
            }).then(function (viewHTML) {

                if (viewHTML.data !== "") {
                    $(e).tooltipster('content', viewHTML.data);
                }
                else {
                    $(e).tooltipster('destroy', true);
                }

                $scope.lastProductItem++;
            });

            //$.ajax({
            //    type: "POST",
            //    url: "/Partial/ProdcutPriceView",
            //    data: { productId },
            //            datatype: "html",
            //            beforeSend: function () {
            //        $(e).attr("title", '<i class="fa fa-spinner fa-pulse fa-lg fa-fw"></i>');
            //                $scope.fireTooltips(e, true);
            //            },
            //    success: function (viewHtml) {
            //        if (viewHtml !== "") {
            //            var html = viewHtml;
            //            $(e).tooltipster('content', html);
            //        }
            //        else {
            //            $(e).tooltipster('destroy', true);
            //        }

            //    },
            //    error: function (errorData) {
            //        //console.log(errorData);
            //    },
            //    complete: function () {
            //        $scope.lastProductItem++;

            //        //if ($scope.lastItem == $scope.basketList.length)
            //        //    $scope.fireTooltips();
            //    }
            //});
        }

    };
    function getParameterByName2(text, name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec("?" + text);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
    $scope.fireSliderBannerSearch = function (p1) {


        if (p1 === "#")
            return;

        var manu = getParameterByName2(p1, 'manu');
        var cmp = getParameterByName2(p1, 'cmp');
        var text = getParameterByName2(p1, 'text');
        // var vehicle = getParameterByName2(p1, 'vehicle');
        //var newArrival = getParameterByName2(p1, 'newarr');
        var comparison = getParameterByName2(p1, 'comp');
        var newProduct = getParameterByName2(p1, 'new');
        var onhand = getParameterByName2(p1, 'onhand');
        // var pg1 = getParameterByName2(p1, 'pg1');


        if (manu !== null && manu !== '') {
            $('#manufacturer-select')[0].sumo.reload();
            $("#manufacturer-select")[0].sumo.selectItem(manu);
            $scope.isSearchBanner = true;
        }
        $scope.clear();
        //if (vehicle != null && vehicle !== '')
        //    $('#ddlVehicleBrand').val(vehicle);
        //if (pg1 != null && pg1 !== '')
        //    $('#ddlProductSubGroup1').val(pg1);
        if (text !== null && text !== '') {
            //$('#txtGeneralSearch').val(text);
            $scope.txtGeneralSearchText = text;
        }
        if (cmp !== null && cmp === '1')
            $scope.chkCampanign = true;
        if (newProduct !== null && newProduct === '1')
            $scope.chkNewProduct = true;
        //if (newArrival != null && newArrival === '1')
        //    $scope.chkNewArrival = true;
        if (comparison !== null && comparison === '1')
            $scope.chkComparison = true;
        if (onhand !== null && onhand === '1')
            $scope.chkOnWay = true;

        $scope.search(1);
    }
    // #endregion




    $(document).ready(function () {
        $rootScope.getCurrencyList('search');
        fireCustomPanelLoading(true);
        $scope.getManufacturerList();
        //$scope.getProductGroup1List();
        //$scope.getVehicleBrandList();s

        $('#productGroup2-select').SumoSelect();
        $('#productGroup2-select')[0].sumo.disable();
        $('#productGroup3-select').SumoSelect();
        $('#productGroup3-select')[0].sumo.disable();

        $('#vehicleModel-select').SumoSelect();
        $('#vehicleModel-select')[0].sumo.disable();

        $('.customselect').SumoSelect();
        $scope.fireShortcuts();
        $scope.getCookieName();

        $('#txtGeneralSearch').focus();
    });

    $scope.getCookieName = function () {
        $http({
            method: "POST",
            url: "/Search/GetCookieValue",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.searchValues = response.data + "SearchProducts";
        });
    };

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

//.factory('HttpService', function ($document, $compile, $rootScope, $templateCache, $http) {

//    return {
//        getModal: function (url, body) {

//            var scope = $rootScope.$new();
//            angular.extend(scope);

//            var template = $http.get(url, { cache: $templateCache })
//                .then(function (response) {

//                    var modal = angular.element([response.data].join(''));

//                    $compile(modal)(scope);
//                    body.after(modal);
//                    scope.close = function () {

//                        modal.remove();
//                        scope.$destroy();
//                    };
//                });
//        }
//    };
//});
