adminApp.controller('PictureImportController', ['$scope', '$http', 'NgTableParams', function ($scope, $http, NgTableParams) {

$(document).ready(function () { fileUploads(); });


function fileUploads() {

    var manualUploader = new qq.FineUploader({
        element: document.getElementById('fine-uploader-manual-trigger'),
        template: 'qq-template-manual-trigger',
        success: OnComplete,
        error: OnFail,
        request: {
            endpoint: 'FilesUpload/ImportPicture?productId=-1&watermark=' + $scope.txtwatermark
        },

        callbacks: {
            onComplete: function (id, name, response) {

                if (response.success) {
                    $scope.getPictureList();
                    iziToast.show({
                        message: "Yüklemeleriniz Gerçekleşti.",
                        position: 'topCenter',
                        color: "success",
                        icon: "fa fa-check"
                    });
                }
            }
        },

        thumbnails: {
            placeholders: {
                waitingPath: '/Content/fine-uploader/images/waiting-generic.png',
                notAvailablePath: '/Content/fine-uploader/images/not_available-generic.png'
            }
        },
        autoUpload: false,
        debug: true
    });

    qq(document.getElementById("trigger-upload")).attach("click", function () {
        manualUploader.uploadStoredFiles();
    });
};

function OnComplete(result) {
    alert('Success');
}

function OnFail(result) {
    alert('Request failed');
}

}]);
