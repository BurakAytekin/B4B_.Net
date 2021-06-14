b2bApp.controller('MessageController', function ($scope, $http) {

    // #region Veriables
    $scope.messageList = null;
    // #endregion

    $scope.showMessageSend = function () {
        
        var whomText = '';
        if ($scope.CurrentSalesmanOfCustomer.length === 0) {
            whomText = $scope.CompanyInformation.Title;
        }
        else {
            for (var i = 0; i < $scope.CurrentSalesmanOfCustomer.length ; i++) {
                whomText += $scope.CurrentSalesmanOfCustomer[i].Salesman.Code + ',';
            }

            whomText = whomText.substring(0, whomText.length-1);
        }
        
        $('#txtWhom').val(whomText);

        $('#myModal').modal();
    };
    $scope.keyPressedSearch = function (e, textValue) {
        var key, brwsr, keyCtrl = false, keyShift = false, keyAlt = false, keyMeta = false;

        if (window.event) { // Internet Explorer
            if (window.event.shiftKey) keyShift = true;
            if (window.event.ctrlKey) keyCtrl = true;
            if (window.event.metaKey) keyMeta = true;
            if (window.event.altKey) keyAlt = true;
            key = window.event.keyCode;
            brwsr = true;
        }
        else if (e) { // Firefox
            if (e.shiftKey) keyShift = true;
            if (e.ctrlKey) keyCtrl = true;
            if (e.metaKey) keyMeta = true;
            if (e.altKey) keyAlt = true;
            key = e.which;
            brwsr = false;
        }
        else { // Other Browsers
            if (event.shiftKey) keyShift = true;
            if (event.ctrlKey) keyCtrl = true;
            if (event.metaKey) keyMeta = true;
            if (event.altKey) keyAlt = true;
            key = event.keyCode;
            brwsr = false;
        }

        function fireReturnFalse(e) {
            if (e.preventDefault) {
                e.preventDefault();
                e.stopPropagation();
            }
            else
                e.returnValue = false;

            if (brwsr === true) {
                window.event.returnValue = false;
                event.keyCode = 0;
            }
            else {
                e.cancelbubble = true;
                e.returnvalue = false;
                e.keycode = false;
            }

            return false;
        }

        function fireReturnTrue() {
            return true;
        }

        if (key === 13) {

            $scope.getMessageList($scope.activeMessagebox, textValue);
            fireReturnFalse(e);
        }
        else
            fireReturnTrue();

       
    };
    $scope.sendMessage = function () {
        var header = $('#txtHeader').val();
        var content = $('#txtContent').val();

        $http({
            method: "POST",
            url: "/Message/SendMessage",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { header: header, content: content }

        }).then(function (response) {
            iziToast.success({
                message: 'Mesajınız iletilmiştir. ',
                position: 'topCenter'
            });

            $('#myModal').modal('hide');
        });
    };
    $scope.getMessageList = function (messagebox,searchText) {
        $scope.activeMessagebox = messagebox;
        $http({
            method: "POST",
            url: "/Message/GetMessageList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { messagebox: messagebox, searchText: searchText }

        }).then(function (response) {
           
            $scope.messageList = response.data;

        });
    };
    $scope.checkMessageItem = function (value, id) {

        $http({
            method: "POST",
            url: "/Message/CheckMessageItem",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, value: value }

        }).then(function (response) {

        });

    };
    $scope.checkMessageAll = function (value) {

        $http({
            method: "POST",
            url: "/Message/CheckMessageAll",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {value: value }

        }).then(function (response) {

            for (var i = 0; i < response.data.length; i++) {
                $('#chk' + response.data[i].Id).prop("checked", value);
            }

        });

    };
    $scope.askForAdd = function (id, qnt) {
        $.confirm({
            title: 'Uyarı!',
            content: "Bu üründen sepetinizde " + qnt + " adet bulunmaktadır. Sepetinize eklemek istiyor musunuz?",
            buttons:
                {
                    EVET: {
                        btnClass: 'btn-blue',
                        action: function () { $scope.addBasket(id); }

                    },
                    HAYIR: {
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Ekleme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
        });
    };


    $(document).ready(function () {
        $scope.CurrentCustomer = currentCustomer;
        $scope.CurrentSalesmanOfCustomer = currentSalesmanOfCustomer;
        $scope.CompanyInformation = companyInformation;
        $scope.activeMessagebox = 0;
        $scope.getMessageList(0, '*');

    });



});