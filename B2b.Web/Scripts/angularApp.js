
var adminApp = angular.module('adminApp', ['ngSanitize', 'ngTable', 'ngResource']);
var reportApp = angular.module('reportApp', ['ngSanitize', 'ngTable', 'ngResource']);




//adminApp.run(function ($rootScope) {

//    //Global İşlemler
//    $rootScope.Salesman = {
//        Id: 0
//    };


//});


adminApp.filter("jsonDate", function () {

    return function (x) {
        return new Date(parseInt(x.substr(6)));
    };
}).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
}).filter("dateFilterB2B", function () {
    return function (item) {
        if (item != null) {
            return new Date(item);
        }
        return "";
    };
}).filter("dateFilterErp", function () {
    return function (item) {
        if (item != null) {
            return new Date(Date.parse(item.substr(0, 10)));
        }
        return "";
    };
}).filter("priceFilter", function () {
    return function (item) {
        if (item != null && item !== undefined) {
            return $.number(parseFloat(item), 2, ',', '.');
        }
        return "";
    };
});



var b2bApp = angular.module('b2bApp', ['ngSanitize', 'ngResource','naif.base64']).service("B2BServices", ['$http', function ($http) {



    this.getBasketCount = function () {
        $http({
            method: "POST",
            url: "/Home/GetBasketCount",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {}

        }).then(function (response) {
            var basketCountText = '<i class="fa fa-shopping-cart fa-lg"></i>&nbsp;&nbsp;<span>Sepetim [' + response.data.CustomerCart + ']</span>';
            if (response.data.LoginType === 1)
                basketCountText += '<span> - [' + response.data.SalesmanCart + ']</span>';
            $('#layoutBasketCount').html(basketCountText);

            var total = response.data.CustomerCart;
            var basketCountHeadText = '<span>Sepetim</span><span>[' + response.data.CustomerCart + ']</span>';
            if (response.data.LoginType === 1) {
                basketCountHeadText = '<span>Sepetim</span><span>[' + response.data.CustomerCart + '] - [' + response.data.SalesmanCart + ']</span>';
                total += response.data.SalesmanCart;
            }
            $('#layoutBasketCountHead').html(basketCountHeadText);
            $('#layaoutBasketCountTotal').html(' <span>' + total + '</span>');


        });


    };


}]).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
}).filter("dateFilterErp", function () {
    return function (item) {
        if (item != null) {
            return new Date(Date.parse(item.substr(0, 10)));
        }
        return "";
    };
}).filter("dateFilterB2B", function () {
    return function (item) {
        if (item != null) {
            return new Date(item);
        }
        return "";
    };
}).filter("priceFilter", function () {
    return function (item) {
        if (item != null && item !== undefined) {
            return $.number(parseFloat(item), 2, ',', '.');
        }
        return "";
    };
});


//.run(function ($rootScope) {

//    //Global İşlemler
//    $rootScope.Customer = {
//        Id: 0
//    };


//});;



b2bApp.run(['$rootScope', '$http', function ($rootScope, $http) {


    //Para Birimi Kurları
    $rootScope.getCurrencyList = function (pageName) {
        $http({
            method: "POST",
            url: "/Home/GetCurrencyList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                pageName: pageName
            }
        }).then(function (response) {
            $rootScope.currencyList = response.data;
        });

    };

    $(document).ready(function () {
        $rootScope.getCurrencyList('global');
    });


}]);