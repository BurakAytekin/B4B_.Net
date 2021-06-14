adminApp.controller('SalesmanSelectController', function ($scope, $http, NgTableParams, $parse) {

    // #region Veriables
    var mSalesmanSearchTable;
    $scope.modalIsOpen = false;
    $scope.selectedSalesman = {};
    $scope.searchCriteriaSalesman = {};

    $scope.searchSalesmanResultList = {};
    $scope.IsModalOpendPassed = false;

    var mSalesmanSearch = "#mSalesmanSearch";

    // #endregion
    $scope.generalSearchSalesman = function (searchCriteriaSalesman) //
    {
        fireCustomLoading(true);
        $scope.selectedSalesman = {};

        var name = searchCriteriaSalesman.Name;

        //Ad değilde kod ile arama yapılıyorsa...
        if (searchCriteriaSalesman.Name === undefined || searchCriteriaSalesman.Name === '') {
            name = searchCriteriaSalesman.Code;
        };

        $http({
            method: "POST",
            url: "/Admin/Salesmans/GetSalesmanList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { name: name }
        }).then(function (response) {
            $scope.searchSalesmanResultList = response.data;

            if ($scope.searchSalesmanResultList.length === 1 && (!$scope.modalIsOpen || $scope.IsModalOpendPassed)){
                $scope.selectSalesman($scope.searchSalesmanResultList[0]);

            } else {
                if (!$scope.modalIsOpen) {
                    $scope.salesmanSelectOpen(searchCriteriaSalesman, $scope.veriableName);
                    setTimeout(function () {
                        $scope.fillDataSalesman(response.data);
                    }, 1000);
                } else {
                    $scope.fillDataSalesman(response.data);
                }
            }
            fireCustomLoading(false);
        });

    };
    $scope.getCountFillData = function () {
        $scope.userCountList = [
       { Count: 0 }, { Count: 1 }, { Count: 2 }, { Count: 3 }, { Count: 4 }, { Count: 5 }, { Count: 6 }, { Count: 7 }, { Count: 8 }, { Count: 9 }, { Count: 10 }, { Count: 11 }, { Count: 12 }, { Count: 13 }, { Count: 14 }, { Count: 15 }, { Count: 16 }, { Count: 17 }, { Count: 18 }, { Count: 19 }, { Count: 20 }];

    }
    $scope.selectSalesman = function (salesman) {
        var model = $parse($scope.veriableName);
        model.assign($scope, salesman);

        $scope.searchCriteriaSalesman.Name = $scope.selectedSalesman.Name;
        $scope.searchCriteriaSalesman.Code = $scope.selectedSalesman.Code;
        $scope.licenceList(salesman.Id);
        $scope.getCountFillData();
        $('#aSalesmanGeneralInformation').click();
        $(mSalesmanSearch).modal('hide');


    }

    $scope.licenceList = function (salesmanId) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/GetLicenceList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { salesmanId: salesmanId }
        }).then(function (response) {

            $scope.tableLicenceB2BParams = new NgTableParams({
                count: response.data.LicenceB2B.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data.LicenceB2B)
            });

            $scope.tableB2BOrjinalData = angular.copy(response.data.LicenceB2B);
            $scope.tableLicenceForDelete = $scope.tableLicenceForDelete;

            $scope.tableLicenceAdminParams = new NgTableParams({
                count: response.data.LicenceAdmin.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data.LicenceAdmin)
            });

            $scope.tableAdminOrjinalData = angular.copy(response.data.LicenceAdmin);
            $scope.tableLicenceForDelete = $scope.tableLicenceForDelete;

            $scope.tableLicenceMobileParams = new NgTableParams({
                count: response.data.LicenceMobile.length // hides pager
            }, {
                filterDelay: 0,
                counts: [],
                dataset: angular.copy(response.data.LicenceMobile)
            });

            $scope.tableMobileOrjinalData = angular.copy(response.data.LicenceMobile);
            $scope.tableLicenceMobileForDelete = $scope.tableLicenceMobileForDelete;

            fireCustomLoading(false);

        });

    };
    $scope.tableLicenceForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.deleteLicence(row); }

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
    };
    $scope.deleteLicence = function (licenceItem) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/DeleteLicence",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { licenceItem: licenceItem }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.licenceList($scope.selectedSalesman.Id);
        });
    };
    $scope.tableLicenceMobileForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
                {
                    Ok: {
                        text: "Evet",
                        btnClass: 'btn-blue',
                        action: function () { $scope.deleteLicenceMobile(row); }

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
    };

    $scope.deleteLicenceMobile = function (licenceItem) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Salesmans/DeleteLicenceMobile",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { licenceItem: licenceItem }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.licenceList($scope.selectedSalesman.Id);
        });
    }


    $scope.fillDataSalesman = function (searchData) {
        var mSalesmanSearchTableName = '#mSalesmanSearchTable'; //Id Or Class

        mSalesmanSearchTable = $(mSalesmanSearchTableName).DataTable({
            data: searchData,
            columns: [
                { defaultContent: '<button class="btn btn-xs btn-rounded btn-primary">Seçiniz</button>', className: "text-right ws-nowrap" },
                { title: "Kodu", data: "Code" },
                { title: "Ünvan", data: "Name", className: "text-left ws-nowrap scroll-on-hover" },



            ],
            language: {
                url: "/Scripts/Admin/vendor/datatables/js/Turkish.json"
            },

            searching: true,

            scrollY: ((searchData.length > 5) ? 160 : false),
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            destroy: true,
            initComplete: function () {
                $scope.fireScrollOnHover();
            }
        });

        $(mSalesmanSearchTableName + ' tbody').on('click', 'button', function () {
            var data = mSalesmanSearchTable.row($(this).parents('tr')).data();
            $scope.selectSalesman(data);

            $(mSalesmanSearch).modal('hide');
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
    $scope.keypressEventSearchSalesman = function (e, searchCriteriaSalesman, veriableName) {
        var key;

        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox

        if (key === 13) {
            $scope.veriableName = veriableName;
            $scope.generalSearchSalesman(searchCriteriaSalesman);
            e.preventDefault();
        }
    };
    $scope.salesmanSelectOpen = function (searchCriteriaSalesman, veriableName) {


        $scope.modalIsOpen = true;
        $scope.veriableName = veriableName;
        $(mSalesmanSearch).appendTo("body").modal('show');
        $(mSalesmanSearch).on('shown.bs.modal', function () {
            $("#iSalesmanName").focus();
         //   $scope.generalSearchSalesman(searchCriteriaSalesman);
            $scope.IsModalOpendPassed = true;

        });


        $(mSalesmanSearch).on('hidden.bs.modal', function () {
            $scope.modalIsOpen = false;
        });



    };
    $scope.clearSalesman = function () {
        $scope.searchCriteriaSalesman = {};
        $scope.selectedSalesman = {};
        $scope.tableLicenceB2BParams = {};
        $scope.tableLicenceAdminParams = {};
        $scope.tableLicenceMobileParams = {};
        $scope.lockUpdate = false;
        $scope.getCountFillData();
        $scope.SalesmanOfCustomerList = {};
        $scope.tableCustomerListParams.settings().dataset = [];
        $scope.tableCustomerListParams.reload();
        $('#aSalesmanGeneralInformation').click();
    };
    $(document).ready(function () {

    });
});
