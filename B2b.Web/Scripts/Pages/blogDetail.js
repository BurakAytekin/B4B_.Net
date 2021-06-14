b2bApp.controller('BlogDetailController', ['$scope', '$http', function ($scope, $http) {
    // #region Veriables
    $scope.blogCommentList;
    // #endregion
    $scope.getBlogCommentList = function () {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Blog/GetBlogCommentList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.blogCommentList = response.data;
            fireCustomLoading(false);
        });
    };
    $scope.addComment = function () {
        var comment = $('#comment-message').val();
        var header = $('#comment-subject').val();
        if (header === '' || comment === '') {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen ilgili alanları doldurunuz',
                position: 'topCenter'
            });
        }
        else {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Blog/AddComment",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { header: header, comment: comment }

            }).then(function (response) {
                var retVal = jQuery.parseJSON(response.data);
                if (retVal.statu === "success") {
                    $('#comment-message').val('');
                    $('#comment-subject').val('');
                    $scope.getBlogCommentList();
                    iziToast.success({
                        message: retVal.message,
                        position: 'topCenter'
                    });
                }
                else {
                    iziToast.error({
                        //title: 'Hata',
                        message: retVal.message,
                        position: 'topCenter'
                    });
                };
                fireCustomLoading(false);
            });
        }
    };
    $scope.deleteBlogComment = function (row) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Blog/DeleteBlogComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: row.Id }

        }).then(function (response) {
            var retVal = jQuery.parseJSON(response.data);
            if (retVal.statu === "success") {

                var index = $scope.blogCommentList.indexOf(row);
                $scope.blogCommentList.splice(index, 1);
                iziToast.success({
                    message: retVal.message,
                    position: 'topCenter'
                });
            }
            else {
                iziToast.error({
                    //title: 'Hata',
                    message: retVal.message,
                    position: 'topCenter'
                });
            };
            fireCustomLoading(false);
        });
    };
    $(document).ready(function () {
        $scope.getBlogCommentList();
    });
}]).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});