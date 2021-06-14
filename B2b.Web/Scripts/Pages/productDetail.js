
function getAlternativeList() {
    fireCustomLoading(true);
    $.ajax({
        type: "POST",
        url: "/Search/GetAlternativeList",
        data: '{ groupId: ' + groupId + ' }',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            var text = '';
            if (response.length !== 0) {
                for (var i = 0; i < response.length; i++) {


                    var trClass = response[i].Campaign.Type > 0 ? "campaign" : (response[i].NewProduct === 1 ? "new-product" : "");
                    text += '<tr id="tr' + response[i].Id + '" class="' + trClass + '" >';
                    text += '   <td>' + response[i].Code + '</td>';
                    text += '   <td>' + response[i].Name + '</td>';
                    text += '   <td>' + response[i].Manufacturer + '</td>';
                    text += '   <td>' + response[i].ManufacturerCode + '</td>';
                    text += '   <td>' + response[i].Unit + '</td>';
                    text += '   <td>';
                    if (response[i].HavePicture)
                        text += '       <i class="fa fa-picture-o fa-lg" onclick="angular.element(document.getElementById(\'divSearch\')).scope().fireProductSlider(' + response[i].Id + ',\'' + response[i].Code + '\')"></i>';
                    text += '   </td>';
                    text += '   <td>' + response[i].DiscountStr + '</td>';
                    //aria-hidden="\"true\""
                    text += '   <td class="text-right" onmouseover="angular.element(document.getElementById(\'divSearch\')).scope().titlePriceValue(event,\'' + response[i] + '\',' + response[i].Id + ')">' + response[i].PriceNetShowStr + '</td>';
                    text += '   <td  class="text-center"><span onmouseover="angular.element(document.getElementById(\'divSearch\')).scope().titleWarehouseValue(event,\'' + response[i] + '\',' + response[i].Id + ')"><span class="fa-stack fa-sm"><i class="' + response[i].AvailabilityCss + '"></i></span><span class="ng-binding">' + response[i].AvailabilityText + '</span></span></td>';
                    text += '   <td>';
                    text += '       <div class="input-group input-group-sm input-criteria">';
                    text += '           <input id="qtyD' + response[i].Id + '" type="text" onkeypress="angular.element(document.getElementById(\'divSearch\')).scope().keyPressedBasket(event,' + response[i].Id + ',2)" placeholder="' + response[i].MinOrder + '" class="form-control text-center search-input">';
                    text += '           <span class="input-group-btn search-qty-btn">';
                    text += '               <a class="btn btn-custom-2" href="javascript:;" onclick="angular.element(document.getElementById(\'divSearch\')).scope().askAvailable(' + response[i].Id + ',2)"><i class="fa fa-shopping-cart fa-lg"></i></a>';
                    text += '           </span>';
                    text += '       </div>';
                    text += '   </td>';
                    text += '   </td>';
                    text += '</tr>';
                }
            }
            else {
                text += '<div class="info">Veri bulunamadı.</div>';
            }
            $('#tbResultDetail_' + productId).empty().append(text);
            fireCustomLoading(false);
        },
        error: function () {
            $('#tbResultDetail_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });
}

function getLinkedList() {
    fireCustomLoading(true);
    $.ajax({
        type: "POST",
        url: "/Search/GetLinkedList",
        data: '{ productId: ' + productId + ' }',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            var text = '';
            if (response.length !== 0) {
                for (var i = 0; i < response.length; i++) {


                    var trClass = response[i].Campaign.Type > 0 ? "campaign" : (response[i].NewProduct === 1 ? "new-product" : "");
                    text += '<tr id="tr' + response[i].Id + '" class="' + trClass + '" >';
                    text += '   <td class="text-left-important">' + response[i].Code + '</td>';
                    text += '   <td class="text-left-important">' + response[i].Name + '</td>';
                    text += '   <td>' + response[i].Manufacturer + '</td>';
                    text += '   <td>' + response[i].ManufacturerCode + '</td>';
                    text += '   <td>' + response[i].Unit + '</td>';
                    text += '   <td>';
                    if (response[i].HavePicture)
                        text += '       <i class="fa fa-picture-o fa-lg" onclick="angular.element(document.getElementById(\'divSearch\')).scope().fireProductSlider(' + response[i].Id + ',\'' + response[i].Code + '\')"></i>';
                    text += '   </td>';
                    text += '   <td>' + response[i].DiscountStr + '</td>';
                    //aria-hidden="\"true\""
                    text += '   <td class="text-right" onmouseover="angular.element(document.getElementById(\'divSearch\')).scope().titlePriceValue(event,' + response[i].Id + ')">' + response[i].PriceListStr + '</td>';
                    text += '   <td  class="text-center"><span class="fa-stack fa-1x" onmouseover="angular.element(document.getElementById(\'divSearch\')).scope().titleWarehouseValue(event,' + response[i].Id + ')"><i class="' + response[i].AvailabilityCss + '"></i><strong class="fa-stack-1x calendar-text">' + response[i].AvailabilityText + '</strong></span></td>';
                    text += '   <td>';
                    text += '       <div class="input-group input-group-sm input-criteria">';
                    text += '           <input id="qtyD' + response[i].Id + '" type="text" onkeypress="angular.element(document.getElementById(\'divSearch\')).scope().keyPressedBasket(event,' + response[i].Id + ',2)" placeholder="' + response[i].MinOrder + '" class="form-control text-center search-input">';
                    text += '           <span class="input-group-btn search-qty-btn">';
                    text += '               <a class="btn btn-custom-2 " href="javascript:;" onclick="angular.element(document.getElementById(\'divSearch\')).scope().askAvailable(' + response[i].Id + ',2)"><i class="fa fa-shopping-cart fa-lg"></i></a>';
                    text += '           </span>';
                    text += '       </div>';
                    text += '   </td>';
                    text += '   </td>';
                    text += '</tr>';
                }
            }
            else {
                text += '<div class="info">Veri bulunamadı.</div>';
            }
            $('#tbResultLinkedDetail_' + productId).empty().append(text);
            fireCustomLoading(false);
        },
        error: function () {
            $('#tbResultLinkedDetail_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });
}

function getOemRivalCodeList(type) {
    fireCustomLoading(true);
    $.ajax({
        type: "POST",
        url: "/Search/GetOemRivalCodeList",
        data: '{groupId:' + groupId + ', type: ' + type + ' }',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            var text = '';

            if (response.length !== 0) {

                for (var i = 0; i < response.length; i++) {
                    text += '   <tr id="tr' + response[i].Id + '">';
                    text += '    <td>' + response[i].Brand + '</td>';
                    text += '    <td>' + response[i].OemNo + '</td>';

                    text += '    <td class="text-right">';
                    text += '       <div class="dropdown">';
                    text += '           <button class="btn btn-default btn-xs btn-custom dropdown-toggle" type="button" id="dropdownMenu2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                    text += '              <i class="fa fa-cog fa-lg"></i>';
                    text += '              <span class="caret"></span>';
                    text += '           </button>';
                    text += '          <ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu2">';
                    text += '              <li><a href="javascript:;" onclick="askBlackList(' + response[i].Id + ',' + response[i].ProductId + ',0)" ><i class="fa fa-ban fa-fw"></i> Kara Listeye Ekle</a></li>';
                    text += '              <li><a href="javascript:;" onclick="askBlackList(' + response[i].Id + ',' + response[i].ProductId + ',1)"><i class="fa fa-pencil fa-fw"></i> Düzeltme Talebi</a></li>';
                    text += '         </ul>';
                    text += '     </div>';
                    text += '    </td>';
                    text += '    </tr>';

                }

            }
            else {
                text += '<div class="info">Veri bulunamadı.</div>';
            }
            if (type === 0)
                $('#tbResultOem_' + productId).empty().append(text);
            else
                $('#tbResultRivalCode_' + productId).empty().append(text);
            fireCustomLoading(false);
        },
        error: function () {
            if (type === 0)
                $('#tbResultOem_' + productId).html('<div class="error">Data listenemiyor.</div>');
            else
                $('#tbResultRivalCode_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });

}

function askBlackList(id, productId, type) {
    var newNameStyle = type === 0 ? 'style="display:none"' : 'style="display:normal"';

    var confirmTitle = "Uyarı Mesajı!";
    var confirmContent = '<div class="formName row">' +
                            '<div class="col-md-12" ' + newNameStyle + '>' +
                                '<div class="input-group input-group-md input-criteria only-input">' +
                                    '<input class="newName form-control" type="text"  placeholder="Yeni Değer Giriniz.." required />' +
                                '</div>' +
                            '</div>' +
                            '<div class="col-md-12">' +
                                '<div class="input-group input-group-md input-criteria only-input">' +
                                    '<input class="note form-control" type="text"  placeholder="Not Giriniz.." required />' +
                                '</div>' +
                            '</div>' +
                         '</div>';
    var confirmButtons = [
        {
            n: "Save",
            t: "Kaydet",
            c: "btn btn-xs btn-success",
            f: function () {
                var note = this.$content.find('.note').val();
                var newName = this.$content.find('.newName').val();
                if ((type === 0 && !note) || (type === 1 && !newName && !note)) {
                    $.alert('Lütfen Not Giriniz');
                    return false;
                }
                addOemBlackList(id, productId, newName, note, type);
            }
        },
        {
            n: "Cancel",
            t: "Vazgeç",
            c: "btn btn-xs btn-danger",
            f: function () { }
        }
    ];


    angular.element(document.getElementById('divSearch')).scope().fireConfirm(confirmTitle, confirmContent, confirmButtons);


    // $.confirm({
    //     title: 'Uyarı!',
    //     content: '' +
    //'<form action="" class="formName">' +
    //'<div class="form-group">' +
    // //'<label>Not Giriniz..</label>' +
    //'<input type="text" ' + newNameStyle + ' placeholder="Yeni Değer Giriniz.." class="newName form-control" required />' +
    //'<label></label>' +
    //'<input type="text" placeholder="Not Giriniz.."  class="note form-control" required />' +
    //'</div>' +
    //'</form>',
    //     buttons: {
    //         formSubmit: {
    //             text: 'Kaydet',
    //             btnClass: 'btn-blue',
    //             action: function () {
    //                 var note = this.$content.find('.note').val();
    //                 var newName = this.$content.find('.newName').val();
    //                 if ((type === 0 && !note) || (type === 1 && !newName && !note)) {
    //                     $.alert('Lütfen Not Giriniz');
    //                     return false;
    //                 }
    //                 AddOemBlackList(id, newName, note, type);
    //             }
    //         },
    //         Vazgeç: function () {
    //             //close
    //         },
    //     },
    // });
}

function addOemBlackList(id, productId, newName, note, type) {
    fireCustomLoading(true);
    $.ajax({
        type: "POST",
        url: "/Search/AddOemBlackList",
        data: "{ id: " + id + ",productId:" + productId + ",newName: '" + newName + "', note: '" + note + "' ,type:" + type + "}",
        contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (response) {
            if (response.Statu === "success") {
                iziToast.success({
                    message: response.Message,
                    position: 'topCenter'
                });

            }
            else {
                iziToast.error({
                    //title: 'Hata',
                    message: response.Message,
                    position: 'topCenter'
                });

            }
            fireCustomLoading(false);

        },
        error: function () {
            fireCustomLoading(false);
        }
    });
}

function getVehicleList() {
    fireCustomLoading(true);
    $.ajax({
        type: "POST",
        url: "/Search/GetVehicleList",
        data: '{groupId:' + groupId + '}',
        contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (response) {
            var text = '';

            if (response.length !== 0) {

                for (var i = 0; i < response.length; i++) {
                    text += '   <tr >';
                    text += '    <td>' + response[i].Brand + '</td>';
                    text += '    <td>' + response[i].Model + '</td>';
                    text += '    <td>' + response[i].Type + '</td>';
                    text += '    <td>' + response[i].Date + '</td>';
                    text += '    <td>' + response[i].EngineCode + '</td>';
                    text += '    <td>' + response[i].Hp + '</td>';
                    text += '    <td>' + response[i].Kw + '</td>';
                    text += '    </tr>';

                }

            }
            else {
                text += '<div class="info">Veri bulunamadı.</div>';
            }
            $('#tbResultVehicle_' + productId).empty().append(text);
            fireCustomLoading(false);
        },
        error: function () {
            $('#tbResultVehicle_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });

}

function getProductOrderList() {
    fireCustomLoading(true);

    $("#comboYear_" + productId).empty();
    $.ajax({
        type: "POST",
        url: "/Search/GetProductOrderYear",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (response) {
            var text = '';

            var yearId = 0;

            for (var i = 0; i < response.length; i++) {
                $("#comboYear_" + productId).append($("<option></option>").val(response[i].Id).html(response[i].Header));
            };

            $('#comboYear_' + productId).on('change', function () {
                yearId = this.value;
                getProductOrderListDetail(yearId);
            })

            yearId = (response.length > 0 ? response[0].Id : -1);

            getProductOrderListDetail(yearId);

            // $('#tbResultVehicle_' + productId).empty().append(text);
        },
        error: function () {
            fireCustomLoading(false);
        }
    });

}


function getProductOrderListDetail(yearId) {

    $.ajax({
        type: "POST",
        url: "/Search/GetProductOrderList",
        data: "{productCode:'" + productCode + "',id:" + yearId + "}",
        contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (response) {
            var text = '';

            if (response.length !== 0) {

                for (var i = 0; i < response.length; i++) {
                    text += '   <tr >';
                    text += '    <td>' + new Date(parseInt((response[i].Date).substr(6))).format("dd/mm/yyyy") + '</td>';
                    text += '    <td>' + response[i].DocumentNo + '</td>';
                    text += '    <td>' + response[i].ProductCode + '</td>';
                    text += '    <td>' + response[i].ProductName + '</td>';
                    text += '    <td>' + response[i].Manufacturer + '</td>';
                    text += '    <td>' + response[i].Quantity + '</td>';
                    text += '    <td>' + response[i].Unit + '</td>';
                    text += '    <td>' + response[i].Price + '</td>';
                    text += '    <td>' + response[i].Total + '</td>';
                    text += '    <td>' + response[i].Discount1 + '</td>';
                    text += '    <td>' + response[i].Discount2 + '</td>';
                    text += '    <td>' + response[i].Discount3 + '</td>';
                    text += '    <td>' + response[i].Discount4 + '</td>';
                    text += '    </tr>';

                }

            }
            else {
                text += '<div class="info">Veri bulunamadı.</div>';
            }
            $('#tbResultProductOrder_' + productId).empty().append(text);
            fireCustomLoading(false);
        },
        error: function () {
            $('#tbResultProductOrder_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });
}

function getProductExplanation() {
    fireCustomLoading(true);
    $.ajax({
        type: "POST",
        url: "/Search/GetProductExplanation",
        data: '{productId:' + productId + '}',
        contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (response) {
            $('#explanation_' + productId).empty().append(response.Notes);
            fireCustomLoading(false);
        },
        error: function () {
            $('#explanation_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });

}

function getCampaignExplanation() {

    $.ajax({
        type: "POST",
        url: "/Search/GetCampaignDetail",
        data: "{productId:" + productId + "}",
        contentType: "application/json; charset=utf-8",
        // dataType: "json",
        success: function (response) {
            var text = '';
            text += '<div class="tab-content clearfix">';
            text += '   <div class="tab-pane active" id="shipping">';
            text += '     <div class="row">';
            text += '         <div class="col-md-6">';
            text += '              <ul class="list-group">';
            text += '             <li class="list-group-item">';
            text += '                <div class="row">';
            text += '                    <div class="col-sm-12">';
            text += '                      <div class="col-sm-4">Kampanya Tipi</div>';
            text += '                       <div class="col-sm-8">';
            text += response.Campaign.TypeStr;
            text += '                      </div>';
            text += '                   </div>';
            text += '                </div>';
            text += '             </li>';
            text += '                 <li class="list-group-item">';
            text += '                     <div class="row">';
            text += '                         <div class="col-sm-12">';
            text += '                             <div class="col-sm-4">Başlangıç Tarihi</div>';
            text += '                             <div class="col-sm-8">' + ((new Date(parseInt(response.Campaign.StartDate.substr(6)))).format("dd.mm.yyyy")) + '</div>';
            text += '                        </div>';
            text += '                    </div>';
            text += '               </li>';
            text += '                <li class="list-group-item">';
            text += '                    <div class="row">';
            text += '                        <div class="col-sm-12">';
            text += '                           <div class="col-sm-4">Bitiş Tarihi</div>';
            text += '                           <div class="col-sm-8">' + ((new Date(parseInt(response.Campaign.FinishDate.substr(6)))).format("dd.mm.yyyy")) + '</div>';
            text += '                        </div>';
            text += '                   </div>';
            text += '               </li>';

            text += '             <li class="list-group-item">';
            text += '                 <div class="row">';
            text += '                     <div class="col-sm-12">';
            text += '                         <div class="col-sm-4">Kampanya Adeti</div>';
            text += '                         <div class="col-sm-8">' + response.Campaign.TotalQuantity + '</div>';
            text += '                    </div>';
            text += '                 </div>';
            text += '             </li>';

            text += '           <li class="list-group-item">';
            text += '              <div class="row">';
            text += '                  <div class="col-sm-12">';
            text += '                     <div class="col-sm-4">Kampanya Kodu</div>';
            text += '                     <div class="col-sm-8">' + response.Campaign.Code + '</div>';
            text += '                 </div>';
            text += '             </div>';
            text += '          </li>';
            text += '      </ul>';
            text += '    </div>';
            text += '   <div class="col-md-6">';
            if (response.Campaign.Type === 4 || response.Campaign.Type === 5) {
                text += ' <div id="pnlTable" class="y-scrool-200">';
                text += '              <table id="pDataTable" class="table table-striped table-hover table-custom">';
                text += '                  <thead>';
                text += '                     <tr>';
                text += '                          <th data-field="Id" data-sortable="false">Kod</th>';
                text += '                        <th data-field="Id" data-sortable="false">Fiyat</th>';
                text += '                       <th data-field="Code" data-sortable="true">İskonto</th>';
                text += '                       <th data-field="Code" data-sortable="true">Minimum Adet</th>';
                text += '                  </tr>';
                text += '               </thead>';
                text += '               <tbody>';

                for (var i = 0; i < response.CampaignList.length; i++) {
                    text += '                        <tr>';
                    text += '                          <td>' + response.CampaignList[i].Code + '</td>';
                    text += '                          <td>' + response.CampaignList[i].PriceStr + '</td>';
                    text += '                          <td>' + response.CampaignList[i].Discount + '</td>';
                    text += '                          <td>' + response.CampaignList[i].MinOrder + '</td>';
                    text += '                        </tr>';
                }
                text += '              </tbody>';
                text += '          </table>';
                text += '        </div>';
            }
            else {
                text += '              <ul class="list-group">';
                text += '             <li class="list-group-item">';
                text += '                <div class="row">';
                text += '                    <div class="col-sm-12">';
                text += '                      <div class="col-sm-4">Min. Adet</div>';
                text += '                       <div class="col-sm-8">';
                text += response.Campaign.MinOrder;
                text += '                      </div>';
                text += '                   </div>';
                text += '                </div>';
                text += '             </li>';
                text += '                 <li class="list-group-item">';
                text += '                     <div class="row">';
                text += '                         <div class="col-sm-12">';
                text += '                             <div class="col-sm-4">Kampanya Fiyatı</div>';
                text += '                             <div class="col-sm-8">' + response.CampaignPriceCustomerStr + '</div>';
                text += '                        </div>';
                text += '                    </div>';
                text += '               </li>';
                text += '                <li class="list-group-item">';
                text += '                    <div class="row">';
                text += '                        <div class="col-sm-12">';
                text += '                           <div class="col-sm-4">Kdvli Kampanya Fiyatı</div>';
                text += '                           <div class="col-sm-8">' + response.CampaignPriceWithVatCustomerStr + '</div>';
                text += '                        </div>';
                text += '                   </div>';
                text += '               </li>';

                text += '             <li class="list-group-item">';
                text += '                 <div class="row">';
                text += '                     <div class="col-sm-12">';
                text += '                         <div class="col-sm-4">İskonto</div>';
                text += '                         <div class="col-sm-8">' + response.Campaign.Discount + ' %</div>';
                text += '                    </div>';
                text += '                 </div>';
                text += '             </li>';

                text += '           <li class="list-group-item">';
                text += '              <div class="row">';
                text += '                  <div class="col-sm-12">';
                text += '                     <div class="col-sm-4">Kampanya Kodu</div>';
                text += '                     <div class="col-sm-8">' + response.Campaign.Code + '</div>';
                text += '                 </div>';
                text += '             </div>';
                text += '          </li>';
                text += '      </ul>';
            }

            text += '  </div>';

            text += '</div>';
            text += '       </div>';
            text += ' </div>';

            $('#campaign_' + productId).empty().append(text);
            fireCustomLoading(false);
        },
        error: function () {
            $('#tbResultProductOrder_' + productId).html('<div class="error">Data listenemiyor.</div>');
            fireCustomLoading(false);
        }
    });




}



