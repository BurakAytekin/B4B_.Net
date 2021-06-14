adminApp.controller('announcementsController', ['$scope', '$http', 'NgTableParams', '$filter', function ($scope, $http, NgTableParams, $filter) {
    $scope.announcementType = { id: -1 };
    $scope.announcementListingType = { id: 0 };
    $scope.selectedAnnouncement = { Id: 0 };
    $scope.priorityList = { id: -1 };

    $scope.deleteIds = '';//örnek format 1,2,3...
    $scope.deleteItem = {};


    $scope.getAnnouncementList = function () {
        fireCustomLoading(true);
        $scope.clearValues();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Announcements/GetAnnouncementsHeader",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { announcementsType: $scope.announcementType.id, listType: $scope.announcementListingType.id }

        }).then(function (response) {
            fireCustomLoading(false);
            $scope.announcementsHeaderList = response.data;
            fireCustomLoading(false);
            //$scope.announcementsHeaderList = $sce.trustAsHtml(response.data);
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

    $scope.setPriority = function (id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Announcements/SetPriority",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }
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

    $scope.saveAnnouncement = function (announcement) {
        announcement.AnnouncementsType = $scope.announcementType.id;
        announcement.StartDate = $('#iAnnouncementStartDate').val();
        announcement.EndDate = $('#iAnnouncementEndDate').val();
        announcement.Content = $('.note-editable').html();
        announcement.ImageBase = $scope.image_sourceIcon;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Announcements/SaveAnnouncements",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { announcement: announcement }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
            $scope.getAnnouncementList();
            $scope.clearValues();
        });
    };

    $scope.clearValues = function () {
        $scope.announcementsHeaderList = {};
        $scope.image_sourceIcon = null;
        $scope.selectedAnnouncement = { Id: 0 };;
        $('#iAnnouncementStartDate').val('');
        $('#iAnnouncementEndDate').val('');
        $('#iAnnouncementTitle').val('');
        $('.note-editable').html('');
        $('#iAnnouncementPicturePath').val();
        $('.addImage').attr('src', '');
        $scope.deleteIds = '';
       
    };

    $scope.SaveQuery = function (setQuery) {
        var queryText = '';
        if (setQuery.Manufacturer != '' && setQuery.Manufacturer!=undefined)
            queryText += 'manu=' + setQuery.Manufacturer + "&";
        if (setQuery.Campaign)
            queryText += 'cmp=1&';
        if (setQuery.NewProduct)
            queryText += 'new=1&';
        if (setQuery.OnQty)
            queryText += 'onhand=1&';
        if (setQuery.HavePicture)
            queryText += 'pic=1&';
        if (setQuery.Text != '')
            queryText += 'text=' + setQuery.Text + "&";

        $scope.selectedAnnouncement.Query = queryText.substring(0, queryText.length - 1);
        $('#mQuerySelect').appendTo("body").modal('hide');
    };

    $scope.announcementQueryOpen = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Products/GetProductsManufacturerList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);

            $scope.productManufacturerList = response.data;

        });
        $('#mQuerySelect').appendTo("body").modal('show');

    };

    $scope.deleteAnnouncements = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Announcements/DeleteAnnouncements",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { DeleteIds: $scope.deleteIds }
        }).then(function (response) {
            fireCustomLoading(false);
            $('#modal-delete').modal('hide');

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            $scope.getAnnouncementList();
            $scope.clearValues();

        });
    };

    $scope.updateAnnouncementsActive = function (announcement) {
        //announcements.StartDate = new Date(parseInt(announcements.StartDate.substr(6)));
        //announcements.EndDate = new Date(parseInt(announcements.EndDate.substr(6)));
        announcement.StartDate = $('#iAnnouncementStartDate').val();
        announcement.EndDate = $('#iAnnouncementEndDate').val();
        announcement.Content = $('.note-editable').html();
        announcement.ImageBase = $scope.image_sourceIcon;
        announcement.AnnouncementsType = $scope.announcementType.id;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Announcements/UpdateAnnouncements",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { announcement: announcement }
        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            $scope.getAnnouncementList();
        });
    };

    $scope.setAnnouncement = function (item) {
        var item2 = angular.copy(item);
        item2.StartDate = $filter('date')(new Date(item.StartDate), 'dd/MM/yyyy');
        item2.EndDate = $filter('date')(new Date(item.EndDate), 'dd/MM/yyyy');
        $('.note-editable').html(item2.Content);
        $scope.selectedAnnouncement = item2;
    };


    $scope.showDeleteModal = function () {
        if ($scope.deleteIds == undefined || $scope.deleteIds == '') {
            iziToast.error({
                message: 'Lütfen Silinecek Duyuru / lar ı Seçiniz.',
                position: 'topCenter'
            });
        } else {
            $scope.deleteItem.Name = 'Duyuru';
            $scope.deleteItem.Description = 'Seçilen Duyuru/ lar ı ';
            $scope.deleteItem.FunctionName = $scope.deleteAnnouncements;

            $('#modal-delete').modal('show');
        };
    };


    $scope.setDeleteIds = function (Id) {

        //Her gelen Id yi silinecek Id ler listesine ekledim, zaten varsa bile ! ( sorun olmayacaktır )
        $scope.deleteIds += Id + ',';
    };



    $(document).ready(function () { });

    var pnlAnnouncementElements = {
        pnlAnnouncementTitle: $('#pnlAnnouncementTitle'),
        pnlAnnouncementContent: $('#pnlAnnouncementContent'),
        pnlAnnouncementPicture: $('#pnlAnnouncementPicture'),
        pnlAnnouncementPicturePath: $('#pnlAnnouncementPicturePath'),
        pnlAnnouncementStartDate: $('#pnlAnnouncementStartDate'),
        pnlAnnouncementEndDate: $('#pnlAnnouncementEndDate'),
        pnlAnnouncementStatu: $('#pnlAnnouncementStatu'),
        pnlAnnouncementQuery: $('#pnlAnnouncementQuery'),
        pnlAnnouncementPrice: $('#pnlAnnouncementPrice'),
    };

    function fireDisabledAnnouncementElements() {
        //$.each(pnlAnnouncementElements, function (eName, eValue) {
        //    eValue.find("input").attr("disabled", "disabled");
        //    eValue.find("textarea").attr("disabled", "disabled");
        //});

        //pnlAnnouncementElements.pnlAnnouncementContent.summernote('disable');
        //$('#iAnnouncementContent').summernote('disable');
    };

    function fireHideAnnouncementElements() {
        $.each(pnlAnnouncementElements, function (eName, eValue) {
            if (!eValue.hasClass("hide")) eValue.addClass("hide");

            fireDisabledAnnouncementElements();
        });
    }

    var updateOutput = function (e) {
        var list = e.length ? e : $(e.target),
            output = list.data('output');
        if (window.JSON && output !== undefined) {
            var json = list.nestable('serialize');
            //output.val(json); //, null, 2));
            if (json.length > 0) {
                $scope.priorityList = json;
                $scope.setPriority($scope.priorityList);
            }
        }
    };

    $(window).load(function () {
        fireHideAnnouncementElements();

        $('#iAnnouncementContent').summernote({
            height: 200, //Set Editable Area's Height
            focus: false
        });

        //Initialize Mini Calendar Datepicker
        $('#iAnnouncementStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            //defaultDate: "01/01/2017",
            locale: "tr"
        });

        $('#iAnnouncementEndDate').datetimepicker({
            format: 'DD/MM/YYYY',
            defaultDate: new Date(),
            locale: 'tr'
        });
        //*Initialize Mini Calendar Datepicker

        $('#ulAnnouncementsSearchResult').nestable({
            listNodeName: 'ul', //'ol',
            itemNodeName: 'li', //'li'
            rootClass: 'list-group-wrapper', //'dd',
            listClass: 'list-group', //'dd-list',
            itemClass: 'list-group-item', //'dd-item',
            dragClass: 'dd-dragel', //'dd-dragel'
            handleClass: 'item-handle', //'dd-handle',
            collapsedClass: 'dd-collapsed', //'dd-collapsed'
            placeClass: 'dd-placeholder', //'dd-placeholder'
            noDragClass: 'dd-nodrag', //'dd-nodrag'
            emptyClass: 'dd-empty', //'dd-empty'
            expandBtnHTML: '', //'<button data-action="expand" type="button">Expand</button>',
            collapseBtnHTML: '', //'<button data-action="collapse" type="button">Collapse</button>',
            group: 0, //0
            maxDepth: 1, //5
            threshold: 20  //20
        }).on('change', updateOutput);

        // output initial serialised data
        updateOutput($('#ulAnnouncementsSearchResult').data('output', $('#nestable-output')));

        $("#iProductAnnouncementType").change(function () {
            $scope.clearValues();
            var elm = $(this),
                eText = elm.find("option:selected").text(),
                eValue = parseInt(elm.val());
            //alert("Element: " + eText + " - Değeri: " + eValue);

            fireHideAnnouncementElements();

            pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
            pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
            pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
            pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");

            switch (eValue) {
                case 0:
                case 4:
                case 5:
                case 7:
                    fireHideAnnouncementElements();
                    pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementContent.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");
                    break;

                case 6:
                    fireHideAnnouncementElements();
                    pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementContent.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicture.addClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicturePath.addClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementQuery.addClass("hide");
                    break;

                case 9:
                case 3:
                case 11:
                    fireHideAnnouncementElements();
                    pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicture.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicturePath.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
                    break;
                case 1:
                    
                    fireHideAnnouncementElements();
                    pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicture.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicturePath.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementQuery.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPrice.removeClass("hide");
                    break;
                case 12:
                    fireHideAnnouncementElements();
                    pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicture.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicturePath.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
                    break;

                case 10:
                    fireHideAnnouncementElements();
                    pnlAnnouncementElements.pnlAnnouncementTitle.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicture.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementPicturePath.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStatu.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementStartDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementEndDate.removeClass("hide");
                    pnlAnnouncementElements.pnlAnnouncementQuery.removeClass("hide");
                    break;

                default:
                    fireHideAnnouncementElements();
                    break;
            }
        });

        $("#iProductAnnouncementType").change();
    });
}]).directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return parseInt(val, 10);
            });

            ngModel.$formatters.push(function (val) {
                return '' + val;
            });
        }
    };
});