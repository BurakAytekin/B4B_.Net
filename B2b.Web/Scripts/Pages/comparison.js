b2bApp.controller('ComparisonController', function ($scope, $http) {

    $scope.deleteProductComparison = function (id) {
      fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Search/FollowProductOrComparisonRemove",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { Id: id}

        }).then(function (response) {
            fireCustomLoading(false);
            iziToast.success({
                message: 'Ürün Takipten Çıkarıldı. ',
                position: 'topCenter'
            });
            window.location.reload();
        });


    };
    $scope.askAvailable = function (id) {
      fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Search/GetAvailable",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id:id }

        }).then(function (response) {
          fireCustomLoading(false);
            if (response.data !== "[]") {
                var qnt = response.data;
                if (qnt !== "")
                    $scope.askForAdd(id, qnt);
                else
                    $scope.addBasket(id);
            } else {
                $scope.addBasket(id);
            }
        });
    };
    $scope.askForAdd = function (id, qnt) {
        $.confirm({
            title: 'Uyarı!',
            content: "Bu üründen sepetinizde " + qnt + " adet bulunmaktadır. Sepetinize eklemek istiyor musunuz?",
            buttons:
                {
                    EVET: {
                        btnClass: 'btn-blue',
                        action: function () { $scope.addBasket(id); }

                    },
                    HAYIR: {
                        btnClass: 'btn-red any-other-class',
                        action: function () {
                            iziToast.error({
                                message: 'Ekleme işleminiz iptal edilmiştir.',
                                position: 'topCenter'
                            });

                        }
                    }

                }
        });
    };
    $scope.addBasket = function (id) {

        qtyStr = $('#qty_' + id).val();
        if (qtyStr === '')
            qtyStr = $('#qty_' + id).attr('placeholder');

        if ($.isNumeric(qtyStr)) {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Search/AddBasket",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { id: id, qty: qtyStr }

            }).then(function (response) {
                fireCustomLoading(false);
                var retVal = jQuery.parseJSON(response.data);
                if (retVal.statu === "success") {
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

                }
                $('#qty_' + id).val('');
            });
        }
    };


});