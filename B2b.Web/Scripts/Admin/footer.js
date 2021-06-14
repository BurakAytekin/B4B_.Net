adminApp.controller("footerController", ['$scope', 'NgTableParams', '$http', '$element', function ($scope, NgTableParams, $http, $element) {

    $scope.footerItem = null;

    $scope.saveFooter = function () {

        $scope.footerItem.ConditionsForReturn = $('#tConditionsForReturn .note-editable').html();
        $scope.footerItem.PrivacyPolicy = $('#tPrivacyPolicy .note-editable').html();
        $scope.footerItem.TermOfUse = $('#tTermOfUse .note-editable').html();
        $scope.footerItem.DistanceSalesContract = $('#tDistanceSalesContract .note-editable').html();
         $scope.footerItem.KvkkContract = $('#tKvkkContract .note-editable').html();
         $scope.footerItem.PaymentContract = $('#tPaymentContract .note-editable').html();

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Footer/SaveFooter",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { footerItem: $scope.footerItem }//, 

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

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Footer/GetFooterItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.footerItem = response.data;
            $('#tConditionsForReturn .note-editable').html($scope.footerItem.ConditionsForReturn)
            $('#tPrivacyPolicy .note-editable').html($scope.footerItem.PrivacyPolicy)
            $('#tTermOfUse .note-editable').html($scope.footerItem.TermOfUse)
            $('#tDistanceSalesContract .note-editable').html($scope.footerItem.DistanceSalesContract)
               $('#tKvkkContract .note-editable').html($scope.footerItem.KvkkContract)
               $('#tPaymentContract .note-editable').html($scope.footerItem.PaymentContract)
        });
    };

    $(document).ready(function () {
        $scope.loadData();
        $('#txtConditionsForReturn').summernote({
            height: 200   //set editable area's height
        });
        $('#txtPrivacyPolicy').summernote({
            height: 200   //set editable area's height
        });
        $('#txtermOfUse').summernote({
            height: 200   //set editable area's height
        });
        $('#txtDistanceSalesContract').summernote({
            height: 200   //set editable area's height
        });
          $('#txtKvkkContract').summernote({
            height: 200   //set editable area's height
        });

          $('#txtPaymentContract').summernote({
            height: 200   //set editable area's height
        });
    });

}]);