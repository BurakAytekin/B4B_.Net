adminApp.controller('CustomerSelectController', function ($scope, $http, $parse) {
    $scope.searchCriteria = {};
    $scope.generalSearchSalesmanOfCustomer = function (searchCriteria) //
    {
        var code = searchCriteria.Code;
        var name = searchCriteria.Name;
        var ruleCode = searchCriteria.RuleCode;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetCustomerSearch",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { code: code, name: name, ruleCode: ruleCode }
        }).then(function (response) {
            $scope.searchCustomerResultList = response.data;

            if (!$scope.modalIsOpen) {
                $scope.CustomerSelectOpen(searchCriteria, $scope.veriableName);
                setTimeout(function () {
                    $scope.fillDataSalesmanOfCustomer(response.data);
                }, 1000);
            } else {

                $scope.fillDataSalesmanOfCustomer(response.data);
            }

            fireCustomLoading(false);
            $scope.isFillDataSalesmanOfCustomer = false;
        });

    };

    $scope.WillDeleteCustomerIds = [];

    $scope.addOrDeleteCustomerId = function (row, checkBox) {
        if (checkBox.checkIds)
            $scope.WillDeleteCustomerIds.push(row.Id);
        else {
            var indx = $scope.WillDeleteCustomerIds.indexOf(row.Id);
            $scope.WillDeleteCustomerIds.splice(indx, 1);
        }
    };


    $scope.DeleteCustomerOfSalesman = function () {
        if ($scope.WillDeleteCustomerIds.length < 1) return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/ConnectCustomer",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { salesmanId: $scope.selectedSalesman.Id, customerId: $scope.WillDeleteCustomerIds, type: 0 }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
        $scope.WillDeleteCustomerIds = [];
        $scope.getCustomerList();
    }

    $scope.addCustomerOfSalesman = function () {
        if ($scope.WillDeleteCustomerIds.length < 1) return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/ConnectCustomer",
            headers: {
                "Content-Type": "Application/json;charset=utf-8"
            },
            data: {
                salesmanId: $scope.selectedSalesman.Id, customerId: $scope.WillDeleteCustomerIds
            }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.getCustomerList();
        });
        $scope.WillDeleteCustomerIds = [];
        $scope.isFillDataSalesmanOfCustomer = false;
        $('#mCustomerSearchSalesmanOfCustomer').modal('hide');
    }


    $scope.CustomerSelectOpenSalesman = function (searchCriteria, veriableName) {
        if ($.fn.dataTable.isDataTable('#mCustomerSearchSalesmanOfCustomerTable'))
            $('#mCustomerSearchSalesmanOfCustomerTable').dataTable().fnClearTable();

        $scope.modalIsOpen = true;
        $scope.isFillDataSalesmanOfCustomer = true;
        $('#mCustomerSearchSalesmanOfCustomer').on('hidden.bs.modal', function () {
            $scope.isFillDataSalesmanOfCustomer = false;
            $('#mCustomerSearchSalesmanOfCustomerTable' + ' tbody').unbind("click");
        });
        $scope.veriableName = veriableName;
        $('#mCustomerSearchSalesmanOfCustomer').appendTo("body").modal('show');
        $('#mCustomerSearchSalesmanOfCustomer').on('shown.bs.modal', function () {
            $("#iCustomerName").focus();
        });

        $('#mCustomerSearchSalesmanOfCustomer').on('hidden.bs.modal', function () {
            $scope.modalIsOpen = false;
        });

    };

    $scope.fillDataSalesmanOfCustomer = function (searchData) {
        var mCustomerSearchTableName = '#mCustomerSearchSalesmanOfCustomerTable'; //Id Or Class
        mCustomerSearchTable = $(mCustomerSearchTableName).DataTable({
            data: searchData,
            columns: [
                { defaultContent: "<label class='checkbox checkbox-custom-alt'><input type='checkbox'/><i></i></label>" },
                    //<type class="btn btn-xs btn-rounded btn-primary">Seçiniz</button>', className: "text-right ws-nowrap" },
                { title: "Kodu", data: "Code", className: "scroll-on-hover" },
                { title: "Ünvan", data: "Name", className: "ws-nowrap overflow-hidden scroll-on-hover ellipsis text-left" },

                { title: "Koşul", data: "RuleCode", className: "text-center ws-nowrap" },
                { title: "Durum", data: "Status", className: "text-center ws-nowrap" },
            ],
            language: {
                url: "/Scripts/Admin/vendor/datatables/js/Turkish.json"
            },

            searching: false,

            scrollY: ((searchData.length > 5) ? 160 : false),
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            destroy: true,
            initComplete: function () {
                $scope.fireScrollOnHover();
            }
        });

        $('#mCustomerSearchSalesmanOfCustomerTable' + ' tbody').on('click', 'input[type="checkbox"]', function () {

            var data = mCustomerSearchTable.row($(this).parents('tr')).data();
            if ($(this)[0].checked)
                $scope.WillDeleteCustomerIds.push(data.Id);
            else {
                var indx = $scope.WillDeleteCustomerIds.indexOf(data.Id);
                $scope.WillDeleteCustomerIds.splice(indx, 1);
            }
            //$scope.addCustomerOfSalesman();
            //$(mCustomerSearch).modal('hide');
        });
    };

    $scope.keypressEventSearch = function (e, searchCriteria, veriableName) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            $scope.generalSearchSalesmanOfCustomer(searchCriteria);
            $scope.veriableName = veriableName;
            e.preventDefault();
        }
    };

});