adminApp.controller('fileListController', ['$scope', '$http', '$element', function ($scope, $http, $element) {

    $scope.rblRestriction = 0;

    $scope.setFile = function (element) {
        $scope.currentFile = element.files[0];
        var reader = new FileReader();
        //$scope.image_path = element.value;
        reader.onload = function (event) {
            $scope.file_source = event.target.result;
            $scope.$apply()
        }
        reader.readAsDataURL(element.files[0]);
    };

    $scope.setFileImage = function (element) {
        $scope.currentFile = element.files[0];
        var reader = new FileReader();
        reader.onload = function (event) {
            $scope.image_sourceIcon = event.target.result;
            $scope.$apply()
        }
        reader.readAsDataURL(element.files[0]);
    };

    $scope.fileDownloadAll = function () {
        angular.forEach($scope.fileList, function (data) {
            if (data.Checked) {
                window.open(data.Path, '_blank');
            }
        });
    };

    $scope.fileSelectAll = function (checkValue) {
        angular.forEach($scope.fileList, function (data) {
            data.Checked = checkValue;
        });
    };

    $scope.checkItem = function (item, checkItemValue) {
        var index = $scope.fileList.indexOf(item);
        $scope.fileList[index].Checked = checkItemValue;
    };

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/FileList/GetFileList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.fileList = response.data;
            console.log($scope.fileList);
            $scope.fileFilterList = [];
            var control = false;
            angular.forEach($scope.fileList, function (data) {
                control = false;
                angular.forEach($scope.fileFilterList, function (data2) {
                    if (data2 === data.FileType && !control) {
                        control = true;
                    }
                });

                if (!control)
                    $scope.fileFilterList.push(data.FileType);
            });
        });
    };

    $scope.fireFileGallery = function () {
        $('.mix-grid').mixItUp();

        $('.mix-controls .select-all input').change(function () {
            if ($(this).is(":checked")) {
                $('.gallery').find('.mix').addClass('selected');
                enableGalleryTools(true);
            }
            else {
                $('.gallery').find('.mix').removeClass('selected');
                enableGalleryTools(false);
            }
        });

        $('.mix .img-select').click(function () {
            var mix = $(this).parents('.mix');
            if (mix.hasClass('selected')) {
                mix.removeClass('selected');
                enableGalleryTools(false);
            }
            else {
                mix.addClass('selected');
                enableGalleryTools(true);
            }
        });

        var enableGalleryTools = function (enable) {

            if (enable) {

                $('.mix-controls li.mix-control').removeClass('disabled');

            }
            else {
                var selected = false;

                $('.gallery .mix').each(function () {
                    if ($(this).hasClass('selected')) {
                        selected = true;
                    }
                });

                if (!selected) {
                    $('.mix-controls li.mix-control').addClass('disabled');
                }
            }
        }
    };

    $scope.clearValues = function () {
        $scope.rblRestriction = 0;
        $scope.fileTitle = '';
        $scope.fileName = '';
        $scope.fileImage = '';
        $scope.fileAll = '';

        //$scope.image_path = '';
        $scope.file_source = '';
    
    };

    $scope.validateSave = function () {

        if ($scope.fileTitle == '' || $scope.fileTitle == undefined
            || $scope.fileName == '' || $scope.fileName == undefined
            //|| $scope.image_path == '' || $scope.image_path == undefined
            || $scope.file_source == '' || $scope.file_source == undefined) {

            $scope.validateResult = false;
            $scope.validateErrorMessage = 'Lütfen Tüm Bilgileri Giriniz';
        } else {
            $scope.validateResult = true;
            $scope.validateErrorMessage = '';
        }
    };

    $scope.save = function () {


        $scope.validateSave();

        if ($scope.validateResult == false) {

            iziToast.error({
                message: $scope.validateErrorMessage,
                position: 'topCenter'
            });
        } else {

            fireCustomLoading(true);

            $http({
                method: "POST",
                url: "/Admin/FileList/SaveFile",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { filePathSelected: $scope.file_source, imageBaseIcon: $scope.image_sourceIcon, title: $scope.fileTitle, name: $scope.fileName, restriction: $scope.rblRestriction }
            }).then(function (response) {
           

                fireCustomLoading(false);


                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });

       
                location.reload();

                //$('#tFileNew').removeClass('active');
                //$('#tFileListLi > a').click();
                //$scope.clearValues();

                //$scope.loadData();
             

            });
        };
    };

    $scope.askForDelete = function (item) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () {
                        if (item != null)
                            $scope.delete(item);
                        else {
                            angular.forEach($scope.fileList, function (data) {
                                if (data.Checked) {
                                    $scope.delete(data);
                                }
                            });
                        }
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

    $scope.delete = function (item) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/FileList/DeleteFile",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: item.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            var index = $scope.fileList.indexOf(item);
            $scope.fileList.splice(index, 1);
        });
    };

    $(document).ready(function () {
        $scope.loadData();
    });

    $scope.$on('ngRepeatFileListFinished', function (ngRepeatFileListFinishedEvent) {
        $scope.fireFileGallery();
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