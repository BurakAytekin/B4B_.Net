adminApp.controller('companySettingsController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {
    $scope.selectedSettings = { BranchCode: 0, Donem: 0, Company: 0, DatabaseType: 1 };
    $scope.settingsList = {};

    $scope.clearValues = function () {
        $scope.selectedSettings = {};
        $('#isActiveCompany').prop("checked", false);
    };

    $scope.saveValues = function (type) {

        var isActiveCompany;
        if (type) {
            var isActiveCompany = $('#isActiveCompany').prop('checked');
            $scope.selectedSettings.IsActiveCompany = isActiveCompany;

        }
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CompanySettings/SaveSettings",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { settings: $scope.selectedSettings }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.clearValues();
            $scope.loadData();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $('.controls a:first').tab('show');
        });

    };

    $scope.deleteSettings = function (row, type) {
        $scope.selectedSettings = row;
        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "Silmek istediğinize emin misiniz?",
                buttons:
                    {
                        Ok: {
                            text: "Evet",
                            btnClass: 'btn-blue',
                            action: function () { $scope.deleteSettings(row, true); }

                        },
                        Cancel: {
                            text: "Hayır",
                            btnClass: 'btn-red any-other-class',
                            action: function () {
                                iziToast.show({
                                    message: "İşleminiz İptal Edildi",
                                    position: 'topCenter',
                                    color: "red",
                                });
                            }
                        }

                    }
            });
        }
        else {
            $scope.selectedSettings.Deleted = true;
            $scope.saveValues(false);
        }
    };

    $scope.duplicateSettings = function (row) {
        $scope.selectedSettings = row;


        $.confirm({
            title: 'Yeni Firma Adı',
            content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<input type="text" id="newCompanyName"  placeholder="Firma Adı Giriniz..." class="form-control" required />' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () {
                        var newCmpName = this.$content.find('#newCompanyName').val();
                        if (!newCmpName) {
                            return false;
                        }
                        $scope.selectedSettings.CompanyName = newCmpName;
                        $scope.selectedSettings.Id = 0;
                        $scope.selectedSettings.IsActiveCompany = true;
                        $scope.saveValues(false);

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


    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CompanySettings/GetSettingList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.settingsList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.settingsList)
            });

            $scope.originalData = angular.copy($scope.settingsList);

        });
    };

    $scope.editItem = function (row) {
        firePriceFormatNumberOnly();
        $scope.clearValues();
        $('.controls a:last').tab('show');
        if (row === null)
            $scope.selectedSettings = { BranchCode: 0, Donem: 0, Company: 0, DatabaseType: 1 };

        else {
            $scope.selectedSettings = row;
            $('#isActiveCompany').prop("checked", row.IsActiveCompany);
        }
        setTimeout(function () {
            $('#cbDatabaseType').val($scope.selectedSettings.DatabaseType);
        }, 250);
            
    };

    $(document).ready(function () {
        $scope.loadData();
    });

}]);