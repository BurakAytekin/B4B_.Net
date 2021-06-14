adminApp.controller('authorityController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.trasnferTypeList = {};

    $scope.loadTransferTypeData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Authority/GetAuthorityGroupHeader",
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
            url: "/Admin/Authority/SaveAuthorityGroupHeader",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { name: name }//, 
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
            title: 'Yetki Tipi',
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
                url: "/Admin/Authority/DeleteAuthorityGroupHeader",
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


    $scope.loadStepGroups = function () {
        $http({
            method: "POST",
            url: "/Authority/GetStepGroupList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.stepGroupList = response.data;

        });
    };

    $scope.loadStepList = function (row) {
        $http({
            method: "POST",
            url: "/Authority/GetStepList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { groupId: row.GroupId, headerId: parseInt($('#comboTransferType').val()) }

        }).then(function (response) {
            $scope.stepList = response.data;


            angular.forEach($scope.stepList, function (value, key) {
                if (value.HeaderId === value.Id) {
                    angular.forEach($scope.stepList, function (value1, key1) {
                        if (value1.HeaderId === value.Id && value1.HeaderId !== value1.Id) {
                            value1.Disabled = !value.IsChecked;
                        }
                    });
                }
            });
            //$(".styled, .multiselect-container input").uniform({
            //    radioClass: 'choice'
            //});
            $('html, body').animate({ scrollTop: $('#stGroup' + (row.GroupId - 1)).offset().top - 20 }, 'slow');
        });
    };

    $scope.checkedItem = function (row) {

        angular.forEach($scope.stepList, function (value, key) {
            if (value.HeaderId === row.Id && value.HeaderId !== value.Id) {
                value.Disabled = !row.IsChecked;
            }
        });
        $http({
            method: "POST",
            url: "/Authority/InsertAuthorityGroup",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { aStep: row }

        }).then(function (response) {
            row.AuthorityGroupId = response.data.ResultId;

        });
    }

    $(document).ready(function () {
        $scope.loadTransferTypeData();
        $scope.loadStepGroups();
    });

}]).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});