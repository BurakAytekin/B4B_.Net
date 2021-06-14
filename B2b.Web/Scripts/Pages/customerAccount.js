b2bApp.controller('CustomerAccountController', function ($scope, $http) {

    $scope.newRecord = false;

    $scope.myMap = function (type) {
        $scope.newRecord = type;
        if ($scope.CurrentCustomer.Users.Latitude === '' || $scope.newRecord) {

            $.confirm({
                title: 'Bilgilendirme!',
                content: '' +
                    '<form action="" class="formName">' +
                    '<div class="form-group">' +
                    '<img   src="../Content/images/locationSave.png"  />' +
                    '<p> Lokasyonunuzu belirleyebilmemiz için çıkan uyarıya izin vermelisiniz ! </p>' +
                    '</div>' +
                    '</form>',
                buttons:
                {
                    EVET: {
                        text: 'Lokasyonumu Kaydet',
                        btnClass: 'btn-blue',
                        action: function () { $scope.getLoacation(); }

                    },
                    HAYIR: {
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Lokasyon alma işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
            });
        } else {
            $scope.getLoacation();
        }
    };

    $scope.getLoacation = function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition($scope.init_map);
        } else {
            x.innerHTML = "Lokasyonunuz harita üzerinde tanınamamaktadır.";
        }
    };

    $scope.init_map = function (position) {
        if ($scope.CurrentCustomer.Users.Latitude === '' || $scope.newRecord) {
            fireCustomLoading(true);

            $http({
                method: "POST",
                url: "/Account/UpdateCustomerLocation",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { latitude: position.coords.latitude, longitude: position.coords.longitude }

            }).then(function (response) {
                fireCustomLoading(false);

                if (response.data.Statu === "success") {
                    iziToast.success({
                        message: response.data.Message,
                        position: 'topCenter'
                    });

                } else {
                    iziToast.error({
                        //title: 'Hata',
                        message: response.data.Message,
                        position: 'topCenter'
                    });

                }
            });
        } else {
            var var_location = new google.maps.LatLng(parseFloat($scope.CurrentCustomer.Users.Latitude.replace(',', '.')), parseFloat($scope.CurrentCustomer.Users.Longitude.replace(',', '.')));

            var var_mapoptions = {
                center: var_location,
                zoom: 14
            };

            var var_marker = new google.maps.Marker({
                position: var_location,
                map: var_map,
                title: "Venice"
            });

            var var_map = new google.maps.Map(document.getElementById("map-container"),
                var_mapoptions);

            var_marker.setMap(var_map);
        }

    }

    $scope.checkMyAvatar = function () {
        if ($scope.CurrentCustomer.Users.Avatar === '') {
            document.getElementById("noavatar.png").checked = true;
        } else {

            document.getElementById($scope.CurrentCustomer.Users.Avatar).checked = true;
        }
    };

    $scope.askForPasswordUpdate = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Şifreniz güncellenecektir. Devam etmek istiyor musunuz?",
            buttons:
            {
                EVET: {
                    btnClass: 'btn-blue',
                    action: function () { $scope.updatePassword(); }

                },
                HAYIR: {
                    btnClass: 'btn-red any-other-class',
                    action: function () {

                        iziToast.error({
                            message: 'Güncelleme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }

            }
        });
    };

    $scope.updatePassword = function () {
        var oldPassword = $('#oldPassword').val();
        var newPassword = $('#newPassword').val();
        var newPasswordRepead = $('#newPasswordRepead').val();

        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Account/UpdatePassword",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { oldPassword: oldPassword, newPassword: newPassword, newPasswordRepead: newPasswordRepead }

        }).then(function (response) {
            fireCustomLoading(false);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });

    };

    $scope.keyPressedRate = function (e) {
        var key, brwsr, keyCtrl = false, keyShift = false, keyAlt = false, keyMeta = false;

        if (window.event) { // Internet Explorer
            if (window.event.shiftKey) keyShift = true;
            if (window.event.ctrlKey) keyCtrl = true;
            if (window.event.metaKey) keyMeta = true;
            if (window.event.altKey) keyAlt = true;
            key = window.event.keyCode;
            brwsr = true;
        } else if (e) { // Firefox
            if (e.shiftKey) keyShift = true;
            if (e.ctrlKey) keyCtrl = true;
            if (e.metaKey) keyMeta = true;
            if (e.altKey) keyAlt = true;
            key = e.which;
            brwsr = false;
        } else { // Other Browsers
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
            } else
                e.returnValue = false;

            if (brwsr === true) {
                window.event.returnValue = false;
                event.keyCode = 0;
            } else {
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
        } else if (key === 13) {
            $scope.updateCustomerRate();
            fireReturnFalse(e);
        } else if (key === 8 || key === 9 || key === 37 || key === 39) {
            fireReturnTrue();
        } else if (key < 48 || key > 57) {
            fireReturnFalse(e);
        } else {
            fireReturnTrue();
        }
    };

    $scope.updateCustomerRate = function () {

        var rate = $('#txtRate').val();
        if (rate != '') {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Account/UpdateCustomerRate",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { rate: rate }

            }).then(function (response) {
                fireCustomLoading(false);

                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
            });
        }

    };

    $scope.setCustomeravatar = function (avatarName) {
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Account/SetAvatarName",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { avatarName: avatarName }

        }).then(function (response) {
            fireCustomLoading(false);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.getCouponList = function () {
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Account/GetCouponList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: 1 }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.couponList = response.data;
        });
    };
    $scope.searchLogList = function () {
        //alert($scope.dateStart);
        //alert($scope.dateEnd);
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Account/GetSearchList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                dateStart: $('#iSearchLogStartDate').val(), dateEnd: $('#iSearchLogEndDate').val()
            }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.reportList = response.data;
        });
    };

    $scope.searchLogDetail = function (id) {
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Account/GetSearchDetailList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id:id}
        }).then(function (response) {
            fireCustomLoading(false);

                $scope.reportDetailList = response.data;
                $('#modal-logdetail').modal();
            });
    }
    function setDefaultDate() {
        var today = new Date();

        $('.detepicker').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    };
    $(document).ready(function () {
        $scope.CurrentCustomer = currentCustomer;
        setDefaultDate();
    });


}).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});