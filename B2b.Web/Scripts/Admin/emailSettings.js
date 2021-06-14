adminApp.controller('EmailSettingsController', function ($scope, $http, NgTableParams, $parse) {

    $scope.getMailSettings = function (type) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/EmailSettings/GetMailSettings",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: type }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.selectedSettings = response.data;
        });
    }



    $scope.updateMailSettings = function (type) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/EmailSettings/UpdateMailSettings",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { selectedMailSettings: $scope.selectedSettings }
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
    function validateEmail(email) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(String(email).toLowerCase());
    }
    $scope.sendTestMail = function () {
        $.confirm({
            title: 'Uyarı!',
            content: '' +
            '<form action="" class="formName">' +
            '<div class="form-group">' +
            '<label>Mail Adresi</label>' +
            '<input type="text" placeholder="Mail adresi giriniz..." class="mail form-control" required />' +
            '</div>' +
            '</form>',
            buttons: {
                formSubmit: {
                    text: 'GONDER',
                    btnClass: 'btn-blue',
                    action: function () {
                        var mailAddress = this.$content.find('.mail').val();
                        if (!mailAddress || !validateEmail(mailAddress)) {
                            $.alert('Lütfen geçerli bir mail adresi giriniz...');
                            return false;
                        }
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Admin/EmailSettings/SendTestMail",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: { selectedMailSettings: $scope.selectedSettings, mailAddress: mailAddress }
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
                },
                IPTAL: function () {
                    //close
                },
            }
        });
    }

    $(document).ready(function () {
        $scope.getMailSettings(0);


    });

});