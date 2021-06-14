adminApp.controller('CustomerSelectController', function ($scope, $http, $parse) {

    // #region Veriables
    var mCustomerSearchTable;
    $scope.modalIsOpen = false;
    $scope.selectedCustomer = {};
    $scope.searchCriteria = {};
    $scope.veriableName;
    $scope.searchCustomerResultList = {};
    $scope.IsModalOpendPassed = false;
    var mCustomerSearch = "#mCustomerSearch";

    // #endregion

    $scope.generalSearch = function (searchCriteria) //
    {
        $scope.selectedCustomer = {};
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
            if ($scope.searchCustomerResultList.length === 1 && (!$scope.modalIsOpen || $scope.IsModalOpendPassed)) {
                $scope.selectCustomer($scope.searchCustomerResultList[0]);


            } else {
                if (!$scope.modalIsOpen) {
                    $scope.CustomerSelectOpen(searchCriteria, $scope.veriableName);
                    setTimeout(function () {
                        $scope.fillData(response.data);
                    }, 1000);
                } else {
                    $scope.fillData(response.data);
                }
            }
            fireCustomLoading(false);
        });

    };


    $scope.selectCustomer = function (customer) {
        //$scope.selectedCustomer = customer;

        var model = $parse($scope.veriableName);
        model.assign($scope, customer);

        var model_orjinal = $parse($scope.veriableName + "_Orjinal");
        model_orjinal.assign($scope, customer);

        $scope.searchCriteria.Name = customer.Name;
        $scope.searchCriteria.Code = customer.Code;
        $scope.getWarehouseList();
        $(mCustomerSearch).modal('hide');
    }

    $scope.getWarehouseList = function () {

        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/GetWarehouseList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { customerId: $scope.selectedCustomer.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.warehouseList = response.data;

        });
    };

    $scope.checkWarehouseUpdate = function (warehouseItem) {

        if ($scope.selectedCustomer === {} || $scope.selectedCustomer.Id === undefined)
            return;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Customers/InsertCustomerWarehouse",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { warehouseItem: warehouseItem, customerId: $scope.selectedCustomer.Id }
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

    $scope.fillData = function (searchData) {
        var mCustomerSearchTableName = '#mCustomerSearchTable'; //Id Or Class
        mCustomerSearchTable = $(mCustomerSearchTableName).DataTable({
            data: searchData,
            columns: [
                { defaultContent: '<button class="btn btn-xs btn-rounded btn-primary">Seçiniz</button>', className: "text-right ws-nowrap" },
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

        $(mCustomerSearchTableName + ' tbody').on('click', 'button', function () {
            var data = mCustomerSearchTable.row($(this).parents('tr')).data();
            $scope.selectCustomer(data);

            $(mCustomerSearch).modal('hide');
        });


    };

    $scope.fireScrollOnHover = function () {
        $(".scroll-on-hover").mouseover(function () {
            $(this).removeClass("ellipsis");
            var maxscroll = $(this).width();
            var speed = maxscroll * 10;
            $(this).animate({
                scrollLeft: maxscroll
            }, speed, "linear");
        });

        $(".scroll-on-hover").mouseout(function () {
            $(this).stop();
            $(this).addClass("ellipsis");
            $(this).animate({
                scrollLeft: 0
            }, 'slow');
        });
    }

    $scope.keypressEventSearch = function (e, searchCriteria, veriableName,pressedKey) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            if (pressedKey == 1) {

                $scope.searchCriteria.Name = "";
            }
            if (pressedKey == 2)
                $scope.searchCriteria.Code = "";
            $scope.generalSearch($scope.searchCriteria);
            $scope.veriableName = veriableName;
            e.preventDefault();
        }
    };


    $scope.CustomerSelectOpen = function (searchCriteria, veriableName) {
        $scope.modalIsOpen = true;
        //if (!$('#mProductSearchTable').hasClass('hide'))
        //    $('#mProductSearchTable').addClass('hide');
        $scope.veriableName = veriableName;
        $(mCustomerSearch).appendTo("body").modal('show');
        $(mCustomerSearch).on('shown.bs.modal', function () {
            $("#iCustomerName").focus();

            //if (($scope.searchCriteria.Code != undefined && $scope.searchCriteria.Code != "")) {
            //    $scope.generalSearch($scope.searchCriteria);
            //    $scope.veriableName = veriableName;
            //    $scope.IsModalOpendPassed = true;
            //}
        });

        $(mCustomerSearch).on('hidden.bs.modal', function () {
            $scope.modalIsOpen = false;
        });
    };



    $scope.clear = function () {
        $scope.searchCriteria = {};
        $scope.searchCustomerResultList = {};
        $scope.selectedCustomer = {};
        $scope.warehouseList = {};

    };

    $(document).ready(function () {

    });

});
