var $table = $('#pDataTable');
var repeatcount = 0;
// #region AngularCodes
angular.module('customerSelectApp', []).controller('MainController', function ($scope, $http) {

    // #region Veriables
    $scope.basketType = false;
    $scope.town = "Seçiniz";
    $scope.city = "Seçiniz";
    $scope.codeOrName = '';
    $scope.customers = '';
    $scope.dataCount = 0;
    $scope.vDataCount = 20;
    $scope.usersCount = 0;
    $scope.isLock = false;
    // #endregion

    $scope.keypressEvent = function (key, codeOrName, city, town, basketType) {
        if (key.which === 13) {
            $scope.createCustomerSelect(codeOrName, city, town, basketType, 1);
        }
    };

    // Cari kayıt çekme
    $scope.createCustomerSelect = function (codeOrName, city, town, basketType, tb) {
        if ($scope.isLock)
            return;

        $scope.isLock = true;

        // eski tabloyu temizle
        if (tb === 1) {
            $scope.customers = '';
            $scope.dataCount = 0;
            repeatcount = 0;
        }

        var count = $scope.dataCount;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Login/GetCustomerSelectData",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { codeOrName: codeOrName, city: city, town: town, basketType: basketType, count: count }

        }).then(function (response) {
            if (count > 0) {
                $scope.customers = $scope.customers.concat(response.data);
            }
            else
                $scope.customers = response.data;
           
            $scope.dataCount = $scope.dataCount + 25;
            $scope.isLock = false;
            if (response.data.length <= 0) {
                $('#tbResult').infiniteScrollHelper('destroy');
                if (count === 0) {
                    iziToast.error({
                        //title: 'Hata',
                        message: 'Aradığınız kriterlerde sonuç bulunamadı',
                        position: 'topCenter'
                    });
                    fireCustomLoading(false);
                }
                fireCustomLoading(false);
            }
        });
    };

    // Ng repeat işlemi bittiği zaman bu method çalışır
    $scope.$on('ngRepeatCustomerFinished', function (ngRepeatCustomerFinishedEvent) {
        fireCustomLoading(false);
        $('#tbResult').infiniteScrollHelper({
            bottomBuffer: 150,
            triggerInitialLoad: true,
            loadingClass: false,
            loadMore: function (page, done) {
                if (repeatcount > 0 && $scope.vDataCount === 20) {
                    $scope.createCustomerSelect($scope.codeOrName, $scope.city, $scope.town, $scope.basketType, 0);
                }
                repeatcount++;
                done();
            }
        });
    });


    $scope.fireSelectUsers = function (e, id) {
        $('#tbUsersTrresult').html('');
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Login/GetUserList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }

        }).then(function (response) {
          fireCustomLoading(false);
            if (response.data.length === 0) {
                $scope.usersCount = response.data.length;
                e.preventDefault();
                if (e.stopPropagation)
                    e.stopPropagation();
                iziToast.error({
                    title: 'Hata',
                    message: 'Bayiye bağlı bir kullanıcı bulunmamaktadır',
                    messageColor: 'red',
                    messageSize: '15',
                    position: 'topCenter'
                });


            } else if (response.data.length === 1) {
                e.preventDefault();
                if (e.stopPropagation)
                    e.stopPropagation();
                $scope.usersCount = response.data.length;
                setTimeout("location.href='CustomerSelect?CustomerId=" + id + "&UserId=" + response.data[0].Id + "'", 0);

            } else {
                $scope.usersCount = response.data.length;
                $scope.users = response.data;
            }


        });

    };

    function fireCustomLoading(cntrl) {
        if (cntrl === undefined || cntrl === null || cntrl === "")
            cntrl = false;

        if (cntrl) {
            if (!$('.custom-loading-wrapper').hasClass("active"))
                $('.custom-loading-wrapper').addClass("active");
        }
        else {
            if ($('.custom-loading-wrapper').hasClass("active"))
                $('.custom-loading-wrapper').removeClass("active");
        }
    }

    $scope.$on('ngRepeatUserFinished', function (ngRepeatUsersFinishedEvent) {
        if ($scope.usersCount >= 2)
        { $('#modal-text').modal(); }

    });

    $scope.fireUsersSelected = function (customerId, usersId) {
        setTimeout("location.href='CustomerSelect?CustomerId=" + customerId + "&UserId=" + usersId + "'", 1);
    };

    $scope.detailFormatter = function (index, id, customerCode) {

        var colspanValue = $('#tr' + id)[0].cells.length;

        if ($('#trChild' + id).length > 0) {
            $('#trChild' + id).remove();
            $('#ic' + id).removeClass("fa-minus").addClass("fa-plus");
        }
        else {
            var text = '';
            text = '<tr id="trChild' + id + '" class="detail-view"><td colspan="' + colspanValue + '" class="text-center"><div id="divDetail' + customerCode + '"><i class="fa fa-spinner fa-spin fa-3x fa-fw" ></i></div></td></tr>';
            $.ajax({
                type: "GET",
                url: "/login/CustomerSelectDetail?customerCode=" + customerCode + "&customerId=" + id,
                data: '',
                beforeSend: function () {
                    $('#tr' + id).closest('tr').after(text);
                },
                success: function (viewHTML) {
                    $('#trChild' + id).remove();
                    $('#tr' + id).closest('tr').after('<tr id="trChild' + id + '" class="detail-view"><td colspan="' + colspanValue + '">' + viewHTML + '</td></tr>');
                },
                error: function (errorData) {
                    $('#tr' + id).closest('tr').after('<tr id="trChild' + id + '" class="detail-view"><td colspan="' + colspanValue + '">' + errorData + '</td></tr>');
                    console.log(errorData);
                }
            });
            $('#ic' + id).removeClass("fa-plus").addClass("fa-minus");
        }
    };

    // Şehirleri Getir
    $scope.getCityList = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Login/GetCityList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.cityList = response.data;
            $scope.city = response.data[0].City;
        });
    };

    // Şehir Seçimi
    $scope.SelectCityChanged = function () {
        if ($('.selectbox-city').val() !== "Seçiniz") {
            $("#selectTown").prop('disabled', false);
            $scope.getTowns($('.selectbox-city').val());
        } else {
            $scope.town = "Seçiniz";
        }
    }

    // Şehirler Yüklendi
    $scope.$on('ngRepeatCityFinished', function (ngRepeatCityFinishedEvent) {

        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        //var myEl = angular.element(document.querySelector('#selectCity'));
        //myEl.addClass('selectbox');
        $('.selectbox-city').SumoSelect();
        $('.selectbox-city').on('sumo:closed', function (sumo) {
            if ($('.selectbox-city').val() !== "Seçiniz") {
                $scope.getTowns($('.selectbox-city').val());
            } else {
                $scope.town = "Seçiniz";
            }
        });
    });

    // İlçeleri Getir
    $scope.getTowns = function (city) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Login/GetTownList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { city: city }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.townList = response.data;
            $scope.town = response.data[0].Town;
        });
    };

    // İlçeler Yüklendi
    $scope.$on('ngRepeatTownFinished', function (ngRepeatTownFinishedEvent) {

        /*Ng repeat işlemi bittiği zaman bu method çalışır */
        $(".selectbox-town").prop('disabled', false);
        $('.selectbox-town').SumoSelect();
        $('.selectbox-town')[0].sumo.reload();

    });

    $(document).ready(function () {
        $scope.getCityList();

        $('#selectTown').SumoSelect();
        $('#selectTown')[0].sumo.disable();
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


// #endregion