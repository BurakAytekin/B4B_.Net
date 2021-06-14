adminApp.controller('ProductRuleSelectController', [
    '$scope', '$http', function ($scope, $http) {
        // #region Veriables
        var mRuleSelect = "#mRuleSelect";
        var returnType = 0;
        $scope.productRuleList = {};
        // #endregion
        $scope.ruleSelect = function (rule) {
            switch (returnType) {
                case 1:

                    $scope.searchCriteria.RuleCode = rule.Product;
                    break;
                case 2:

                    $scope.searchItemCriteria.RuleCode = rule.Product;
                    break;
                default:
                    $scope.selectedProduct.RuleCode = rule.Product;
                    break;
            }
            $(mRuleSelect).modal('hide');
        };

        //0:Ana ürün kartı ekranından gelen 1 :ürün seçmek için açılan popuptan gelen 2:Ürün Seçerek Geriye Item Döndüren Pencereden Gelen değer
        $scope.productRuleOpen = function (type) {
            returnType = type;
            $(mRuleSelect).appendTo("body").modal('show');
            if ($scope.productRuleList.length > 0)
                return;
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/Products/GetProductsRuleList",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {}
            }).then(function (response) {
                fireCustomLoading(false);
                $scope.productRuleList = response.data;
            });
        };

        $scope.$on('ngProductRuleFinished', function (ngProductRuleFinishEvent) {
            $('#tProductRuleTable').DataTable({
                searching: true,
                language: {
                    url: "/Scripts/Admin/vendor/datatables/js/Turkish.json"
                },
                destroy: true,
                paging: false,
                scrollY: 300,
                initComplete: function () {
                    $scope.fireScrollOnHover();
                }
            });
        });

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
    }
]);


