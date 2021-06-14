function removeFollowProduct(id) {

    fireCustomLoading(true);

    $.ajax({
        type: "POST",
        url: "/Search/FollowProductOrComparisonRemove",
        data: '{Id:' + id +' }',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tbUsersTrresult_' + id).remove();
            iziToast.success({
                message: 'Ürün Takipten Çıkarıldı.',
                position: 'topRight'
            });
        },
        error: function () {
            iziToast.error({
                message: 'İşlem Sıransında Hata Oluştu.',
                position: 'topCenter'
            });
        }
    });
    fireCustomLoading(false);


}