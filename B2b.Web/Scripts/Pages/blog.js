b2bApp.controller('BlogController', ['$scope', '$http','$sce', function ($scope, $http, $sce) {

    // #region Veriables
    $scope.blogResult = "";
    $scope.dataCount = 0;
    $scope.vDataCount = 10;
    $scope.repeatCount = 0;
    $scope.lock = false;
    $scope.selectedCategory = '';
    // #endregion
    $scope.setCategory = function (category) {
        $scope.selectedCategory = category;
        $scope.getBlogList(0);
    };
    $scope.getBlogList = function (tb) {
        if (tb === 0)
        {
            $scope.dataCount = 0;
            $scope.vDataCount = 10;
            $scope.repeatCount = 0;
            $scope.lock = false;
        }
        if ($scope.lock)
            return;
        $scope.lock = true;
        var dataCount = $scope.dataCount;
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Blog/GetBlogList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { dataCount: dataCount, category: $scope.selectedCategory }

        }).then(function (response) {
            if (dataCount > 0) {
                $scope.blogResult = $scope.blogResult.concat(response.data);
            }
            else
                $scope.blogResult = response.data;
            $scope.dataCount = $scope.dataCount + 10;
            $scope.lock = false;

            if (response.data.length <= 0) {
                $('#tbResult').infiniteScrollHelper('destroy');
                if (dataCount === 0) {
                    iziToast.error({
                        //title: 'Hata',
                        message: 'Aradığınız kriterlerde sonuç bulunamadı',
                        position: 'topCenter'
                    });
                }
            };
            fireCustomLoading(false);
        });
    };
    $scope.trustDangerousSnippet = function (snippet) {
        return $sce.trustAsHtml(snippet);
    };
    $scope.$on('ngRepeatblogResultFinished', function (ngRepeatblogResultFinishedEvent) {
        $('#tbResult').infiniteScrollHelper({
            bottomBuffer: 150,
            triggerInitialLoad: true,
            loadingClass: false,
            loadMore: function (page, done) {
                if ($scope.repeatCount > 0 && $scope.vDataCount === 10 && $scope.lock === false) {
                    $scope.getBlogList(1);
                }
                $scope.repeatCount++;
                done();
            }
        });
    });
    $(document).ready(function () {
        $scope.selectedCategory = '';
        $scope.getBlogList(0);
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