adminApp.controller('syncSettingsController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.trasnferTypeList = {};
    $scope.companySettingsList = {};
    $scope.selectedItem = { SettingsId: -1, Minute: 10, StartHour: 7, StartMinute: 30, EndHour: 21, EndMinute: 30, ExportViewType: 0, BeforeErpProcedureType: 0, AfterErpProcedureType :0};

    $scope.loadTransferTypeData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/GetTrasnferTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            $scope.trasnferTypeList = response.data;
            fireCustomLoading(false);
        });
    };

    $scope.saveValues = function (name) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/SaveTrasnferType",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { name: name}//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.loadTransferTypeData();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.addTransferType = function () {
        $.confirm({
            title: 'Trasnfer Tipi',
            content: '' +
       '<form action="" class="formName">' +
       '<div class="form-group">' +
       '<input type="text"  placeholder="Değer Giriniz.." class="note form-control" required  />' +
       '</div>' +
       '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    action: function () {
                        var note = this.$content.find('.note').val();
                        if (!note) {
                            return false;
                        }
                        $scope.saveValues(note);
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

   

    $scope.SaveSettings = function () {
        if ($scope.selectedType === undefined || $scope.selectedType === '') {
            iziToast.show({
                message: 'Lütfen Transfer Tipi Seçiniz.',
                position: 'topCenter',
                color: 'error',
                icon: 'fa fa-ban'
            });
            return;
        }
        if ($scope.selectedItem.SettingsId === -1) {
            iziToast.show({
                message: 'Lütfen Firma Seçiniz.',
                position: 'topCenter',
                color: 'error',
                icon: 'fa fa-ban'
            });
            return;
        }

        $scope.selectedItem.TransferTypeId = $scope.selectedType;

        
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/SaveSyncSettings",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { selectedItem: $scope.selectedItem }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.selectedItem.Id = response.data.ResultId;

            $scope.loadSettingsItem($scope.selectedItem.Id,-1);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });

    };


    $scope.loadSettingsItem = function (id, transferTypeId) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/GetSettingItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, transferTypeId: transferTypeId }//, 
        }).then(function (response) {
            $scope.selectedItem = response.data;
            fireCustomLoading(false);

        });
    };


    $scope.deleteTransferType = function (type) {
        var Id = parseInt($('#comboTransferType').val());
        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "Silmek istediğinize emin misiniz?",
                buttons:
                    {
                        Ok: {
                            text: "Evet",
                            btnClass: 'btn-blue',
                            action: function () { $scope.deleteTransferType(true); }
                        },
                        Cancel: {
                            text: "Hayır",
                            btnClass: 'btn-red any-other-class',
                            action: function () {
                                iziToast.show({
                                    message: 'İşleminiz İptal edilmiştir.',
                                    position: 'topCenter',
                                    color: 'error',
                                    icon: 'fa fa-ban'
                                });
                            }
                        }
                    }
            });
        }
        else {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Sync/DeleteTrasnferType",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: Id }//, 
            }).then(function (response) {
                fireCustomLoading(false);
                $scope.loadTransferTypeData();
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
            });
        }
    };

    $scope.loadCompanyData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CompanySettings/GetSettingList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.companySettingsList = response.data;
        });
    };

    $(document).ready(function () {
        $scope.loadTransferTypeData();
        $scope.loadCompanyData();
        firePriceFormatNumberOnly();
    });

}]).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});