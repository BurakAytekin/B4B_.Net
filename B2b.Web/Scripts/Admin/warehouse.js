adminApp.controller('warehouseController', ['$scope', '$http', 'NgTableParams', function ($scope, $http, NgTableParams) {
    // #region Veriables
    $scope.originalData;
    $scope.warehouseList;
    $scope.deleteCount = 0;
    // #endregion

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Warehouse/GetWarehouseList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.warehouseList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.warehouseList)
            });

            $scope.originalData = angular.copy($scope.warehouseList);

            $scope.cancel = $scope.cancel;
            $scope.delete = $scope.delete;
            $scope.save = $scope.save;
            $scope.add = $scope.add;
            $scope.cancelChanges = $scope.cancelChanges;
            $scope.hasChanges = $scope.hasChanges;
            fireCustomLoading(false);

        });
    }
    $scope.hasChanges = function () {
        return $scope.tableParams.$dirty || $scope.deleteCount > 0
    };
    $scope.cancelChanges = function () {
        $scope.resetTableStatus();
        var currentPage = $scope.tableParams.page();
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
            Code: "",
            Name: "",
            Priority: 1,
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
            url: "/Admin/Warehouse/DeleteWarehouse",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }

        }).then(function (response) {
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
            fireCustomLoading(false);
        });
    };
    $scope.resetRow = function (row, rowForm) {

        row.isEditing = false;
        for (let i in $scope.originalData) {
            if ($scope.originalData[i].Id === row.Id) {
                return $scope.originalData[i]
            }
        }
    }
    $scope.save = function (row, rowForm) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Warehouse/UpdateWarehouse",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id, code: row.Code, name: row.Name, priority: row.Priority }

        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            var originalRow = $scope.resetRow(row, rowForm);
            angular.extend(originalRow, row);
            fireCustomLoading(false);
        });


    }
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