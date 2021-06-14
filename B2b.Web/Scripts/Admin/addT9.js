adminApp.controller('addT9Controller', ['$scope', '$http', 'NgTableParams', function ($scope, $http, NgTableParams) {
    // #region Veriables
    $scope.tmpT9List = [];
    $scope.t9Type = false;
    $scope.selectedT9Id = -1;
    $scope.isListSelected = false;

    // #endregion

    $scope.addList = function () {

        if (!$scope.t9Type && $scope.t9Data.length <= 3) {
            iziToast.warning({
                position: 'topCenter',
                message: 'Girilecek Değer En Az 4 Karakter İçermelidir.'
            });
            return;
        }


        if ($scope.tmpT9List.findIndex(x => x.Name == $scope.t9Data) === -1 && $scope.t9Data != "" && $scope.t9Data != undefined) {

            var arrayItem = "{\"Name\":\"" + $scope.t9Data + "\"}";
            var jsonItem = JSON.parse(arrayItem);
            $scope.tmpT9List.push(jsonItem);
            console.log($scope.tmpT9List.indexOf($scope.t9Data));
            console.log($scope.tmpT9List);
            $scope.t9Data = "";
            $scope.tableT9DetailParams = new NgTableParams({
                count: $scope.tmpT9List.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy($scope.tmpT9List)
            });
        } else
        {
            iziToast.warning({
                position: 'topCenter',
                message: 'Listede daha önce eklenmiş aynı veri var.'
            });
        }

    };
    $scope.removeT9Data = function (row) {
        $scope.tmpT9List.splice($scope.tmpT9List.indexOf(row.Name), 1);
        $scope.tableT9DetailParams = new NgTableParams({
            count: $scope.tmpT9List.length // hides pager
        }, {
            filterDelay: 0,
            counts: [],
            dataset: angular.copy($scope.tmpT9List)
        });
    };
    $scope.keypressEventSearchT9 = function (e) {
        var key;

        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            $scope.searchT9();
            e.preventDefault();
        }
    };
    $scope.loadT9DataSplit = function (row) {
        $scope.tmpT9List = [];
        $scope.selectedT9Id = row.Id;
        $scope.isListSelected = true;
        var splitList = row.Data.split(";");

        $.each(splitList, function (index, value) {

            if (value !== "") {
                var arrayItem = "{\"Name\":\"" + value + "\"}";
                var jsonItem = JSON.parse(arrayItem);
                $scope.tmpT9List.push(jsonItem);

            }

        });
        $scope.tableT9DetailParams = new NgTableParams({
            count: $scope.tmpT9List.length // hides pager
        }, {
            filterDelay: 0,
            counts: [],
            dataset: angular.copy($scope.tmpT9List)
        });

    };
    $scope.searchT9 = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/T9/GetListT9",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { searchText: $scope.searchT9Text }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.tableT9KeyParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data)
            });

        });
    };

    $scope.deletesekectedT9 = function () {
        if ($scope.selectedT9Id === -1)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/T9/DeleteT9",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedT9Id }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.searchT9();
            $scope.tmpT9List = [];
            $scope.selectedT9Id = -1;
            $scope.isListSelected = false;
            $scope.tableT9DetailParams = new NgTableParams({
                count: 0// hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: []
            });
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

        });
    };
    $scope.addT9data = function () {

        //burada uzunluk kotrolü felan yapılacak

        if ($scope._t9Name === undefined || $scope._t9Name === "")
            return;

        var t9Datas = "";

        $.each($scope.tmpT9List, function (a, b) {

            t9Datas += b.Name + ";";

        });

        if (t9Datas.length > 0)
            t9Datas.slice(0, -1);

        if (t9Datas.length <= 0) {
            //Uyarı Çıkar

        }
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/T9/AddT9Data",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { key: $scope._t9Name, t9Datas:t9Datas, type: $scope.t9Type }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.clearT9();
            $scope.searchT9();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });


    };
    $scope.clearT9 = function () {
        $scope.tmpT9List = [];
        $scope.selectedT9Id = -1;
        $scope.isListSelected = false;
        $scope.t9Type = false;
        $scope.searchT9Text = "";
        $scope._t9Name = "";
        $scope.t9Data = "";
        $scope.tableT9KeyParams = new NgTableParams({
            count: 0 // hides pager
        }, {
            filterDelay: 0,
            counts: [],
            dataset: []
        });
        $scope.tableT9DetailParams = new NgTableParams({
            count: 0// hides pager
        }, {
            filterDelay: 0,
            counts: [],
            dataset: []
        });
    }

    $(document).ready(function () {


    });

}]);