b2bApp.controller('suggestionRequestReportController', ['$scope','$http','$sce', function ($scope,$http,$sce) {



    $scope.trustDangerousSnippet = function (snippet) {
        return $sce.trustAsHtml(snippet);
    };


    $scope.suggestionRequestReport = {
        Email: '',
        Header :'',
        Message: '',
        PhoneNumber: '',
        ProductCode: '',
        SubjectTypeId: 0
    };

    $scope.saveSuggestionRequestReport = function () {
        
        if ( $scope.suggestionRequestReport.SubjectTypeId == 0 
            || ($scope.suggestionRequestReport.Email == '' || $scope.suggestionRequestReport.Email == undefined)
            || ($scope.suggestionRequestReport.PhoneNumber == '' || $scope.suggestionRequestReport.PhoneNumber == undefined)
            || ($scope.suggestionRequestReport.Header == '' || $scope.suggestionRequestReport.Header == undefined)
            || ($scope.suggestionRequestReport.Message == ''  || $scope.suggestionRequestReport.Message == undefined))
        {

            iziToast.error({
                message: 'Lütfen tüm alanları doldurunuz... ',
                position: 'topCenter'
            });
        }
        else if ($scope.suggestionRequestReport.SubjectTypeId === 2 && ($scope.suggestionRequestReport.ProductCode == '' ||$scope.suggestionRequestReport.ProductCode == undefined)) {
            iziToast.error({
                message: 'Lütfen önereceğiniz ürünün kodunu da yazınız... ',
                position: 'topCenter'
            });
        }
        else {
            fireCustomLoading(true);
            //POST
            $http({
                method: "POST",
                url: "/Contact/SaveSuggestionRequestReport",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {suggestionRequestReport : $scope.suggestionRequestReport}
            }).then(function (response) {
                fireCustomLoading(false);

                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });

                //Sıfırla
                if (response.data.Statu == 'success') {
                    $scope.suggestionRequestReport = angular.copy($scope.suggestionRequestReportDefault);
                } else {
                    iziToast.error({
                        message: 'Hata Oluştu',
                        position: 'topCenter'
                    });
                }

            });
        }
    };

    $scope.getListSuggestionType = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Contact/GetListSuggestionType",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.subjectTypeList = response.data;
            fireCustomLoading(false);
        });
    };
    $scope.getContactList=function() {
        $http({
            method: "POST",
            url: "/Contact/GetContactList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            $scope.cantactList = response.data;
           
        });
    }

    $(document).ready(function () {
        $scope.getContactList();
        $scope.getListSuggestionType();
        $scope.suggestionRequestReportDefault = angular.copy($scope.suggestionRequestReport);
    });

}]);