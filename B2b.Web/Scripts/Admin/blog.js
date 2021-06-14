adminApp.controller("blogController", ['$scope', 'NgTableParams', '$http', '$element', function ($scope, NgTableParams, $http, $element) {
    $scope.videoConnectChange = function (value) {
        $('#divPreview').html(value);
    };

    $scope.selectedBlogId = 0;

    $scope.loadData = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/GetBlogList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.blogList = response.data;

            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy($scope.blogList)
            });

            $scope.originalData = angular.copy($scope.blogList);
        });
    };

    $scope.clearValues = function () {
        $scope.isEditing = false;
        $scope.category = '';
        $scope.header = '';
        $scope.shortContent = '';
        $('#txtVideo').val('');
        $("[name='customRadio']:checked").val("0");
        $('#isShowCommentUser').prop("checked", false);
        $('#approvalComment').prop("checked", false);
        $('#isLockComment').prop("checked", false);
        $('.note-editable').html('');
        $('#divPreview').html('');
        $('#imgPreview').attr('src', '');
        $('#txtVideo').val('');
        $scope.videoValue = '';
        $scope.isShow = false;
        $("[name='customRadio']")[0].checked = true;
        $("[name='customRadio']:checked").val("0");
        $('html, body').animate({ scrollTop: $('#divTable').offset().top - 20 }, 'slow');
        $scope.selectedBlogId = 0;
        $('.controls a:first').tab('show');
    };

    $scope.loadDataComment = function (id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/GetBlogCommentList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.blogComment = response.data;
        });
    }

    $scope.askForDelete = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () { $scope.delete(row); }
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

    $scope.delete = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/DeleteBlog",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });

            var index = $scope.tableParams.data.indexOf(row);
            $scope.deleteCount++;
            $scope.tableParams.data.splice(index, 1);
            $scope.tableParams = new NgTableParams({}, {
                filterDelay: 0,
                dataset: angular.copy(this.tableParams.data)
            });

            $scope.clearValues();
        });
    };

    $scope.askForDeleteComment = function (row) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                Ok: {
                    text: "Evet",
                    btnClass: 'btn-blue',
                    action: function () { $scope.deleteComment(row); }
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

    $scope.deleteComment = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/DeleteBlogComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            var index = $scope.blogComment.indexOf(row);
            $scope.blogComment.splice(index, 1);

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.approveComment = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/ApproveBlogComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }
        }).then(function (response) {
            fireCustomLoading(false);
            var index = $scope.blogComment.indexOf(row);
            row.IsApproval = true;
            $scope.blogComment[index] = row;

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.keypressEventAddBlogComment = function (e, selectedBlogId) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 13) {
            $scope.addBlogComment(selectedBlogId);
        }
    };

    $scope.addBlogComment = function (selectedBlogId) {
        var content = $scope.commentText; //$('#txtCommentContent').val();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/Blog/AddBlogComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { blogId: selectedBlogId, content: content }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.loadDataComment(selectedBlogId);
            $('#txtCommentContent').val('');

            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $scope.openModal = function (id) {
        $scope.selectedBlogId = id;
        $scope.loadDataComment(id);
        $('#mBlogComment').appendTo("body").modal('show');
    };

    $scope.setFile = function (element) {
        $scope.currentFile = element.files[0];
        var reader = new FileReader();

        reader.onload = function (event) {
            $scope.image_source = event.target.result
            $scope.$apply()

        }
        // when the file is read it triggers the onload event above.
        reader.readAsDataURL(element.files[0]);
    };

    $scope.editBlogItem = function (row) {
        $scope.clearValues();
        $('.controls a:last').tab('show');
        $('html, body').animate({ scrollTop: $('#divCategory').offset().top - 20 }, 'slow');
        $scope.selectedBlogId = row.Id;
        $scope.isEditing = true;
        $scope.category = row.Category;
        $scope.header = row.Header;
        $scope.shortContent = row.ShortContent;
        $('#isShowCommentUser').prop("checked", row.IsShowCommentUser);
        $('#approvalComment').prop("checked", row.ApprovalComment);
        $('#isLockComment').prop("checked", row.IsLockComment);
        $('.note-editable').html(row.Content);
        if (row.Type === 0) {
            $('#imgPreview').attr('src', row.Path);
            $("[name='customRadio']")[0].checked = true;
            $("[name='customRadio']:checked").val("0");
        }
        else {
            $scope.isShow = true;
            $("[name='customRadio']")[1].checked = true;
            $('#divPreview').html(row.Path);
            $('#txtVideo').val(row.Path);
            $scope.videoValue = row.Path;
            $("[name='customRadio']:checked").val("1");
        }
    };

    $scope.saveBlog = function (edit) {
        var category = $scope.category;// $('#txtCategory').val();
        var header = $scope.header;// $('#txtHeader').val();
        var type = $("[name='customRadio']:checked").val();
        var videoUrl = type == "1" ? $('#txtVideo').val() : "";
        var isShowCommentUser = $('#isShowCommentUser').prop('checked');
        var approvalComment = $('#approvalComment').prop('checked');
        var shortContent = $scope.shortContent;
        var isLockComment = $('#isLockComment').prop('checked');
        var content = $('.note-editable').html();

        $http({
            method: "POST",
            url: "/Admin/Blog/SaveBlog",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { edit: edit, id: $scope.selectedBlogId, category: category, header: header, videoUrl: videoUrl, isShowCommentUser: isShowCommentUser, approvalComment: approvalComment, type: type, content: content, imageBase: $scope.image_source, shortContent: shortContent, isLockComment: isLockComment }//, 
        }).then(function (response) {
            $scope.clearValues();
            $scope.loadData();
            iziToast.show({
                message: response.data.Message,
                position: 'topCenter',
                color: response.data.Color,
                icon: response.data.Icon
            });
        });
    };

    $(window).load(function () {
        $("[name='customRadio']")[0].checked = true;
        $('#txtConntent').summernote({
            height: 200   //set editable area's height
        });
    });

    $(document).ready(function () {
        $scope.loadData();
    });
}]).config(['$qProvider', function ($qProvider) {
    $qProvider.errorOnUnhandledRejections(false);
}]).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});