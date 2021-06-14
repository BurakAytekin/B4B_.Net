adminApp.controller('ruleController', ['$scope', '$http', 'NgTableParams', function ($scope, $http, NgTableParams) {
    // #region Veriables

    $scope.originalData;
    $scope.ruleList=[];
    $scope.deleteCount = 0;

    // #endregion

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Rule/GetRuleList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.ruleList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.ruleList)
            });

            $scope.originalData = angular.copy($scope.ruleList);

            $scope.cancel = $scope.cancel;
            $scope.delete = $scope.delete;
            $scope.save = $scope.save;
            $scope.add = $scope.add;
            $scope.cancelChanges = $scope.cancelChanges;
            $scope.hasChanges = $scope.hasChanges;

            fireCustomLoading(false);

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
            Product: "",
            Customer: "",
            PaymentType: 0,
            Disc1: 0,
            Disc2: 0,
            Disc3: 0,
            Disc4: 0,
            PriceNumber: 1,
            Rate:0
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
            url: "/Admin/Rule/DeleteRule",
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
            url: "/Admin/Rule/UpdateRule",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id, product: row.Product, customer: row.Customer, paymentType: row.PaymentType, disc1: row.Disc1, disc2: row.Disc2, disc3: row.Disc3, disc4: row.Disc4, priceNumber: row.PriceNumber ,rate:row.Rate}

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

    $scope.exportToExcel = function () {

        if ($scope.ruleList.length > 0) {
            var newData = [];

            $scope.ruleList.forEach(function (e) {

                newData.push({
                    "Stok": e.Product,
                    "Cari": e.Customer,
                    "Ödeme Tipi": e.PaymentType,
                    "Isk1": e.Disc1,
                    "Isk2": e.Disc2,
                    "Isk3": e.Disc3,
                    "Isk4": e.Disc4,
                    "Stok Fiyatı": e.PriceNumber,
                    "Oran": e.Rate
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                         "Stok",
                         "Cari",
                         "Ödeme Tipi",
                         "Isk1",
                         "Isk2",
                         "Isk3",
                         "Isk4",
                         "Stok Fiyatı",
                         "Oran"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Satis_Kosullari_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Satis_Kosullari_Listesi.xlsx");
        }
    };

    $(document).ready(function () {
        $scope.loadData();        
    });

}]);