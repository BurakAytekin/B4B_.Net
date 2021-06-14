adminApp.controller('FAQController', ['$scope', '$http', function ($scope, $http) {


    var faqList;
    $scope.actionMode ='List';
    $scope.faq = {
        Id: 0,
        Question: '',
        Answer: ''
    };

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/faq/GetFAQList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {  }
        }).then(function (response) {
            $scope.faqList = response.data;
            faqList = $scope.faqList;
            fireCustomLoading(false);
        });
    };

    $scope.setActionMode = function (mode,row) {

        if (mode == 'Edit') {
            $scope.faq.Id = row.Id;
            $scope.faq.Question = row.Question;
            $scope.faq.Answer = row.Answer;
        }

        else if (mode == 'Add') {

            $scope.faq = angular.copy($scope.faqDefault);
        }

        else if (mode == 'Delete') {
            $scope.faq.Id = row.Id;
            $scope.faq.Question = row.Question;
            $('#modal-faq-delete').modal('show');
        }

        if (mode != 'Delete') {
            $scope.actionMode = mode;
        }
    };




    $scope.save = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Faq/Save",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { faq: $scope.faq }
        }).then(function (response) {
            fireCustomLoading(false);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            $scope.actionMode = 'List';

            $scope.loadData();
            $scope.clearValues();

        });
    };


    $scope.delete = function (faq) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Faq/Delete",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { faq: $scope.faq }
        }).then(function (response) {
            fireCustomLoading(false);
            $('#modal-faq-delete').modal('hide');
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.loadData();
            $scope.clearValues();
        });
    };

    $scope.search = function () {
        var searchList = faqList;
        searchList = searchList.filter(item=> item['Question'].indexOf($scope.searchValue) > -1 || item['Answer'].indexOf($scope.searchValue) > -1);
        $scope.faqList = searchList;
    };


    $scope.clearValues = function () {
        $scope.searchValue = '';
    };

    $(document).ready(function () {
        $scope.faqDefault = angular.copy($scope.faq);
        $scope.loadData();
    });


}]);