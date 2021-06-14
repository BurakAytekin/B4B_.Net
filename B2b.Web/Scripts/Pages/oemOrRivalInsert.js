function saveOemOrRival(type)//0:Oem Girişi 1:Rakip Kod Girişi
{
    var vehivleBrand, newData,addType;
    if (type === 0) {
        vehivleBrand = $('#oemBrandName').val();
        newData = $('#oemNo').val();
        addType = 2;
    } else {
        vehivleBrand = $('#rivalBrandName').val();
        newData = $('#rivalNo').val();
        addType = 3;
    }
    if (newData !== '') {
        $.ajax({
            type: "POST",
            url: "/Search/AddOemOrRivalCode",
            data: "{ productId: " + productId + ",newData: '" + newData + "', vehicleBrand: '" + vehivleBrand + "' ,type:" + addType + "}",
            contentType: "application/json; charset=utf-8",
            // dataType: "json",
            success: function(response) {
                if (response.Statu === "success") {
                    $('#rivalBrandName').val('');
                    $('#oemBrandName').val('');
                    $('#oemNo').val('');
                    $('#rivalNo').val('');
                    iziToast.success({
                        message: response.Message,
                        position: 'topCenter'
                    });

                } else {
                    iziToast.error({
                        //title: 'Hata',
                        message: response.Message,
                        position: 'topCenter'
                    });

                }

            },
            error: function() {

            }
        });
    } else {
        iziToast.error({
            //title: 'Hata',
            message: "Lütfen Gerekli Alanları Doldurunuz.",
            position: 'topCenter'
        });
    }

}