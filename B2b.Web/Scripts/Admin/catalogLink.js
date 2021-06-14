adminApp.controller('catalogLinkController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.originalData;
    $scope.catalogLinkList;
    $scope.deleteCount = 0;

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CatalogLink/GetCatalogLinkList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.catalogLinkList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.catalogLinkList)
            });
            $scope.originalData = angular.copy($scope.catalogLinkList);
            $scope.cancel = $scope.cancel;
            $scope.delete = $scope.delete;
            $scope.save = $scope.save;
            $scope.add = $scope.add;
            $scope.cancelChanges = $scope.cancelChanges;
            $scope.hasChanges = $scope.hasChanges;

        });
    };

    $scope.hasChanges = function () {
        return $scope.tableParams.$dirty || $scope.deleteCount > 0
    };

    $scope.cancelChanges = function () {
        $scope.resetTableStatus();
        var currentPage = $scope.tableParams.page();
        //$scope.tableParams.settings({
        //    dataset: angular.copy($scope.originalData)
        //});
        // keep the user on the current page when we can
        $scope.loadData();
        if (!$scope.isAdding) {
            $scope.tableParams.page(currentPage);
        }
    };

    $scope.resetTableStatus = function () {
        $scope.isEditing = false;
        $scope.isAdding = false;

        for (var i = 0; i < $scope.tableParams._settings.dataset.length; i++) {
            $scope.tableParams._settings.dataset[i].isEditing = false;
        }
    };

    $scope.add = function () {
        $scope.isEditing = true;
        $scope.isAdding = true;
        $scope.tableParams.settings().dataset.unshift({
            Id: 0,
            Header: "",
            Link: "",
            IsActive: true,
        });


        $scope.tableParams.sorting({});
        $scope.tableParams.page(1);
        $scope.tableParams.reload();

        for (var i = 0; i < $scope.tableParams._settings.dataset.length; i++) {
            $scope.tableParams._settings.dataset[i].isEditing = true;
        }
    };

    $scope.cancel = function (row, rowForm) {
        var originalRow = $scope.resetRow(row, rowForm);
        angular.extend(row, originalRow);
    };

    $scope.askForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.delete(row); }

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

    $scope.delete = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CatalogLink/DeleteCatalogLink",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            var index = $scope.tableParams.data.indexOf(row);
            $scope.deleteCount++;
            $scope.tableParams.data.splice(index, 1);
            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy(this.tableParams.data)
            });

        });
    };

    $scope.resetRow = function (row, rowForm) {

        row.isEditing = false;
        for (let i in $scope.originalData) {
            if ($scope.originalData[i].Id === row.Id) {
                return $scope.originalData[i]
            }
        }
    };

    $scope.save = function (row, rowForm) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CatalogLink/UpdateCatalogLink",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id, header: row.Header, link: row.Link, isActive: row.IsActive }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            var originalRow = $scope.resetRow(row, rowForm);
            angular.extend(originalRow, row);

        });
    };

    $scope.keypressEvent = function (e, row) {

        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 46)
            e.returnValue = false;
        if (key === 13) {
            $scope.save(row);
        }
    };

    $(document).ready(function () {
        $scope.loadData();


    });

}]);