adminApp.controller("CollectingController", [
    '$scope', 'NgTableParams', '$http', '$filter', function ($scope, NgTableParams, $http, $filter) {

        $scope.collectSearchCriteria =
            {
                T9Text: '',
                CollectStatu: 1,
                StartDate: '',
                EndDate: ''
            };

        $scope.CollectSearch = function (collectSearchCriteria) {
            collectSearchCriteria.StartDate = $('#iCollectStartDate').val();
            collectSearchCriteria.EndDate = $('#iCollectEndDate').val();
            console.log(collectSearchCriteria);
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Collecting/GetListCollectHeader",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { collectSearchCriteria: collectSearchCriteria }
            }).then(function (response) {

                $scope.collectList = response.data;
                $scope.tableCollectsParams = new NgTableParams({
                    count: $scope.collectList.length === 0 ? 1 :  $scope.collectList .length // hides pager
                },
                    {
                        filterDelay: 0,
                        counts: [],
                        dataset: angular.copy( $scope.collectList )
                    });

                $scope.tableCollectsOrjinalData = angular.copy($scope.collectList);
                $scope.tableCollectdelete = $scope.tableKitdelete;
                fireCustomLoading(false);

            });
        };



        $scope.updateCollectStatus = function (cId) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Collecting/UpdateCollectStatus",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { cId: cId }

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


        $scope.deleteCollectStatus = function (cId) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Collecting/DeleteCollectStatus",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { cId: cId }

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

        $scope.tableCollectDetail = function (row) {
            window.open("/Admin/Collectings/CollectDetail/" + row.Id, '_blank');
        };

        $scope.openDetail = function (id) {
            $scope.detailList = {};
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Collecting/GetCollectingDetail",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: id }

            }).then(function (response) {
                fireCustomLoading(false);
                $scope.detailList = response.data;
                $('#modal-text').appendTo("body").modal('show');
            });

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
            }).val(dStart);

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
                        e.CreateDate = moment(e.CreateDate).format('MM/DD/YYYY');
                    };



                    newData.push({
                        "Durum": e.Status == 0 ? 'Beklemede' : 'Onaylandı',
                        "No": e.Id,
                        "Tarih": e.CreateDate,
                        "Cari İsim": e.CustomerName,
                        "Makbuz No": e.DocumentNo,
                        "Tutar": e.PriceTotal.Value.toFixed(2) + ' TL'

                    });
                });

                var ws = XLSX.utils.json_to_sheet(newData, {
                    header: [
                             "Durum",
                             "No",
                             "Tarih",
                             "Cari İsim",
                             "Makbuz No",
                             "Tutar"
                    ]
                });

                //Set Columns Size !
                var wscols = [{ wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 15 }];
                ws['!cols'] = wscols;

                var wb = XLSX.utils.book_new();
                XLSX.utils.book_append_sheet(wb, ws, "Tahsilat_Listesi");


                XLSX.utils.shee;
                var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
                saveAs(new Blob([fireGenerateBlobData(wbout)], { type: "application/octet-stream" }), "Tahsilat_Listesi.xlsx");
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