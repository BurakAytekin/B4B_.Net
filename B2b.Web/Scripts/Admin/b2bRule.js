adminApp.controller('b2bRuleController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.StartScreen = 'Home';


    $scope.minOrderTotalUpdate = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Veri güncellenektir emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () {
                            fireCustomLoading(true);
                            $http({
                                method: "POST",
                                url: "/Admin/RuleDefinition/MinOrderTotalUpdate",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { total: $scope.basketMinOrderTotal }//, 
                            }).then(function (response) {
                                fireCustomLoading(false);
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                                $scope.loadData();
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
    $scope.updateCustomerWebLogin = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Veri güncellenektir emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () {
                            fireCustomLoading(true);
                            $http({
                                method: "POST",
                                url: "/Admin/RuleDefinition/CustomerWebLogin",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { status: $scope.CustomerWebLoginProcess }//, 
                            }).then(function (response) {
                                fireCustomLoading(false);
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                                $scope.loadData();
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
    $scope.updateSalesmanWebLogin = function () {
        $.confirm({
            title: 'Uyarı!',
            content: "Veri güncellenektir emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () {
                            fireCustomLoading(true);

                            $http({
                                method: "POST",
                                url: "/Admin/RuleDefinition/SalesmanWebLogin",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { status: $scope.SalesmanWebLoginProcess }//, 
                            }).then(function (response) {
                                fireCustomLoading(false);
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                                $scope.loadData();

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
    $scope.updateOrderAutomaticTransfer = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/RuleDefinition/OrderAutomaticTransfer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { status: $scope.OrderAutomaticTransfer }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.loadData();

        });

    }
    $scope.startScreenChange = function (value) {

        $.confirm({
            title: 'Uyarı!',
            content: "Tüm müşterilerin giriş ekranı değişecektir. Emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {
                        fireCustomLoading(true);
                        $http({
                            method: "POST",
                            url: "/Admin/RuleDefinition/ChangeStartScreen",
                            headers: { "Content-Type": "Application/json;charset=utf-8" },
                            data: { startScreen: $scope.StartScreen }//, 
                        }).then(function (response) {
                            fireCustomLoading(false);
                            iziToast.show({
                                message: response.data.Message,
                                position: 'topCenter',
                                color: response.data.Color,
                                icon: response.data.Icon
                            });
                            $scope.loadData();
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
    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/RuleDefinition/GetB2BRulelist",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.b2bRuleList = response.data;
            $scope.basketMinOrderTotal = $scope.b2bRuleList.MinOrderTotal;
            $scope.CustomerWebLoginProcess = $scope.b2bRuleList.CustomerWebLogin;
            $scope.SalesmanWebLoginProcess = $scope.b2bRuleList.SalesmanWebLogin;
            $scope.OrderAutomaticTransfer = $scope.b2bRuleList.OrderAutomaticTransfer;
            $scope.StartScreen = $scope.b2bRuleList.StartScreen;
            $scope.maintenanceStatus = $scope.b2bRuleList.Maintenance;

        });
    };

    $scope.showMaintenanceWarningModal = function () {

        if ($scope.maintenanceStatus)
            $scope.maintenancMessage = "Bakım Modunu Aktifleştirdiğiniz Takdirde Müşterileriniz Ve Plasiyerleriniz Sisteme Giremeyecektir.";
        else
            $scope.maintenancMessage = "Bakım Modunu Kapattığınız Takdirde Müşterileriniz Ve Plasiyerleriniz Sisteme Giriş Sağlayabilecektir.";

        $('#modal-announcement-preview').modal('show');

        $('#modal-announcement-preview').on('hidden.bs.modal', function () {
            $scope.cancelMaintenanceStatusChange();
        })
    };


    $scope.cancelMaintenanceStatusChange = function () {
        $scope.maintenanceStatus = $scope.b2bRuleList.Maintenance;
    };

    $scope.updateMaintenanceStatus = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/RuleDefinition/UpdateMaintenanceStatus",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { status: $scope.maintenanceStatus }//, 
        }).then(function (response) {
            location.reload();
        });
    };




    $(document).ready(function () {
        $scope.loadData();
    });

}]);