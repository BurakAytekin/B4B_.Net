adminApp.controller("ReturnProcessController", [
    '$scope', 'NgTableParams', '$http', '$filter', function ($scope, NgTableParams, $http, $filter) {

        $scope.collectSearchCriteria =
            {
                Text: '',
                CollectStatu: -1,
                StartDate: '',
                EndDate: ''
            };

        $scope.CollectSearch = function (collectSearchCriteria) {
            collectSearchCriteria.StartDate = $('#iCollectStartDate').val();
            collectSearchCriteria.EndDate = $('#iCollectEndDate').val();
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/ReturnProcess/GetListReturnForm",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { collectSearchCriteria: collectSearchCriteria }
            }).then(function (response) {

                $scope.collectList = response.data;
                $scope.tableCollectsParams = new NgTableParams({
                    count: $scope.collectList.length === 0 ? 1 : $scope.collectList.length // hides pager
                },
                    {
                        filterDelay: 0,
                        counts: [],
                        dataset: angular.copy($scope.collectList)
                    });

                $scope.tableCollectsOrjinalData = angular.copy($scope.collectList);
                $scope.tableCollectdelete = $scope.tableKitdelete;
                fireCustomLoading(false);

            });
        };


        $scope.showPdf = function (item) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/ReturnProcess/SavePdf",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { eItem: item }//, 
            }).then(function (response) {
                $scope.frameUrl = response.data;

                $('#mPdfShow').appendTo("body").modal('show');

                fireCustomLoading(false);

            });

        };


        $scope.updateCollectStatus = function (item, status) {
            if (status === -1)
                item.Deleted = true;
            else {
                item.Status = status;
            }

            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/ReturnProcess/UpdateReturnProcessStatus",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { item: item }

            }).then(function (response) {
                fireCustomLoading(false);
                iziToast.show({
                    message: response.data.Message,
                    position: 'topCenter',
                    color: response.data.Color,
                    icon: response.data.Icon
                });

                $scope.CollectSearch($scope.collectSearchCriteria);
            });
        };
        

        $scope.ConvertDate = function (input) {
            if (input)
                return $filter('date')(new Date(input), 'dd/MM/yyyy');
            else
                return "-";
        };
        

        $scope.keypressEventCollectSearch = function (e, collectSearchCriteria) {
            var key;
            if (window.event)
                key = window.event.keyCode; //IE
            else
                key = (e.which) ? e.which : event.keyCode; //Firefox

            if (key === 46)
                e.returnValue = false;

            if (key === 13) {
                $scope.CollectSearch(collectSearchCriteria);
            }
        };

        function setDefaultDate() {
            var today = new Date();
            var dStart = '01/01/' + today.getFullYear();
            $('#iCollectStartDate').datetimepicker({
                format: "DD/MM/YYYY",
                defaultDate: dStart,
                locale: "tr"
            }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());;

            $('#iCollectEndDate').datetimepicker({
                format: 'DD/MM/YYYY',
                locale: 'tr'
            }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
        };

        $scope.clear = function () {
            $scope.collectSearchCriteria = {};
            $scope.collectSearchCriteria =
                {
                    T9Text: '',
                    CollectStatu: 1,
                    StartDate: '',
                    EndDate: ''
                };
            setDefaultDate();
        };

        $(window).load(function () {
            setDefaultDate();
        });

        $scope.exportToExcel = function () {


            if ($scope.collectList.length > 0) {
                var newData = [];
                $scope.collectList.forEach(function (e) {

                    //Örnek tarih formatı    
                    if (e.CreateDate == null || e.CreateDate == '' || e.CreateDate == undefined) {
                        e.CreateDate = '';
                    } else {
                        e.CreateDate = moment(e.CreateDate).format('DD/MM/YYYY');
                    };



                    newData.push({
                        "Durum": e.StatusStr ,
                        "No": e.Id,
                        "Tarih": e.CreateDate,
                        "Cari Kodu": e.Customer.Code,
                        "Cari İsim": e.Customer.Name,
                        "Ürün Kodu": e.ProductCode,
                        "Ürün Adı": e.ProductName,
                        "Üretici": e.Manufacturer,
                        "Fatura No": e.InvoiceNumber,
                        "Miktar": e.Quantity,
                        "Fiyat": e.PriceStr,
                       

                    });
                });

                var ws = XLSX.utils.json_to_sheet(newData, {
                    header: [
                        "Durum",
                        "No",
                        "Tarih",
                        "Cari Kodu",
                        "Cari İsim",
                        "Ürün Kodu",
                        "Ürün Adı",
                        "Üretici",
                        "Fatura No",
                        "Miktar",
                        "Fiyat",
                    ]
                });

                //Set Columns Size !
                var wscols = [{ wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
                ws['!cols'] = wscols;

                var wb = XLSX.utils.book_new();
                XLSX.utils.book_append_sheet(wb, ws, "İade_Listesi");


                XLSX.utils.shee;
                var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
                saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "İade_Listesi.xlsx");
            }
        };


    }]).directive('onFinishRender', function ($timeout) {
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