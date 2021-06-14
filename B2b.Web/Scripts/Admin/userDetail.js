adminApp.controller('UserDetailController', function ($scope, $http, NgTableParams) {
    $scope.selectedUser = {};
    $scope.lockUserUpdate = true;
    $scope.selectedDefaultUser = null;
    $scope.userDetailOpen = function (row) {
        $scope.selectedUser = row;
        $scope.selectedDefaultUser = angular.copy(row);
        $(mUserDetail).appendTo("body").modal('show');
        $(mUserDetail).on('shown.bs.modal', function () {
            // $('[href="#tUserGeneralInformation"]').tab('show');
        });

        $(mUserDetail).on('hidden.bs.modal', function () {
            $('[href="#tUserGeneralInformation"]').tab('show');
            $scope.selectedUser = null;
            //  $scope.var_map = null;
        });
    };
    $scope.newuserAddOpen = function () {
        $scope.selectedUser = {};
        $scope.lockUserUpdate = false;
        $(mUserDetail).appendTo("body").modal('show');
        $(mUserDetail).on('shown.bs.modal', function () {
            // $('[href="#tUserGeneralInformation"]').tab('show');
        });
        $(mUserDetail).on('hidden.bs.modal', function () {
            $('[href="#tUserGeneralInformation"]').tab('show');
            $scope.selectedUser = null;
            //  $scope.var_map = null;
        });
    };

    $scope.editActive = function () {
        $scope.lockUserUpdate = false;
    };


    $scope.editCancel = function () {
        $scope.lockUserUpdate = true;

        if (!$scope.selectedDefaultUser.undefined)
            $scope.selectedUser = angular.copy($scope.selectedDefaultUser);
        else
            $scope.selectedUser = {};
    };
    $scope.getcustomerUserLoginLog = function () {
        if ($scope.selectedUser === {} || $scope.selectedUser.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerLoginLog",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: -1, userId: $scope.selectedUser.Id }
        }).then(function (response) {
            
            $scope.tableCustomerUsersLoginParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });
            fireCustomLoading(false);
        });
    }



    $scope.saveUser = function () {

        $scope.selectedUser.CustomerId = $scope.selectedCustomer.Id;

        if ($scope.selectedUser.Code === undefined || $scope.selectedUser.Code === "") {
            iziToast.show({
                message: "Kullanıcı Kodu Boş Olamaz",
                position: 'topCenter',
                color: "error",
                icon: "fa fa-ban"
            });
            return;
        }
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/UpdateUser",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { selectedUser: $scope.selectedUser }
        }).then(function (response) {
            $scope.lockUserUpdate = true;

            var index = $scope.userList.indexOf($scope.selectedDefaultUser);
            $scope.userList.splice(index, 1);

            $scope.userList.push($scope.selectedUser);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });


            $('#mUserDetail').modal('hide');

            fireCustomLoading(false);
            $scope.getCustomerUserList();


        });

    };


    $scope.showLocation = function () {

        if ($scope.selectedUser.Latitude === '' || $scope.selectedUser.Latitude === null) {
            $('#tUserLocation').empty().append('Kullanıcı lokasyon bildirmemiştir.');
        }
        else {
            var var_location = new google.maps.LatLng(parseFloat($scope.selectedUser.Latitude.replace(',', '.')), parseFloat($scope.selectedUser.Longitude.replace(',', '.')));

            var var_mapoptions = {
                center: var_location,
                zoom: 14
            };

            var var_marker = new google.maps.Marker({
                position: var_location,
                map: $scope.var_map,
                title: "Venice"
            });

            $scope.var_map = new google.maps.Map(document.getElementById("map-location"),
                var_mapoptions);

            var_marker.setMap($scope.var_map);
        }

    };


    $scope.getUserAuthority = function () {

        if ($scope.selectedUser === {} || $scope.selectedUser.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetAuthorityUser",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedUser.Id }
        }).then(function (response) {
            
            $scope.userFieldList = response.data.FieldList;
            $scope.userAuthorityItem = response.data.AuthorityItem;
            fireCustomLoading(false);
        });

    };

    $scope.checkAuthorityUser = function (field) {

        var updateValue = $scope.userAuthorityItem[0][field];
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/UpdateAuthorityUser",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.userAuthorityItem[0].Id, updateValue: updateValue, field: field }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);

        });
    };

    $scope.searchUserLogList = function () {
        //alert($scope.dateStart);
        //alert($scope.dateEnd);
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetSearchList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                dateStart: $('#iSearchUserLogStartDate').val(), dateEnd: $('#iSearchUserLogEndDate').val(), customerId: -1, userId: $scope.selectedUser.Id
            }
        }).then(function (response) {

            if (response.data.length <= 0) {
                iziToast.error({
                    message: 'Aradığınız kriterlerde sonuç bulunamamıştır.',
                    position: 'topCenter'
                });
            }

            $scope.reportUserList = response.data;
            fireCustomLoading(false);
        });
    };

    $scope.getUserDetailSearchTab = function () {


        if ($scope.selectedUser.Id == undefined || $scope.selectedUser.Id < 1) {

            $scope.reportUserList = [];
        };
    };

    function setDefaultDate() {
        var today = new Date();

        $('.detepicker').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    };

    $scope.showEmailSendScreen = function () {

        $scope.userEmail = $scope.selectedUser.Mail;
        $('#modal-EmailShow').appendTo("body").modal('show');
    };

    $scope.sendPasswordResetMail = function () {
        if ($scope.userEmail === '') {
            iziToast.error({
                message: 'Lütfen mail adresi giriniz',
                position: 'topCenter',
            });
            return;
        }

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/SendPasswordResetMail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { user: $scope.selectedUser, email: $scope.userEmail }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
            $('#modal-EmailShow').appendTo("body").modal('hide');

        });
    };


    $scope.createAuthenticator = function (item, type) {
        fireCustomLoading(true);
        if (type === 1) {
            $.confirm({
                title: 'Uyarı!',
                content: "Qr Code görselini yenilediğinizde kullanıcının görseli tekrar okutması gerekmektedir. Devam etmek istediğinize emin misiniz?",
                buttons:
                {
                    EVET: {
                        btnClass: 'btn-blue',
                        action: function () {

                            $http({
                                method: "POST",
                                url: "/Admin/Customers/CreateAuthenticator",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { user: item, type: type }
                            }).then(function (response) {
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                                $scope.getAuthenticatorImage();

                                fireCustomLoading(false);
                            });
                        }

                    },
                    HAYIR: {
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Silme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
            });
        }
        else {
            $http({
                method: "POST",
                url: "/Admin/Customers/CreateAuthenticator",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { user: item, type: type }
            }).then(function (response) {
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $scope.getAuthenticatorImage();

                fireCustomLoading(false);
            });
        }



    };

    $scope.getAuthenticatorImage = function () {
        $http({
            method: "POST",
            url: "/Admin/Customers/GetSalesmanAuthenticatorImage",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedUser.Id }
        }).then(function (response) {
            $scope.selectedUser = response.data.User;
            $scope.authenticatorQrCode = response.data.QrCode;

        });
    };

    $scope.setAuthenticatorValue = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/UpdateUser",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { selectedUser: $scope.selectedUser }
        }).then(function (response) {
            
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            
            fireCustomLoading(false);
        });
    }

    $(document).ready(function () {

        setDefaultDate();

    });

});


