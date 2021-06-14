adminApp.controller('erpFunctionsController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {
    $scope.erpFunctionTypeList = {};
    $scope.selectedItem = {};
    $scope.erpFunctionDetailList = {};

    $scope.loadErpFunctionType = function () {
        $http({
            method: "POST",
            url: "/Admin/ErpFunctions/GetErpFunctionTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            $scope.erpFunctionTypeList = response.data;
        });
    };

    $scope.newRecord = function () {
        $scope.selectedItem = { Id: 0, TypeId:$scope.selectedType };
    };

    $scope.selectItem = function (row) {
        $scope.selectedItem = row;
    };

    $scope.saveItem = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ErpFunctions/SaveErpFunctionDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { item: $scope.selectedItem }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.loadErpFunctionDetail($scope.selectedType);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.deleteErpFunctionDetail = function (type,row) {
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
                            action: function () { $scope.deleteErpFunctionDetail(true,row); }
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
            $scope.selectedItem.Deleted = true;
            $scope.saveItem();
        }
    };

    $scope.duplicateErpFunctionDetail = function (row) {

        $scope.selectedItem = row;
        $.confirm({
            title: 'Servis Kopyalama',
            content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<input type="text" id="newServiceHeader"  placeholder="Başlık Giriniz..." class="form-control" required />' +
                '</div>' +
                '</form>',
            buttons: {
                formSubmit: {
                    text: 'Kaydet',
                    btnClass: 'btn-blue',
                    keys: ['enter'],
                    action: function () {
                        var newServiceHeader = this.$content.find('#newServiceHeader').val();
                        if (!newServiceHeader) {
                            return false;
                        }
                        $scope.selectedItem.Header = newServiceHeader;
                        $scope.selectedItem.Id = 0;
                        $scope.selectedItem.IsActive = true;
                        $scope.saveItem();

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



    $scope.loadErpFunctionDetail = function (typeId) {
        $scope.selectedItem = { Id: 0, TypeId: parseInt(typeId) };
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ErpFunctions/GetErpFunctionDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { typeId: typeId }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.erpFunctionDetailList = response.data;

        });
    };

    $scope.loadCompanyData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CompanySettings/GetSettingList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.companySettingsList = response.data;
        });
    };


    $(document).ready(function () {
        $scope.loadErpFunctionType();
        $scope.loadCompanyData();
    });

}]).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});