adminApp.controller("paymentListController", ['$scope', 'NgTableParams', '$http', '$filter', function ($scope, NgTableParams, $http, $filter) {
    $scope.paymentSearchCriteria =
        {
            T9Text: '',
            PaymentStatu: 1,
            StartDate: '',
            EndDate: ''
        };

    $scope.showPdf = function (item) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Payment/SavePdf",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { eItem: item }//, 
        }).then(function (response) {
            $scope.frameUrl = response.data;

            $('#mPdfShow').appendTo("body").modal('show');

            fireCustomLoading(false);

        });
    }

    $scope.tableLinkedShowDetail = function (row) {
        $scope.selectedPayment = row;
        $('#mPaymentDetail').appendTo("body").modal('show');
    };

    $scope.paymentSearch = function (paymentSearchCriteria) {
        fireCustomLoading(true);
        paymentSearchCriteria.StartDate = $('#iPaymentStartDate').val();
        paymentSearchCriteria.EndDate = $('#iPaymentEndDate').val();

        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Payment/GetListPayment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { paymentSearchCriteria: paymentSearchCriteria } //, 

        }).then(function (response) {

            $scope.tableOrdersParams = new NgTableParams({
                count: response.data.length === 0 ? 1 : response.data.length // hides pager
            }, {
                    filterDelay: 0,
                    counts: [],
                    dataset: angular.copy(response.data)
                });

            $scope.tablePaymentOrjinalData = angular.copy(response.data);
            fireCustomLoading(false);

        });
    };

    $scope.ConvertDate = function (input) {
        if (input)
            return $filter('date')(new Date(input), 'dd/MM/yyyy');
        else
            return "-";
    };

    $scope.ConvertDateTime = function (input) {
        if (input)
            return $filter('date')(new Date(input), 'dd/MM/yyyy HH:mm:ss');
        else
            return "-";
    };

    $scope.askForDelete = function (row, type, status) {
        if (!type) {
            $.confirm({
                title: 'Uyarı!',
                content: "İptal etmek istediğinize emin misiniz?",
                buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.updatepaymentStatus(row, true, status); }

                    },
                    Cancel: {
                        text: "Hayır",
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

            $scope.updatepaymentStatus(row.Id, status);
        }

    };

    $scope.updatepaymentStatus = function (id, status) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Payment/UpdatePaymentStatus",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, status: status }

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.paymentSearch($scope.paymentSearchCriteria);
        });
    };

    $scope.keypressEventPaymentSearch = function (e, paymentSearchCriteria) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 46)
            e.returnValue = false;
        if (key === 13) {
            $scope.paymentSearch(paymentSearchCriteria);
        }
    }

    function setDefaultDate() {
        var today = new Date();
        var dStart = '01/01/' + today.getFullYear();
        $('#iPaymentStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: dStart,
            locale: "tr"
        }).val(dStart);

        $('#iPaymentEndDate').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    }

    $scope.clear = function () {
        $scope.paymentSearchCriteria = {};
        $scope.paymentSearchCriteria =
            {
                T9Text: '',
                PaymentStatu: 1,
                StartDate: '',
                EndDate: ''
            };
        setDefaultDate();
    }

    $(window).load(function () {
        setDefaultDate();
    });

    $scope.exportToExcel = function () {

        $scope.paymentList = $scope.tableOrdersParams.data;


        if ($scope.paymentList.length > 0) {
            var newData = [];
            $scope.paymentList.forEach(function (e) {

                //Örnek tarih formatı    
                if (e.CreateDate == null || e.CreateDate == '' || e.CreateDate == undefined) {
                    e.CreateDate = '';
                } else {
                    e.CreateDate = moment(e.CreateDate).format('DD/MM/YYYY');
                };

                if (e.ConfirmDate == null || e.ConfirmDate == '' || e.ConfirmDate == undefined) {
                    e.ConfirmDate = '';
                } else {
                    e.ConfirmDate = moment(e.ConfirmDate).format('DD/MM/YYYY');
                };


                newData.push({
                    "No": e.Id,
                    "Kod": e.Customer.Code,
                    "Adı": e.Customer.Name,
                    "Ad Soyad": e.NameSurname,
                    "Kart Numarası": e.CardNumber,
                    "Tutar": e.Amount,
                    "Taksit": e.Installment,
                    "Provizyon No": e.AuthCode,
                    "Kullanılan Pos": e.UseEPaymentType,
                    "Kullanılan Kart": e.BankName,
                    "Mesaj": e.Result,
                    "Çekim Tarihi": e.ProcessingDate,
                });
            });

            var ws = XLSX.utils.json_to_sheet(newData, {
                header: [
                    "No",
                    "Kod",
                    "Adı",
                    "Ad Soyad",
                    "Kart Numarası",
                    "Tutar",
                    "Taksit",
                    "Provizyon No",
                    "Kullanılan Pos",
                    "Kullanılan Kart",
                    "Mesaj",
                    "Çekim Tarihi"
                ]
            });

            //Set Columns Size !
            var wscols = [{ wch: 5 }, { wch: 15 }, { wch: 60 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
            ws['!cols'] = wscols;

            var wb = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(wb, ws, "Sanalpos_İşlem_Listesi");


            XLSX.utils.shee;
            var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
            saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Sanalpos_İşlem_Listesi.xlsx");
        }
    };
}
]);




