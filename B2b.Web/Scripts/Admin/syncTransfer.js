adminApp.controller('syncTransferController', ['$scope', '$http', 'NgTableParams', '$element', function ($scope, $http, NgTableParams, $element) {

    $scope.value = 0;
    $scope.isRunning = false;


    $scope.fireRefresh = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/FireRefresh",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $('#logShow').html('');
        });
    };


    $scope.bindAllData = function () {
        $scope.value = 20;
        $('#prCustomer').attr('style', ('width:' + $scope.value + '%'));
    }

    $scope.getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };

    $scope.firstalue = 0;

    $scope.checkTransfer = function () {

        $http({
            method: "POST",
            url: "/Admin/Sync/CheckTransfer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            var result = response.data;

            $scope.isRunning = result.Item1;

            //var _error = false;
            //$('#pr' + result.SettingsId).attr('style', ('width:' + result.ProgressValue + '%'));
            //$('#btn' + result.SettingsId).attr('disabled', 'disabled');
            for (var i = $scope.firstalue; i < result.Item2.Log.length; i++) {
                if (result.Item2.Log[i].Type === 0)
                    $('#logShow').prepend(result.Item2.Log[i].Message);
                else {
                    $('#logShow').prepend('<span style="color:red">' + result.Item2.Log[i].Message + '</span>');

                }

            }
            $scope.firstalue = result.Item2.Log.length;
            //if (result.Log[result.Log.length - 1].Type === 1) {
            //    $('#pr' + result.SettingsId).removeClass('progress-bar-orange').removeClass('progress-bar-green').addClass('progress-bar-red');
            //    $('#btn' + result.SettingsId).prop('disabled', '');
            //    $('#pr' + result.SettingsId).attr('style', ('width:100%'));
            //}
            //else if (result.Status === 1 || result.ProgressValue === 100) {
            //    $('#pr' + result.SettingsId).removeClass('progress-bar-orange').addClass('progress-bar-green');
            //    $('#btn' + result.SettingsId).prop('disabled', '');
            //}
            //else {

            //}
            //$('#prCustomer1').attr('style', ('width:' + $scope.value + '%'));progress-bar-red

        });
    };

    $scope.showTransferLogModal = function (row) {

        fireCustomLoading(false);
        $('#modal-transferLog').modal('show');

    };

    $scope.getTransferLog = function () {
        var minDAte = $('#iTransferStartDate').val();
        var maxDate = $('#iTransferEndDate').val();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/GetTransferLog",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { minDate: minDAte, maxDate: maxDate }
        }).then(function (response) {

            $scope.transferLog = response.data;

            if ($scope.transferLog.length < 1) {
                iziToast.error({
                    message: 'Veri Bulunamadı.',
                    position: 'topCenter'
                });

            }


            fireCustomLoading(false);

        });
    };


    $scope.fireResponse = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/FireResponse",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { item: row }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            //            setInterval(
            //    function () {
            //       // $scope.checkTransfer();

            //    },
            //    2000
            //);
        });
    };


    $scope.setValues = function (type) {
        if ($scope.trasnferTypeList.length === 0)
            return;

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/SetValues",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { type: type, item: $scope.trasnferTypeList[0] }//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.checkTransfer();
        });
    };

    $scope.loadTransferTypeData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Sync/GetActiveTrasnferTypeList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}//, 
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.trasnferTypeList = response.data;
        });
    };

    function setDefaultDate() {
        var today = new Date();
        var dStart = '01/01/' + today.getFullYear();
        $('#iTransferStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: dStart,
            locale: "tr"
        }).val(dStart);

        $('#iTransferEndDate').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    };


    $(function () {
        $scope.loadTransferTypeData();

        setDefaultDate();

        setInterval(function () {$scope.checkTransfer();},5000);

    });

    $scope.$on('ngRepeatTransferTypeFinished', function (ngRepeatTransferTypeFinishedEvent) {

        //angular.forEach($scope.trasnferTypeList, function (value, key) {

        //    //$("#getting-started" + value.TransferTypeId).stop();
        //    //$("#getting-started" + value.TransferTypeId).countdown({
        //    //    startTime: value.StartTime,
        //    //    format: 'hh:mm:ss',
        //    //    digitImages: 6,
        //    //    digitWidth: 28,
        //    //    digitHeight: 40,
        //    //    timerEnd: function () {
        //    //      //  $('#logShow').html('');
        //    //       //  location.reload();
        //    //        $scope.loadTransferTypeData();

        //    //    },
        //    //    image: '../../Content/images/digits40.png'

        //    //});

        //});

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
}).filter("dateFilter", function () {
    return function (item) {
        if (item !== null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});
