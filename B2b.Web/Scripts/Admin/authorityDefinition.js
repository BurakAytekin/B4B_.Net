adminApp.controller('authorityDefinitionController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.detailDisable = false;
    $scope.selectedDefinitionGroup = {};

    $scope.openModal = function (type, row) {


        if (type === 'Add') {
            $scope.addMode = true;
            $scope.editMode = false;
            $scope.selectedDefinitionGroup = {};
            $('#modal-definition').modal('show');


        } else if (type === 'Edit') {
            $scope.addMode = false;
            $scope.editMode = true;
            $scope.selectedDefinitionGroup = angular.copy(row);
            $('#modal-definition').modal('show');

        } else if (type === 'Delete') {

        };
    };

    $scope.showDetail = function (row) {
        $scope.selectedDefinitionGroup = row;
        $scope.detailDisable = true;
        $scope.getListSalesman();

    };

    $scope.saveAuthorityDefinition = function () {
        var groupList = $('.chosenGroup').val();
        var userList = $('.chosenUser').val();

        if (groupList === null || userList === null || $scope.selectedDefinitionGroup === null || groupList.length == 0 || userList.length == 0 || $scope.selectedDefinitionGroup.length === 0) {

            iziToast.error({
                message: 'Lütfen seçim yapınız',
                position: 'topCenter'
            });
        }
        else {
            $http({
                method: "POST",
                url: "/Admin/Authority/SaveAuthorityDefinition",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { groupList: groupList, userList: userList, definitionGroup: $scope.selectedDefinitionGroup }

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

    $scope.getListSalesman = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Authority/GetListSalesman",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.salesmanList = response.data;
            $scope.loadAuthorityGroupHeader();
        });
    };

    $scope.loadAuthorityGroupHeader = function () {

        $http({
            method: "POST",
            url: "/Admin/Authority/GetAuthorityGroupHeader",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            $scope.groupHeaderList = response.data;

        });
    };

    $scope.cancelList = function () {
        $scope.loadData();
        $scope.detailDisable = false;
    }

    $scope.getDefinitionList = function () {

        $http({
            method: "POST",
            url: "/Admin/Authority/GetDefinitionList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { definitionGroupId: $scope.selectedDefinitionGroup.Id }//, 
        }).then(function (response) {
            $scope.definitionList = response.data;
            

            if ($scope.definitionList.length > 0) {
                var groupList = [];
                var userList = [];

                angular.forEach($scope.definitionList, function (value, key) {
                    groupList.push(value.AuthorityGroupHeaderId);
                    userList.push(value.SalesmanId);
                });

                $('.chosenGroup').val(groupList);
                $('.chosenUser').val(userList);
            }
            else {
                $('.chosenGroup').val(null);
                $('.chosenUser').val(null);
            }




            $('.chosen-select').each(function () {
                var element = $(this);
                element.on('chosen:ready', function (e, chosen) {
                    var width = element.css("width");
                    element.next().find('.chosen-choices').addClass('form-control');
                    element.next().css("width", width);
                    element.next().find('.search-field input').css("width", "125px");
                }).chosen();
            });

            fireCustomLoading(false);
        });

    };

    $scope.addOrUpdate = function () {
        if ($scope.selectedDefinitionGroup.Name === "") {
            iziToast.error({
                message: "Lütfen değer giriniz",
                position: 'topCenter'
            });
        }
        else {
            fireCustomLoading(true);

            $http({
                method: "POST",
                url: "/Admin/Authority/AddOrUpdateDefinitionGroup",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { definitionGroup: $scope.selectedDefinitionGroup }

            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });

                $('#modal-definition').modal('hide');

                $scope.loadData();
            });

        };
    };

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Authority/GetAuthorityDefinitionGroup",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.definiitionGroupList = response.data;
        });
    };

    $(document).ready(function () {
        $scope.loadData();
    });


    $scope.deleteDefinition = function (type, row) {
        $scope.selectedDefinitionGroup = row;
        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "Silmek istediğinize emin misiniz?",
                buttons:
                    {
                        Ok: {
                            text: "Evet",
                            btnClass: 'btn-blue',
                            action: function () { $scope.deleteDefinition(true, row); }
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
            $scope.selectedDefinitionGroup.Deleted = true;
            $scope.addOrUpdate();
        }
    };

    $scope.$on('ngRepeatGroupFinished', function (ngRepeatGroupFinishedEvent) {
        $scope.getDefinitionList();

    });

}]).directive('onFinishRender', function ($timeout) {
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