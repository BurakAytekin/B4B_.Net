adminApp.controller('reportSettingsController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {
    $scope.erpFunctionTypeList = {};
    $scope.selectedItem = {};
    $scope.reportDetailList = {};
    $scope.lockText = true;
    $scope.lockTextCompany = true;
    $scope.parametersTypeList = ["text", "number", "checkbox", "datetime"];


    $scope.b2bShowStatus = [
{ Value: 0, Text: 'Gözükmesin' },
{ Value: 1, Text: 'Sadece Admin Plasiyer Görsün' },
{ Value: 2, Text: 'Tüm Plasiyerler Görsün' },
    ];







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
        $scope.lockText = false;
        $scope.selectedItem = { Id: 0, IsActive: false };
        if (!$.fn.dataTable.isDataTable($scope.tableParams)) {
            $scope.tableParams = new NgTableParams({}, {
                count: 0,
                filterDelay: 0,
                dataset: [],
            });
        }
        $scope.tableParams.reload();
    };
    $scope.resetTableStatus = function () {
        $scope.tableParams.data.splice(0, 1);
        $scope.tableParams = new NgTableParams({}, {
            filterDelay: 0,
            dataset: angular.copy($scope.tableParams.data),
        });
        $scope.tableParams.reload();

    };

    $scope.cancelChanges = function () {
        $scope.resetTableStatus();
        $scope.isEditing = false;
        var currentPage = $scope.tableParams.page();
        //$scope.loadData();
        if (!$scope.isAdding) {
            $scope.tableParams.page(currentPage);
        }
    };

    $scope.resetRow = function (row, rowForm) {
        row.isEditing = false;
        rowForm.$setPristine();
        self.tableTracker.untrack(row);
        return _.findWhere(originalData, function (r) {
            return r.id === row.id;
        });
    }

    $scope.cancel = function (row, rowForm) {
        var originalRow = $scope.resetRow(row, rowForm);
        angular.extend(row, originalRow);
    }

    $scope.selectItem = function (row) {
        $scope.selectedItem = row;
        $scope.lockText = false;

        $scope.tableParams = new NgTableParams({}, {
            count: $scope.selectedItem.Parameters.length,
            filterDelay: 0,
            dataset: angular.copy($scope.selectedItem.Parameters),
        });
    };

    $scope.addParameters = function () {
        $scope.isEditing = true;
        $scope.isAdding = true;
        $scope.tableParams.settings().dataset.unshift({
            Type: "",
            paramName: "",
            Header: "",
        });
        $scope.tableParams.sorting({});
        $scope.tableParams.page(1);
        $scope.tableParams.reload();
        $scope.tableParams._settings.dataset[0].isEditing = true
        //for (var i = 0; i < $scope.tableParams._settings.dataset.length; i++) {
        //    $scope.tableParams._settings.dataset[i].isEditing = true;
        //}
    };

    $scope.resetRow = function (row, rowForm) {
        row.isEditing = false;
        for (let i in $scope.originalData) {
            if ($scope.originalData[i].Id === row.Id) {
                return $scope.originalData[i];
            }
        }
    };
    $scope.save = function (row, rowForm) {
        $scope.isEditing = false;
        $scope.isAdding = false;
        var originalRow = $scope.resetRow(row, rowForm);
        angular.extend(originalRow, row);
    }

    $scope.saveItem = function () {
        var params = "";
        angular.forEach($scope.tableParams.data, function (value) {
            params += value.paramName + "=" + value.Header + "=" + value.Type + ";"
        });
        params = params.length > 0 ? params.substr(0, (params.length - 1)) : params;


        if ($scope.selectedItem.CompanySettings === undefined)
            $scope.selectedItem.CompanySettings = { Id: -1 };
        $http({
            method: "POST",
            url: "/Admin/ReportSettings/SaveReportSettings",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                aReport: $scope.selectedItem,
                aParameters: params
                //id: $scope.selectedItem.Id, name: $scope.selectedItem.Name, header: $scope.selectedItem.Header, settingsId: $scope.selectedItem.CompanySettings.Id,
                //type: $scope.selectedItem.Type, functions: $scope.selectedItem.Function, functionType: $scope.selectedItem.FunctionType, parameters: params,
                //isActive: $scope.selectedItem.IsActive
            }
        }).then(function (response) {
            $scope.loadReportDetail();
            $scope.selectedItem.Id = (response.data.ResultId == -1 || response.data.ResultId == -2) ? $scope.selectedItem.Id : response.data.ResultId;
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };







    $scope.loadReportDetail = function () {
        $http({
            method: "POST",
            url: "/Report/Home/GetMenuList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { reportIsActive: 0 }
        }).then(function (response) {
            $scope.reportDetailList = response.data;

        });
    };

    $scope.loadCompanyData = function () {
        $http({
            method: "POST",
            url: "/Admin/CompanySettings/GetSettingList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            $scope.companySettingsList = response.data;
        });
    };

    $scope.deleteReport = function (row) {
        $http({
            method: "POST",
            url: "/Admin/ReportSettings/DeleteReportSettings",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                id: row.Id
            }
        }).then(function (response) {
            $scope.loadReportDetail();
            if (response.data.ResultId != -1) {
                $scope.lockText = true;
                $scope.selectedItem = {};
            }
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };


    $scope.askForDelete = function (row, functionName) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () { $scope[functionName](row); }
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

    $scope.deleteReportParams = function (row) {
        var index = $scope.tableParams.data.indexOf(row);
        $scope.tableParams.data.splice(index, 1);
        $scope.tableParams = new NgTableParams({}, {
            count: $scope.selectedItem.Parameters.length,
            filterDelay: 0,
            dataset: angular.copy($scope.tableParams.data),
        });
        $scope.tableParams.reload();
    };

    $(document).ready(function () {
        $scope.loadReportDetail();
        $scope.loadErpFunctionType();
        $scope.loadCompanyData();
    });

}]);