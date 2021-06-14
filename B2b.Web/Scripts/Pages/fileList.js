b2bApp.controller('FileListController', function ($scope, $http) {
    $(document).ready(function () {

        $http({
            method: "POST",
            url: "/Home/GetFileListType",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            $scope.filelistType = response.data;
            $http({
                method: "POST",
                url: "/Home/GetFileList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}

            }).then(function (response) {
                $scope.filelist = response.data;

            });

        });
    });
    $scope.$on('ngRepeatFileListFinished', function (ngRepeatFileListhedEvent) {
        $('input[name="folderTypes"]').on("change", function () {
            var sType = $(this).data("select");
            var elems = $('[data-type]');

            $.each(elems, function (a, b) {
                var qType = $(b).data('type');
                if (sType !== qType && sType !== "all") $(b).hide('blind'); else $(b).show('blind');
            });
        });
      
      
    });
}).directive('onFinishRender', function ($timeout) {
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
}).filter('convertDate', [
    '$filter', function ($filter) {
        return function (input, format) {
            //  return $filter('date')((new Date(parseInt(input.substr(6))),format(format)));
            return $filter('date')(new Date(parseInt(input.substr(6))), format);
        };
    }
]);