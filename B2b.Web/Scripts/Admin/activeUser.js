adminApp.controller('activeUserController', ['$scope', '$http', function ($scope, $http) {

    $(document).ready(function () {
         $.connection.hub.start();
    });

    $scope.activeUsersSalesmanList = {};
    $scope.activeUsersCustomerList = {};

    $.connection.hub.start();
    var vEryazActiveUser = $.connection.EryazActiveUsers;

    $scope.control = true;

    $scope.showUser = function () {
        $scope.control = true;
        //$('#customerActiveUser').empty();
        //$('#salesmanActiveUser').empty();

        $.each($('body').attr('class').split(' '), function (index, className) {
            if (className.indexOf('rightbar-show') === 0) {
                $scope.control = false;
                $scope.activeUsersSalesmanList = {};
                $scope.activeUsersCustomerList = {};
            }
        });

        if ($scope.control === true) {
            vEryazActiveUser.server.getActiveUserList().done(function (onlineUsers) {
                $scope.activeUsersCustomer = [];
                $scope.activeUsersSalesman = [];


                $.each(onlineUsers, function () {
                    $scope.vUsers = [];
                    $scope.vUsers.push({ 'Key': this["Key"], 'Value': this["Value"][0] });
                    if (this["Key"] !== "" && this["Key"].substring(0, 1) === "0") {
                        $scope.userItems = [];
                        $scope.vUser = $scope.vUsers[0];
                        var userStr = $scope.vUser.Key.split('&&');
                        $scope.userItems.push({ Id: $scope.vUser.Value, Type: parseInt(userStr[1]), Code: userStr[2], Name: userStr[3], Key: $scope.vUser.Key });
                        $scope.userItem = $scope.userItems[0];
                        if ($scope.userItem.Type === 0)
                            $scope.activeUsersCustomer.push($scope.userItem);
                        else
                            $scope.activeUsersSalesman.push($scope.userItem);

                    }
                });
                $scope.activeUsersSalesmanList = angular.copy($scope.activeUsersSalesman);
                $scope.activeUsersCustomerList = angular.copy($scope.activeUsersCustomer);

                var salesmanHtml = '';


                angular.forEach($scope.activeUsersSalesmanList, function (value, key) {

                    salesmanHtml += '  <li class="online masonry-brick">';
                    salesmanHtml += '   <div class="media">';
                    salesmanHtml += '       <a role="button" tabindex="0" class="pull-left thumb thumb-sm">';
                    salesmanHtml += '         <img class="media-object img-circle" src="" alt="">';
                    salesmanHtml += '                  </a>';
                    salesmanHtml += '           <div class="media-body">';
                    salesmanHtml += '             <span class="media-heading">- <strong>' + value.Code + '</strong></span>';
                    salesmanHtml += '              <small>' + value.Name + '</small>';
                    salesmanHtml += '              <span  class="badge badge-outline status"></span>';
                    salesmanHtml += '           </div>';
                    salesmanHtml += '             </div>';
                    salesmanHtml += '           </li>';

                });

                $('#salesmanActiveList').empty().append(salesmanHtml);

                var customerHtml = '';


                angular.forEach($scope.activeUsersCustomerList, function (value, key) {

                    customerHtml += '  <li class="online masonry-brick">';
                    customerHtml += '   <div class="media">';
                    customerHtml += '       <a role="button" tabindex="0" class="pull-left thumb thumb-sm">';
                    customerHtml += '         <img class="media-object img-circle" src="" alt="">';
                    customerHtml += '                  </a>';
                    customerHtml += '           <div class="media-body">';
                    customerHtml += '             <span class="media-heading">- <strong>' + value.Code + '</strong></span>';
                    customerHtml += '              <small>' + value.Name + '</small>';
                    customerHtml += '              <span  class="badge badge-outline status"></span>';
                    customerHtml += '           </div>';
                    customerHtml += '             </div>';
                    customerHtml += '           </li>';

                });
                $('#customerActiveList').empty().append(customerHtml);
                $('#activeSalesmanLengt').html($scope.activeUsersSalesmanList.length);
                $('#activeCustomerLengt').html($scope.activeUsersCustomerList.length);

            });
        }
    };

    $scope.disconnectActiveUser = function (pName, pContextId) {
        vEryazActiveUser.server.disconnectUser(pName, pContextId);
    }

}]);