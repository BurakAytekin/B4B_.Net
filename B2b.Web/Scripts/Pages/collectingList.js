b2bApp.controller('CollectingListController', function ($scope, $http) {
    // #region veriables
   
    // #endregion

    $scope.getListSalesman = function() {
          fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/GetListSalesman",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function(response) {

            $scope.salesmanList = response.data;
              fireCustomLoading(false);

        });
    };
    $scope.$on('ngRepeatSalesmansFinished', function (ngRepeatVehicleModelFinishedEvent) {
        $('#salesmans')[0].sumo.reload();
    });
    $scope.getListCollecting = function () {
        var salesmanId,startDate, endDate;;
     
        var index = $("#salesmans")[0].selectedIndex;
        if (index > 0)
        {
             salesmanId =parseInt($("#salesmans").val());
        }
        else
        {
            $scope.showErrorMessage("Temsilci Seçimi Yapmalısınız.");
            return;
        }
        if (!$("#txtStartDate").val() || !$("#txtEndDate").val())
        {
            $scope.showErrorMessage("Tarih Seçimi Yapmalısınız.");
            return;
        }
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/GetListCollecting",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { salesmanId:salesmanId, startDate:startDate, endDate:endDate}

        }).then(function (response) {
            $scope.collectingList = response.data;
            fireCustomLoading(false);
        });
    };
    $scope.clearTable = function() {
        $scope.collectingList = {};
    };
    $scope.openDetail = function (id) {
        $scope.detailList = {};
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Collecting/GetCollectingDetail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id:id}

        }).then(function (response) {
            $scope.detailList = response.data;
            $('#modal-text').modal();
            fireCustomLoading(false);
        });
       
    };
    $scope.showErrorMessage = function (message) {
        iziToast.error({
            message: message,
            position: 'topRight'
        });
    }


    $(document).ready(function () {
        $('#salesmans').SumoSelect();
        $('#txtStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: "01/01/" + new Date().getFullYear(),
            locale: "tr"
        });

        $('#txtEndDate').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: new Date(),
            locale: "tr"
        });
        $scope.getListSalesman();

    });
   
}).directive('onFinishRender', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) { //ng repeat dönerken son kayıtmı diye bakıyorum
                $timeout(function () {
                    scope.$emit(attr.onFinishRender);
                });
            }
        }
    };
}).filter('convertDate', [
    '$filter', function ($filter) {
        return function (input, format) {
            //  return $filter('date')((new Date(parseInt(input.substr(6))),format(format)));
            return $filter('date')(new Date(parseInt(input.substr(6))), format);
        };
    }
]);