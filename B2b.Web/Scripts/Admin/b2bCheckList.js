adminApp.controller('checkListController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.selectedCheckList = {};

    $scope.loadData = function () {

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/CheckList/GetCheckList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.checkList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.checkList)
            });

            var counterAll = 0, counter = 0;
            angular.forEach($scope.checkList, function (value, key) {
                if(value.Status ===1 || value.Status === 2)
                counter++;

                counterAll++;
            });

            var percent = parseInt((counter * 100) / counterAll);

            $('.easypiechart').data('easyPieChart').update(percent);
            $('.easypiechart').find('span').countTo({ to: percent });

        });
    };

    $scope.showModal = function (content) {
        $scope.deleteItem = content;

        $('#modal-detail').modal('hide');
        $('#modal-detail').modal('show');
    }

    $scope.updateItem = function (item,status) {
        $scope.selectedCheckList = item;
        $scope.selectedCheckList.Status = status;
        $http({
            method: "POST",
            url: "/Admin/CheckList/UpdateCheckList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { selectedCheckList: $scope.selectedCheckList }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });


            $scope.loadData();
        });
    };

    $(document).ready(function () {
        $scope.loadData();
    });


}]);