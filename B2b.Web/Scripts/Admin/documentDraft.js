adminApp.controller("documentDraft", ['$scope', 'NgTableParams', '$http', '$element', function ($scope, NgTableParams, $http, $element) {
    $scope.documentDraft = {};
    $scope.documentDraftDefault = {};
    $(document).ready(function () {


        $('#content-Email').summernote({
            height: 300,
            disableDragAndDrop: true
        });
        $('#content-Header').summernote({
            height: 300,
            disableDragAndDrop: true
        });
        $('#content-Content').summernote({
            height: 300,
            disableDragAndDrop: true
        });
        $('#content-Footer').summernote({
            height: 300,
            disableDragAndDrop: true
        });


        $scope.loadData();
        $scope.documentDraft.HeaderHeight = 55;
        $scope.documentDraft.FooterHeight = 20;
        //$("#txtConntent").on('summernote.blur', function () {
        //    $('#txtConntent').html($('#txtConntent').summernote().code());
        //});
    });

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/DocumentDraft/GetDocumentDraftItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: pageType, defaultData: true }
        }).then(function (responseDefault) {
            fireCustomLoading(false);
            $scope.documentDraftDefault = responseDefault.data;

            $http({
                method: "POST",
                url: "/Admin/DocumentDraft/GetDocumentDraftItem",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { type: pageType, defaultData: false }
            }).then(function (response) {
                $scope.documentDraft = response.data;
                if ($scope.documentDraft.Id === 0) {
                    $scope.documentDraft = angular.copy($scope.documentDraftDefault);
                    $scope.documentDraft.Id = 0;
                    $scope.documentDraft.DefaultData = 0;
                }


                $('#content-Email').summernote().code($scope.documentDraft.EmailBody);
                $('#content-Header').summernote().code($scope.documentDraft.HeaderBody);
                $('#content-Content').summernote().code($scope.documentDraft.ContentBody);
                $('#content-Footer').summernote().code($scope.documentDraft.FooterBody);

                $('#isSendEmail').prop("checked", $scope.documentDraft.IsSendEmail);
                $('#isSendSalesman').prop("checked", $scope.documentDraft.IsSendSalesman);
                $('#isSendCustomer').prop("checked", $scope.documentDraft.IsSendCustomer);
                $('#isPageNumber').prop("checked", $scope.documentDraft.IsPageNumber);
                $('#isDisplayHeader').prop("checked", $scope.documentDraft.IsDisplayHeader);
                $('#isDisplayFooter').prop("checked", $scope.documentDraft.IsDisplayFooter);

            });


        });
    };

    $scope.clearValues = function () {
        $scope.documentDraft = {};
        $('#content-Email').summernote().code('');
        $('#content-Header').summernote().code('');
        $('#content-Content').summernote().code('');
        $('#content-Footer').summernote().code('');

        $('#isShowCommentUser').prop("checked", true);
        $('#isSendSalesman').prop("checked", true);
        $('#isSendCustomer').prop("checked", true);
        $('#isPageNumber').prop("checked", true);
        $('#isDisplayHeader').prop("checked", true);
        $('#isDisplayFooter').prop("checked", true);
        $scope.documentDraft.HeaderHeight = 55;
        $scope.documentDraft.FooterHeight = 20;
    };

    $scope.setDefault = function (type) {
        if (type === 0) {
            $scope.documentDraft.EmailBody = angular.copy($scope.documentDraftDefault.EmailBody);
            $('#content-Email').summernote().code($scope.documentDraft.EmailBody);
        }
        else {
            $scope.documentDraft.ContentBody = angular.copy($scope.documentDraftDefault.ContentBody);
            $('#content-Content').summernote().code($scope.documentDraft.ContentBody);
        }


    };

    $scope.savePdf = function (type) {
        fireCustomLoading(true);
        $scope.documentDraft.EmailBody = $('#content-Email').summernote().code();
        $scope.documentDraft.HeaderBody = $('#content-Header').summernote().code();
        $scope.documentDraft.ContentBody = $('#content-Content').summernote().code();
        $scope.documentDraft.FooterBody = $('#content-Footer').summernote().code();

        $scope.documentDraft.IsSendEmail = $('#isSendEmail').prop('checked');
        $scope.documentDraft.IsSendSalesman = $('#isSendSalesman').prop('checked');
        $scope.documentDraft.IsSendCustomer = $('#isSendCustomer').prop('checked');
        $scope.documentDraft.IsPageNumber = $('#isPageNumber').prop('checked');
        $scope.documentDraft.IsDisplayHeader = $('#isDisplayHeader').prop('checked');
        $scope.documentDraft.IsDisplayFooter = $('#isDisplayFooter').prop('checked');

        $http({
            method: "POST",
            url: "/Admin/DocumentDraft/SavePdf",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { documentDraft: $scope.documentDraft, type: type }//, 
        }).then(function (response) {
            $scope.frameUrl = response.data;

            if (type) {
                $scope.loadData();
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
            }
            else
                $('#mPdfShow').appendTo("body").modal('show');

            fireCustomLoading(false);

        });
    };

}]);