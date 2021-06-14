adminApp.controller('approveController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {
    $scope.processType = {
        comments: 0,
        oemDelete: 1,
        oemBlackListClose: 2,
        blackListDelete: 3
    };

    $scope.oemSelectedType = 0;

    $scope.loadCommentData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ApproveComments/GetCommentList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.blogComment = response.data;
        });
    };

    $scope.loadData = function (type, oemType) {
        $scope.oemSelectedType = oemType;
        $scope.selectedTab = type;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ApproveComments/GetOemBlackListByType",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: parseInt(type), oemType: oemType }
        }).then(function (response) {
            fireCustomLoading(false);
            if (type === 0) {
                $scope.blackList = response.data;
                $scope.tableOemBlackListParams = new NgTableParams({}, {
                    filterDelay: 0,
                    dataset: angular.copy($scope.blackList)
                });

                $scope.originalData = angular.copy($scope.blackList);
            }
            else if (type === 1) {
                $scope.oemEditList = response.data;
                $scope.tableOemEditListParams = new NgTableParams({}, {
                    filterDelay: 0,
                    dataset: angular.copy($scope.oemEditList)
                });

                $scope.originalData = angular.copy($scope.oemEditList);
            }
            else if (type === 2 || type === 3) {
                $scope.oemApproveList = response.data;
                $scope.tableOemApproveListParams = new NgTableParams({}, {
                    filterDelay: 0,
                    dataset: angular.copy($scope.oemApproveList)
                });

                $scope.originalData = angular.copy($scope.oemApproveList);
            }
        });
    };

    $scope.editOemById = function (item, type, oemType) {
        var confirmTitle = "Uyarı Mesajı!";
        var confirmContent = '<div class="formName row">' +
            '<div class="col-md-12">' +
            '<div class="input-group input-group-md input-criteria only-input">' +
            '<input class="brand form-control" type="text"  placeholder="Yeni Değer Giriniz.." required value="' + item.Oem.Brand + '" />' +
            '</div>' +
            '</div>' +
            '<div class="col-md-12">' +
            '<div class="input-group input-group-md input-criteria only-input">' +
            '<input class="oemNo form-control" type="text"  placeholder="Yeni Değer Giriniz.." required value="' + item.Oem.OemNo + '" />' +
            '</div>' +
            '</div>' +
            '</div>';

        var confirmButtons = [
            {
                n: "Save",
                t: "Kaydet",
                c: "btn btn-xs btn-success",
                f: function () {
                    var oemNo = this.$content.find('.oemNo').val();
                    var brand = this.$content.find('.brand').val();
                    if (!oemNo && !brand) {
                        $.alert('Lütfen Değer Giriniz');
                        return false;
                    }

                    $http({
                        method: "POST",
                        url: "/Admin/ApproveComments/EditOemById",
                        headers: { "Content-Type": "Application/json;charset=utf-8" },
                        data: { id: item.Id, oemId: item.OemId, brand: brand, oemNo: oemNo }
                    }).then(function (response) {
                        $scope.loadData(type, oemType);

                        iziToast.show({
                            message: response.data.Message,
                            position: 'topCenter',
                            color: response.data.Color,
                            icon: response.data.Icon
                        });
                    });
                }
            },
            {
                n: "Cancel",
                t: "Vazgeç",
                c: "btn btn-xs btn-danger",
                f: function () { }
            }
        ];

        $scope.fireConfirm(confirmTitle, confirmContent, confirmButtons);
    };

    $scope.insertOem = function (item, type, oemType) {
        var confirmTitle = "Uyarı Mesajı!";
        var confirmContent = '<div class="formName row">' +
            '<div class="col-md-12">' +
            '<div class="input-group input-group-md input-criteria only-input">' +
            '<input class="brand form-control" type="text"  placeholder="Yeni Değer Giriniz.." required value="' + item.BrandName + '" />' +
            '</div>' +
            '</div>' +
            '<div class="col-md-12">' +
            '<div class="input-group input-group-md input-criteria only-input">' +
            '<input class="oemNo form-control" type="text"  placeholder="Yeni Değer Giriniz.." required value="' + item.OemNo + '" />' +
            '</div>' +
            '</div>' +
            '</div>';

        var confirmButtons = [
            {
                n: "Save",
                t: "Kaydet",
                c: "btn btn-xs btn-success",
                f: function () {
                    var oemNo = this.$content.find('.oemNo').val();
                    var brand = this.$content.find('.brand').val();
                    if (!oemNo && !brand) {
                        $.alert('Lütfen Değer Giriniz');
                        return false;
                    }
                    fireCustomLoading(true);
                    $http({
                        method: "POST",
                        url: "/Admin/ApproveComments/InsertOem",
                        headers: { "Content-Type": "Application/json;charset=utf-8" },
                        data: { id: item.Id, productId: item.ProductId, oemType: $scope.oemSelectedType, brand: brand, oemNo: oemNo }
                    }).then(function (response) {
                        fireCustomLoading(false);

                        $scope.loadData(type, oemType);

                        iziToast.show({
                            message: response.data.Message,
                            position: 'topCenter',
                            color: response.data.Color,
                            icon: response.data.Icon
                        });
                    });
                }
            },
            {
                n: "Cancel",
                t: "Vazgeç",
                c: "btn btn-xs btn-danger",
                f: function () { }
            }
        ];

        $scope.fireConfirm(confirmTitle, confirmContent, confirmButtons);
    };

    $scope.fireConfirm = function (t, c, b) {
        //Örn: "Başlık", "İçerik", b[n, t, c, f]
        var btns = {};

        $.each(b, function (eName, eValue) {
            btns[eValue["n"]] = {
                text: eValue.t,
                btnClass: eValue.c,
                action: eValue.f
            }
        });

        $.confirm({
            title: t,
            content: c,
            buttons: btns
        });
    };

    $scope.askForDeleteComment = function (row, processType, type, oemType) {
        $.confirm({
            title: 'Uyarı!',
            content: "İşleme devam etmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {
                        if (processType === $scope.processType.comments)
                            $scope.deleteComment(row);
                        else if (processType === $scope.processType.oemBlackListClose)
                            $scope.closeOemBlackList(row, type, oemType);
                        else if (processType === $scope.processType.blackListDelete)
                            $scope.deleteOemBlackList(row, type, oemType);
                        else if (processType === $scope.processType.oemDelete)
                            $scope.deleteOem(row, type, oemType);
                    }
                },
                Cancel: {
                    text: "Hayır",
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
    };

    $scope.deleteOem = function (row, type, oemType) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ApproveComments/DeleteOem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id, oemId: row.OemId }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.loadData(type, oemType);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.deleteOemBlackList = function (row, type, oemType) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ApproveComments/DeleteOemBlackList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.loadData(type, oemType);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.closeOemBlackList = function (row, type, oemType) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ApproveComments/CloseOemBlackList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.loadData(type, oemType);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.deleteComment = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/DeleteBlogComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            var index = $scope.blogComment.indexOf(row);
            $scope.blogComment.splice(index, 1);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.approveComment = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/ApproveBlogComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            var index = $scope.blogComment.indexOf(row);
            row.IsApproval = true;
            $scope.blogComment[index] = row;

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $(document).ready(function () {
        $scope.loadCommentData();

        $('[data-toggle="tab"]').tooltip({
            trigger: 'hover',
            placement: 'top',
            animate: true,
            delay: 500,
            container: 'body'
        });
    });
}]).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});