reportApp.controller('ViewReportController', ['$scope', '$http', function ($scope, $http) {

    $scope.reportParams = [];
    $scope.paramList = [];
    $scope.headerList = [];
    $scope.typeList = [];
    $scope.Customers = [];
    $scope.validateResult = false;
    $scope.validateResultMessage = '';

    $scope.validateGetReport = function () {
        $scope.validateResult = true;

        $.each($scope.reportParams, function (eIndex, eValue) {
            if (eValue === '') {
                $scope.validateResult = false;
                $scope.nullParamList.push(eIndex);
                $scope.validateResultMessage += $scope.headerList[eIndex] + ' - ';
            };
        });

        if ($scope.nullParamList.length > 0) {
            $scope.validateResult = false;
            $scope.validateResultMessage = $scope.validateResultMessage.substr(0, ($scope.validateResultMessage.length - 3)) + ($scope.nullParamList.length > 1 ? ' alanları boş bırakılamaz' : ' alanı boş');
        };
        
    };


    $scope.getReport = function () {
        $scope.nullParamList = [];

        $scope.validateGetReport();

        if (!$scope.validateResult) {

            iziToast.error({
                message: $scope.validateResultMessage,
                position: 'topCenter'
            });

        } else {
        
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Report/Home/GetReport",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { procName: $scope.procName, params: $scope.paramList, types: $scope.typeList, values: $scope.reportParams, exportViewType: $scope.detailsList.FunctionType, set: $scope.reportSettings }
            }).then(function (response) {
                console.log(response.data);
                fireCustomLoading(false);

                if ($scope.reportResult !== undefined && $scope.reportResult.length !== 0) {
                    $('#reportTable').DataTable().destroy();
                }
                $scope.reportResult = response.data;

                if ($scope.reportResult.length > 0)
                    $scope.cols = Object.keys($scope.reportResult[0]);
            });

        };
    };


    $scope.reportDetails = function () {
        if ($scope.ReportId === 0) return false;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Report/Home/ReportDetails",
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

    $(document).ready(function () {
        $scope.reportDetails();
    });

    $scope.$on('ngRepeatLoadResultFinished', function (ngRepeatLoadResultFinished) {
        $.each($scope.detailsList.Parameters, function (eIndex, eValue) {
            $('#hide').remove();
            if (eValue.paramName === 'CariKod') {
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


        $('#show').on('change', function () {
            var val = this.value;
            var indx = $scope.paramList.indexOf('CariKod');
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
                            columns: ':visible:not(.not-export-col)'
                        },
                        customize: function (xlsx) {
                            var sheet = xlsx.xl.worksheets['sheet1.xml'];

                            // jQuery selector to add a border
                            //$('row c[r*="10"]', sheet).attr('s', '25');
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
//}).directive('largeSelect', [function () {
//    return {
//        restrict: 'E',
//        require: 'ngModel',
//        link: function (scope, elm, attrs, ctrl) {
//            var select = document.createElement("SELECT");
//            elm[0].appendChild(select);
//            var indx = scope.paramList.indexOf('CariKod');
//            function updateOptions(data) {
//                var fragment = document.createDocumentFragment();
//                select.innerHTML = "";
//                if (angular.isArray(data)) {
//                    for (var i = 0; i < data.length; i++) {

//                        if (i === 0) {
//                            var option = document.createElement("OPTION");
//                            option.value = option.innerText = 'SEÇİNİZ';
//                            option.text = option.innerText = 'SEÇİNİZ';
//                            fragment.appendChild(option);
//                        }
//                        var option = document.createElement("OPTION");
//                        option.value = option.innerText = data[i].Code;
//                        option.text = option.innerText = data[i].Name;
//                        fragment.appendChild(option);
//                    }
//                    select.appendChild(fragment);
//                }
//            }

//            scope.$watch(attrs.data, function (newVal, oldVal) {
//                updateOptions(newVal);
//            });

//            //scope.$watch(attrs.ngModel, function (newVal, oldVal) {
//            //    var option = select.querySelector("option[value=\"" + newVal + "\"]");
//            //    if (option) {
//            //        option.selected = true;
//            //        option.setAttribute('selected', 'selected');
//            //    }
//            //});

//            select.addEventListener("change", function (event) {
//                var index = select.selectedIndex;
//                var val = select.options[index];
//                scope.reportParams[indx] = val.value;
//            });
//        }
//    };
//}]);