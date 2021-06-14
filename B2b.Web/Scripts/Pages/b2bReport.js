b2bApp.controller('b2bReportController', ['$scope', '$http', function ($scope, $http) {
    $scope.loadMenu = function () {
        $http({
            method: "POST",
            url: "/B2bReport/GetMenuList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { reportIsActive: 1 }
        }).then(function (response) {
            $scope.menuList = response.data;
        });
    };

    $(document).ready(function () {
        $scope.loadMenu();
        $scope.reportDetails();
    });



    $scope.reportParams = [];
    $scope.paramList = [];
    $scope.headerList = [];
    $scope.typeList = [];
    $scope.Customers = [];


    $scope.getReport = function () {
        fireCustomLoading(true);
        $scope.nullParamList = [];
        var errorMessage = '';
        $.each($scope.reportParams, function (eIndex, eValue) {
            if (eValue === '') {
                $scope.nullParamList.push(eIndex);
                errorMessage += $scope.headerList[eIndex] + ' - ';
            }
        });

        if ($scope.nullParamList.length > 0) {
            iziToast.show({
                message: errorMessage.substr(0, (errorMessage.length - 3)) + ($scope.nullParamList.length > 1 ? ' alanları boş bırakılamaz' : ' alanı boş bırakılamaz'),
                position: 'topCenter',
                color: 'red',
                icon: 'fa fa-ban'
            });
            fireCustomLoading(false);
            return false;
        }
        $http({
            method: "POST",
            url: "/B2bReport/GetReport",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { procName: $scope.procName, params: $scope.paramList, types: $scope.typeList, values: $scope.reportParams, exportViewType: $scope.detailsList.FunctionType, set: $scope.reportSettings }
        }).then(function (response) {
            if ($scope.reportResult !== undefined && $scope.reportResult.length !== 0) {
                $('#reportTable').DataTable().destroy();
            }
            $scope.reportResult = response.data;

            if ($scope.reportResult.length > 0)
                $scope.cols = Object.keys($scope.reportResult[0]);


        });
    };


    $scope.reportDetails = function () {
        if ($scope.ReportId === 0) return false;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/B2bReport/ReportDetails",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { reportId: $scope.ReportId }
        }).then(function (response) {
            $scope.detailsList = response.data;
            $scope.Customers = $scope.detailsList.Customers;
            $scope.ReportName = $scope.detailsList.Header;
            $scope.procName = $scope.detailsList.Function;
            //$csope.exportViewType = 
            $scope.reportSettings = $scope.detailsList.CompanySettings;

            if ($scope.detailsList.Parameters !== null)
                $.each($scope.detailsList.Parameters, function (eIndex, eValue) {
                    $scope.paramList.push(eValue.paramName);
                    $scope.typeList.push(eValue.Type);
                    $scope.headerList.push(eValue.Header);
                });
            fireCustomLoading(false);
        });

    };


    $scope.$on('ngRepeatLoadResultFinished', function (ngRepeatLoadResultFinished) {
        $.each($scope.detailsList.Parameters, function (eIndex, eValue) {
            $('#hide').remove();
            $('#hideCustomer').remove();
            $('#hidePlasiyer').remove();
            if (eValue.paramName === 'CariKod') {
                $scope.reportParams[eIndex] = 'SEÇİNİZ';
            }
            else if (eValue.paramName === 'PlasiyerKod') {
                $scope.reportParams[eIndex] = 'SEÇİNİZ';
            }
            if (eValue.Type === 'datetime') {
                $scope.reportParams[eIndex] = (new Date()).toLocaleDateString("tr-TR").replace('.', '/').replace('.', '/');
                $('#inp' + eIndex).datetimepicker({
                    format: "DD/MM/YYYY",
                    defaultDate: new Date(),
                    locale: "tr"
                });
                $('#inp' + eIndex).on("dp.change", function () {
                    $scope.reportParams[eIndex] = $('#inp' + eIndex).val();
                });
            }
            else if (eValue.Type.indexOf('varchar') > -1) {
                $scope.reportParams[eIndex] = '';
            }
            else if (eValue.Type === 'checkbox') {
                $scope.reportParams[eIndex] = false;
            }
            else
                $scope.reportParams[eIndex] = '';
        });


        $('#showCustomer').on('change', function () {
            var val = this.value;
            var indx = $scope.paramList.indexOf('CariKod');
            if (indx > -1)
                $scope.reportParams[indx] = this.value;
        });
        $('#showPlasiyer').on('change', function () {
            var val = this.value;
            var indx = $scope.paramList.indexOf('PlasiyerKod');
            if (indx > -1)
                $scope.reportParams[indx] = this.value;
        });


    });
    var table = null;
    $scope.$on('ngRepeatsearchResultFinished', function (ngRepeatSearchResultFinishedEvent) {
            if ($.fn.dataTable.isDataTable('#reportTable')) {
                table = $('#reportTable').DataTable();
            }
            else {
                table = table = $('#reportTable').dataTable({
                    "language": {
                        "lengthMenu": "Her Sayfada _MENU_ sonuç göster",
                        "info": "_TOTAL_ kayıttan _START_ - _END_ arasındaki kayıtlar gösteriliyor",
                        "infoEmpty": "0 kayıttan 0 - 0 arasındaki kayıtlar gösteriliyor",
                        "infoFiltered": "(_MAX_ kayıt içinde)",
                        "loadingRecords": "Yükleniyor...",
                        "processing": "İşleniyor...",
                        "search": "Arama:",
                        "zeroRecords": "Kayıt Bulunamadı...",
                        "paginate": {
                            "first": "İlk",
                            "last": "Son",
                            "next": "İleri",
                            "previous": "Geri"
                        }
                    },
                    paging: true,
                    dom: 'lBfrtip',
                    buttons: [{ extend: 'colvis', text: 'Kolon Gizle/Göster' },
                        {
                            extend: 'excelHtml5',
                            filename: $scope.ReportName + '_' + (new Date(Date.now()).toLocaleString()).replace(':', '').replace(' ', '_'),
                            exportOptions: {
                                columns: ':visible:not(.not-export-col)',
                                format: {
                                    body: function (data, row, column, node) {
                                        if (typeof data !== 'undefined') {
                                            if (data !== null) {
                                                data += '\u200C';
                                                return data;
                                            }
                                        }
                                        return data;
                                    }
                                }
                            },
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];

                                // jQuery selector to add a border
                                //$('row c[r*="10"]', sheet).attr('s', '25');

                                var col = $('col', sheet);
                                col.each(function () {
                                    $(this).attr('width', 30);
                                });


                            }
                        }, {
                            extend: 'csvHtml5',
                            filename: $scope.ReportName + '_' + (new Date(Date.now()).toLocaleString()).replace(':', '').replace(' ', '_'),
                            exportOptions: {
                                columns: ':visible:not(.not-export-col)'
                            }
                        }, {
                            extend: 'pdfHtml5',
                            filename: $scope.ReportName + '_' + (new Date(Date.now()).toLocaleString()).replace(':', '').replace(' ', '_'),
                            orientation: 'landscape', pageSize: 'A4', customize: function (doc) {
                                doc.defaultStyle.fontSize = 8; //2,3,4,etc 
                                doc.styles.tableHeader.fontSize = 8; //2, 3, 4, etc 
                            },
                            exportOptions: {
                                columns: ':visible:not(.not-export-col)'
                            }
                        }], initComplete: function () {
                            this.api().columns().every(function () {
                                var column = this;
                                $(("<div style='color:red;' id='filterTable'></div>")).appendTo($(column.footer()));
                                $('#column' + column[0][0]).remove();
                                var select = $('<select id="column' + column[0][0] + '" class="form-control" style="max-width:45px;"><option value=""></option></select>').appendTo($(column.footer())).on('change', function () {
                                    var val = $.fn.dataTable.util.escapeRegex(
                                        $(this).val()
                                    );
                                    $(column.footer()).find("#filterTable").empty().append(val).on('click', function () {
                                        column.search('', true, false).draw();
                                        $(column.footer()).find("#filterTable").empty();
                                        $("#column" + column[0][0] + " option[value='']").prop('selected', true);
                                    });

                                    column
                                        .search(val ? '^' + val + '$' : '', true, false)
                                        .draw();
                                });

                                column.data().unique().sort().each(function (d, j) {
                                    select.append('<option value="' + d + '">' + d + '</option>')
                                });
                            });
                        }
                });
                fireCustomLoading(false);
        }
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
});