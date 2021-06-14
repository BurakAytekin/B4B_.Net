adminApp.controller("suggestionRequestController", ['$scope', 'NgTableParams', '$http', '$element', function ($scope, NgTableParams, $http, $element) {
   
    
    $scope.saveContact = function () {
        $scope.contactItem.MapPath = $scope.location;
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Admin/Contact/SaveContact",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { contact: $scope.contactItem, imageBase: $scope.image_source }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);

        });
    };

    $scope.openDetailModal = function (row) {
        $scope.selectedSuggestionRequest = [];
        $scope.selectedSuggestionRequest = row;
        $('#modal-contact-request').modal('show');
    };

    $scope.sendAnswer = function () {
        fireCustomLoading(true);
       
        $http({
            method: "POST",
            url: "/Admin/Contact/SaveSuggestionRequestReportAnswer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { Id: $scope.selectedSuggestionRequest.Id, Answer: $scope.selectedSuggestionRequest.Answer}
        }).then(function (response) {


            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);

            $scope.loadData();

        });
    };

    $scope.loadData = function () {
        fireCustomLoading(true);

        $http({
            method: "POST",
            url: "/Admin/Contact/GetListSuggestionRequestReport",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.customerRequestList = response.data.list;

     

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.customerRequestList)
            });
           
        });
    };

    $(document).ready(function () {
        $scope.loadData();
    });

}]);