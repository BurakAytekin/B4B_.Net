adminApp.controller('IndexController', function ($scope, $http, NgTableParams) {

    $scope.lockUpdate = true;
    $scope.validateResult = false;
    $scope.validateMessage = '';
    //$scope.selectedSalesman = { Id: 0 };

    $scope.showEmailSendScreen = function () {

        $scope.userEmail = $scope.selectedSalesman.Email;
        $('#modal-EmailShow').appendTo("body").modal('show');
    };

    $scope.sendPasswordResetMail = function () {
        if ($scope.userEmail === '') {
            iziToast.error({
                message: 'Lütfen mail adresi giriniz',
                position: 'topCenter',
            });
            return;
        }

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/SendPasswordResetMail",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { user: $scope.selectedSalesman, email: $scope.userEmail }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);
            $('#modal-EmailShow').appendTo("body").modal('hide');

        });
    };
    $scope.getCountFillData = function () {
        $scope.userCountList = [
            { Count: 0 }, { Count: 1 }, { Count: 2 }, { Count: 3 }, { Count: 4 }, { Count: 5 }, { Count: 6 }, { Count: 7 }, { Count: 8 }, { Count: 9 }, { Count: 10 }, { Count: 11 }, { Count: 12 }, { Count: 13 }, { Count: 14 }, { Count: 15 }, { Count: 16 }, { Count: 17 }, { Count: 18 }, { Count: 19 }, { Count: 20 }];
    }

    $scope.changeNewLockUpdate = function () {
        $scope.lockUpdate = false;
        $scope.selectedSalesman = { IntervalTime: 300 };
        $scope.getCountFillData();
    }

    $scope.deleteSalesman = function (type) {
        if ($scope.selectedSalesman === {} || $scope.selectedSalesman === null || $scope.selectedSalesman.Id === 0 || $scope.selectedSalesman.Id === undefined)
            return;

        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "Silmek istediğinize emin misiniz?",
                buttons:
                {
                    EVET: {
                        btnClass: 'btn-blue',
                        action: function () { $scope.deleteSalesman(true); }

                    },
                    HAYIR: {
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
        }
        else {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Salesmans/DeleteSalesman",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { salesman: $scope.selectedSalesman }
            }).then(function (response) {
                fireCustomLoading(false);
                $scope.clear();
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
            });
        }
    };



    $scope.changeLockUpdate = function () {
        $scope.lockUpdate = false;
        $scope.orjinalSelectedSalesman = $scope.selectedSalesman;
    }

    $scope.cancelLockUpdate = function () {
        $scope.lockUpdate = true;
        $scope.selectedSalesman = $scope.orjinalSelectedSalesman;
    }

    $scope.getAuthoritySalesman = function () {
        if ($scope.selectedSalesman === {} || $scope.selectedSalesman.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/GetAuthoritySalesman",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedSalesman.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.fieldList = response.data.FieldList;
            $scope.authorityItem = response.data.AuthorityItem;
        });
    };

    $scope.setAuthenticatorValue = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/SaveSalesman",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { salesman: $scope.selectedSalesman }
        }).then(function (response) {
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            fireCustomLoading(false);

        });
    }

    $scope.createAuthenticator = function (item, type) {
        fireCustomLoading(true);
        if (type === 1) {
            $.confirm({
                title: 'Uyarı!',
                content: "Qr Code görselini yenilediğinizde kullanıcının görseli tekrar okutması gerekmektedir. Devam etmek istediğinize emin misiniz?",
                buttons:
                {
                    EVET: {
                        btnClass: 'btn-blue',
                        action: function () {

                            $http({
                                method: "POST",
                                url: "/Admin/Salesmans/CreateAuthenticator",
                                headers: { "Content-Type": "Application/json;charset=utf-8" },
                                data: { salesman: item, type: type }
                            }).then(function (response) {
                                iziToast.show({
                                    message: response.data.Message,
                                    position: 'topCenter',
                                    color: response.data.Color,
                                    icon: response.data.Icon
                                });
                                $scope.getAuthenticatorImage();

                                fireCustomLoading(false);
                                });
                        }

                    },
                    HAYIR: {
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
        }
        else {
            $http({
                method: "POST",
                url: "/Admin/Salesmans/CreateAuthenticator",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { salesman: item, type: type }
            }).then(function (response) {
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $scope.getAuthenticatorImage();

                fireCustomLoading(false);
            });
        }



    };

    $scope.getAuthenticatorImage = function () {
        $http({
            method: "POST",
            url: "/Admin/Salesmans/GetSalesmanAuthenticatorImage",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedSalesman.Id }
        }).then(function (response) {
            $scope.selectedSalesman = response.data.Salesman;
            $scope.authenticatorQrCode = response.data.QrCode;

        });
    }

    $scope.checkAuthoritySalesman = function (field) {
        var updateValue = $scope.authorityItem[0][field];
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/UpdateAuthoritySalesman",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.authorityItem[0].Id, updateValue: updateValue, field: field }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.setFileImage = function (element) {
        $scope.currentFile = element.files[0];
        var reader = new FileReader();
        reader.onload = function (event) {
            $scope.image_sourceIcon = event.target.result;
            $scope.$apply()
        }
        // when the file is read it triggers the onload event above.
        reader.readAsDataURL(element.files[0]);
    };

    $scope.fileImageSave = function () {
        if ($scope.selectedSalesman === {} || $scope.searchCriteriaSalesman.Code === undefined || $scope.searchCriteriaSalesman.Code === "")
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/saveSalesmanImage",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedSalesman.Id, imageBase: $scope.image_sourceIcon }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.getCustomerList = function () {
        if ($scope.selectedSalesman === {} || $scope.searchCriteriaSalesman.Code === undefined || $scope.searchCriteriaSalesman.Code === "")
            return;

        fireCustomLoading(true);


        $http({
            method: "POST",
            url: "/Admin/Salesmans/GetCustomerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: $scope.selectedSalesman.Id }
        }).then(function (response) {

            $scope.SalesmanOfCustomerList = response.data;

            $scope.tableCustomerListParams = new NgTableParams({
                count: response.data.length // hides pager
            }, {
                    filterDelay: 0,
                    counts: [],
                    dataset: angular.copy($scope.SalesmanOfCustomerList)
                });
            fireCustomLoading(false);

        });
    };

    $scope.saveSalesmanValidate = function () {

        if (($scope.searchCriteriaSalesman.Code == undefined || $scope.searchCriteriaSalesman.Code == '') ||
            ($scope.searchCriteriaSalesman.Name == undefined || $scope.searchCriteriaSalesman.Name == '')) {
            $scope.validateResultMessage = 'Temsilci Adını ve Kodunu Doldurunuz';
        }

        else if ($scope.selectedSalesman.NumberOfUser === "" || $scope.selectedSalesman.NumberOfUser === undefined) {
            $scope.validateResultMessage = 'Kullanıcı Sayısı Seçiniz';
        }

        else if ($scope.selectedSalesman.NumberOfUserModerator === "" || $scope.selectedSalesman.NumberOfUserModerator === undefined) {
            $scope.validateResultMessage = 'Yönetici Kullanıcı Sayısı Seçiniz';
        }

        else if ($scope.selectedSalesman.NumberOfUserAndroid === "" || $scope.selectedSalesman.NumberOfUserAndroid === undefined) {
            $scope.validateResultMessage = 'Android Kullanıcı Sayısı Seçiniz';
        }

        else if ($scope.selectedSalesman.Password === "" || $scope.selectedSalesman.Password === undefined) {
            $scope.validateResultMessage = 'Şifre Belirleyiniz';
        }

        else {
            $scope.validateResult = true;
            $scope.validateResultMessage = ''
        }

    };

    $scope.saveSalesman = function () {

        $scope.selectedSalesman.Name = $scope.searchCriteriaSalesman.Name;
        $scope.selectedSalesman.Code = $scope.searchCriteriaSalesman.Code;
        var salesman = $scope.selectedSalesman;

        //Validate İŞLEMLERİ
        $scope.saveSalesmanValidate();

        if ($scope.validateResult == false) {
            iziToast.error({
                message: $scope.validateResultMessage,
                position: 'topCenter'
            });
        } else if ($scope.validateResult == true) {

            fireCustomLoading(true);

            $http({
                method: "POST",
                url: "/Admin/Salesmans/SaveSalesman",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { salesman: salesman }
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
    };


    $scope.$watch('selectedCustomer', function (newValue, oldValue) {
        if (newValue != undefined && newValue.Id != undefined && newValue !== oldValue) {
            var selectedcustomer = newValue;
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Salesmans/ConnectCustomer",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { salesmanId: $scope.selectedSalesman.Id, customerId: selectedcustomer.Id }
            }).then(function (response) {

                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });
                $scope.getCustomerList();
                fireCustomLoading(false);
            });
        }

    }, true);

    $scope.setSessionAutoLock = function () {

        if ($scope.selectedSalesman.IsAutoLock === false) {
            $scope.selectedSalesman.IntervalTime = 0;
        };
    };

    $scope.clear = function () {

        $scope.lockUpdate = true;
        $scope.selectedSalesman = {};
        $scope.selectedCustomer = {};
        $scope.searchCriteriaSalesman = {};
        $scope.searchCriteria = {};
    };


    $scope.exportToExcel = function () {


        if ($scope.SalesmanOfCustomerList.length > 0) {
            var newData = [];
            $scope.SalesmanOfCustomerList.forEach(function (e) {


                newData.push({
                    "Cari Kodu": e.Code,
                    "Cari Ünvanı": e.Name,
                    "Adres": e.Address,
                    "İl": e.City,
                    "Tel": e.Tel1,
                    "Mail": e.Mail
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                    "Cari Kodu",
                    "Cari Ünvanı",
                    "Adres",
                    "İl",
                    "Tel",
                    "Mail"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 15 }, { wch: 45 }, { wch: 30 }, { wch: 15 }, { wch: 15 }, { wch: 40 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Temsilci_Musterileri");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), $scope.searchCriteriaSalesman.Name + " Temsilci_Musterileri.xlsx");
        }
    };


    $(document).ready(function () {

    });

}).directive('customSalesmanSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Salesmans/SalesmansSelect",
        controller: 'SalesmanSelectController'
    };
}).directive('customCustomerSelect', function () {
    return {
        restrict: 'E',//3 Paranetre Alır A:Attiribute E:Element C:Class Tektek veya hepsi aynı anda yazılabilir.
        templateUrl: "/Admin/Customers/CustomersSelect",
        controller: 'CustomerSelectController'
    };
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
    }
}).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});
