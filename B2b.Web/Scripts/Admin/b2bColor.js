adminApp.controller('colorController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.selectedColor = {
        Id: 0,
        Header: '',
        Color1: '#ff0000',
        Color2: '#ff0000',
        Color3: '#ff0000',
        IsActive: false
    };

  


    $scope.loadData = function () {
       
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/ProjectColor/GetColorList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.colorList = response.data;
            $scope.colorDefault = angular.copy($scope.colorList);
            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.currencyList)
            });
        });
    };


    $scope.openModal = function (type, row) {


        if (type == 'Add') {
            $scope.addMode = true;
            $scope.editMode = false;
            $scope.selectedColor = {
                Id: 0,
                Header: '',
                Color1: '#ff0000',
                Color2: '#ff0000',
                Color3: '#ff0000',
                IsActive: false
            };
            $('#modal-currency').modal('show');


        } else if (type == 'Edit') {
            $scope.addMode = false;
            $scope.editMode = true;
            $scope.selectedColor = angular.copy(row);
            $('#modal-currency').modal('show');

        } else if (type == 'Delete') {
            $scope.selectedColor = row;
            $('#modal-currency-delete').modal('show');
        };
    };

    $scope.delete = function () {
        fireCustomLoading(true);
        if ($scope.selectedColor.IsActive) {
            iziToast.error({
                message: 'Varsayılan temanızı silemezsiniz !!',
                position: 'topCenter'
            });
            $('#modal-currency-delete').modal('hide');
            fireCustomLoading(false);
        }
        else {
            $scope.selectedColor.Deleted = true;
            $http({
                method: "POST",
                url: "/Admin/ProjectColor/DeleteColor",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { selectedColor: $scope.selectedColor }

            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $('#modal-currency-delete').modal('hide');

                $scope.loadData();

            });
        }

       
    };



    $scope.validateColor = function () {
        $scope.ValidateError = true;
        $scope.ValidateResult = '';

        if (($scope.selectedColor.Header == '' || $scope.selectedColor.Header == undefined)
            || ($scope.selectedColor.Color1 == '' || $scope.selectedColor.Color1 == undefined)
            || ($scope.selectedColor.Color2 == '' || $scope.selectedColor.Color2 == undefined)
            || ($scope.selectedColor.Color3 == '' || $scope.selectedColor.Color3 == undefined)
            || ($scope.selectedColor.IsActive == undefined)
            ) {
            $scope.ValidateError = 'Lütfen Tüm Alanları Doldurunuz';
            $scope.ValidateResult = false;
        }


    };

    $scope.addOrUpdate = function () {


        $scope.validateColor();

        if ($scope.ValidateResult === false) {
            iziToast.error({
                message: $scope.ValidateError,
                position: 'topCenter'
            });
        }
        else {
            fireCustomLoading(true);

            $http({
                method: "POST",
                url: "/Admin/ProjectColor/AddOrUpdate",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { selectedColor: $scope.selectedColor }

            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });

                $('#modal-currency').modal('hide');

                $scope.loadData();
            });

        };
    };

    //$scope.$watch('mycolor', function (newVal) {
    //    console.log('newVal ' + newVal);
    //});

    $(document).ready(function () {
        $scope.loadData();
    });

}]);