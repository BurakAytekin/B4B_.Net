adminApp.controller("contactController", ['$scope', 'NgTableParams', '$http', '$element', function ($scope, NgTableParams, $http, $element) {
    $scope.emptyImage = true;
    $scope.contactItem = null;

    $scope.setFile = function (element) {
        $scope.currentFile = element.files[0];
        var reader = new FileReader();

        reader.onload = function (event) {
            $scope.image_source = event.target.result
            $scope.$apply()
        }
        $scope.emptyImage = false;
        reader.readAsDataURL(element.files[0]);
    };

    $scope.locationChange = function (value) {
        $scope.isShow = true;
        $('#divPreview').html(value);
    };

    $scope.saveContact = function () {
        $scope.contactItem.MapPath = $scope.location;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Contact/SaveContact",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { contact: $scope.contactItem, imageBase: $scope.image_source }
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

    $scope.deleteContact = function () {

        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () {
                            fireCustomLoading(true);
                            $http({
                                method: "POST",
                                url: "/Admin/Contact/DeleteContact",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { id: $scope.contactItem.Id }
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



    }

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Contact/GetContactItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.contactList = response.data;
            if ($scope.contactList.length > 0)
                $scope.selectAdress($scope.contactList[0]);

        });
    };

    $scope.newAddress = function () {
        $scope.contactItem = null;
        $scope.emptyImage = true;
        $scope.location = null;
        $scope.image_source = null;
        $('#divPreview').html('');
        $scope.contactItem.Picture = null;
    }
    $scope.selectAdress = function (adress) {
        $scope.contactItem = adress;
        $scope.emptyImage = ($scope.contactItem === "" || $scope.contactItem.Picture == null) ? true : false;
        $scope.location = $scope.contactItem.MapPath;
        $scope.image_source = $scope.contactItem.Base64String;
        $('#divPreview').html($scope.contactItem.MapPath);
       // $scope.contactItem.Picture = null;
    }

    $(document).ready(function () {
        $scope.loadData();
    });

}]);