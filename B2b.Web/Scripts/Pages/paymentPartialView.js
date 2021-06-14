var screenWidth = window.screenWidth;

function fireVirtualPosInstallmentInformation() {
    var content = '';
    var localNotes = false;

    $.ajax({
        type: "POST",
        url: "/Partial/GetInstallmentBankCardList",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            if (data !== '[]') {
                var cardTypes = data;
                var installmentIndex = 0;
                //var kartTipi = cardTypes[installmentIndex].Type;
                content += '<div class="payment-installment-boxs">';
                $.each(cardTypes, function () {
                    var bankBg = 'default';
                    switch (this["BankId"]) {
                        case 1:
                            bankBg = "qnb-finansbank";
                            break;

                        case 2:
                        case 34:
                            bankBg = "denizbank";
                            break;

                        case 3:
                        case 27:
                            bankBg = "yapikredi";
                            break;

                        case 4:
                            bankBg = "citibank";
                            break;

                        case 5:
                            bankBg = "deutschebank";
                            break;

                        case 6:
                            bankBg = "eurobank-tekfen";
                            break;

                        case 7:
                            bankBg = "fibabanka";
                            break;

                        case 8:
                            bankBg = "hsbcbank";
                            break;

                        case 9:
                            bankBg = "ingbank";
                            break;

                        case 10:
                        case 31:
                            bankBg = "ziraat";
                            break;

                        case 11:
                            bankBg = "halkbank";
                            break;

                        case 12:
                        case 26:
                            bankBg = "vakifbank";
                            break;

                        case 13:
                            bankBg = "adabank";
                            break;

                        case 14:
                            bankBg = "abank";
                            break;

                        case 15:
                        case 29:
                            bankBg = "anadolu";
                            break;

                        case 16:
                        case 35:
                            bankBg = "sekerbank";
                            break;

                        case 17:
                            bankBg = "tekstilbank";
                            break;

                        case 18:
                            bankBg = "turkishbank";
                            break;

                        case 19:
                        case 32:
                            bankBg = "teb";
                            break;

                        case 20:
                        case 33:
                            bankBg = "garanti";
                            break;

                        case 21:
                        case 30:
                            bankBg = "isbank";
                            break;

                        case 22:
                            bankBg = "akbank";
                            break;

                        case 23:
                            bankBg = "turkiye-finans";
                            break;

                        case 24:
                            bankBg = "electronic-benefit-t";
                            break;

                        case 25:
                            bankBg = "albaraka";
                            break;

                            //case 26:
                            //    bankBg = "vakifbank";
                            //    break;

                            //case 27:
                            //    bankBg = "yapikredi";
                            //    break;

                        case 28:
                            bankBg = "fortis";
                            break;

                            //case 29:
                            //    bankBg = "anadolu";
                            //    break;

                            //case 30:
                            //    bankBg = "isbank";
                            //    break;

                            //case 31:
                            //    bankBg = "ziraat";
                            //    break;

                            //case 32:
                            //    bankBg = "teb";
                            //    break;

                            //case 33:
                            //    bankBg = "garanti";
                            //    break;

                            //case 34:
                            //    bankBg = "denizbank";
                            //    break;

                            //case 35:
                            //    bankBg = "sekerbank";
                            //    break;

                        case 36:
                        case 42:
                            bankBg = "bank-asya";
                            break;

                        case 37:
                            bankBg = "kuveytturk";
                            break;

                        case 38:
                            bankBg = "t-bank";
                            break;

                        case 39:
                            bankBg = "burganbank";
                            break;

                        case 40:
                            bankBg = "aktif-yatirim";
                            break;

                        case 41:
                            bankBg = "odeabank";
                            break;

                            //case 42:
                            //    bankBg = "bank-asya";
                            //    break;

                        case 43:
                            bankBg = "ptt";
                            break;

                        default:
                            bankBg = "default";
                            break;
                    }

                    content += '<div class="payment-installment-box">';
                    content += '    <div class="installment-box bank-' + bankBg + '">';
                    content += '        <div class="card-image">';
                    content += '            <div class="card-image-content">';
                    content += '                <img alt="Banka Adı" src="/content/images/banks/bcc-' + this['BankId'] + '.png" />';
                    content += '            </div>';
                    content += '        </div>';

                    content += '        <div class="installment-content">';
                    $.ajax({
                        type: "POST",
                        url: "/Partial/GetBankInstalmentList",
                        data: "{Type:'" + this['Type'] + "',bankId:" + this['BankId'] + ",posBankId:" + this["PosBankId"] +"}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data !== '[]') {
                                var datas = data;
                                $.each(datas, function () {
                                    content += '        <div class="row-group">';
                                    content += '            <div class="rows">' + this['Installment'] + ((parseInt(this['ExtraInstallment']) !== 0) ? " + " + this['ExtraInstallment'] + " Taksit" : " Taksit") + '</div>';
                                    //content += '            <div class="rows">' + ((parseInt(this['DeferalInstallment']) !== 0) ? this['DeferalInstallment'] + ' Ay Erteleme' : "") + '</div>';
                                    //content += '            <div class="rows">' + ((parseInt(this['CommissionRate']) !== 0) ? "%" + this['DeferalInstallment'] + ' Komisyon' : "") + '</div>';
                                    var localNote = ' ';
                                    if (this['Note'] !== null && this['Note'] !== "") {
                                        localNote = '<img src="/content/images/danger2.png" class="img-icon tooltip" title="' + this['Note'] + '" />';
                                        localNotes = true;
                                    }
                                    content += '            <div class="rows">' + localNote + '</div>';
                                    content += '    </div>';
                                });
                            }
                        },
                        error: function (msg) {
                            alert('Error Installment Load: ' + msg.responseText);
                        }
                    });
                    content += '        </div>';

                    content += '        <div class="bank-logo">';
                    content += '            <div class="bank-logo-content">';
                    content += '                <img alt="Banka Adı" src="/content/images/banks/b-' + this['BankId'] + '.png" />';
                    content += '            </div>';
                    content += '        </div>';
                    content += '    </div>';
                    content += '</div>';
                    installmentIndex++;

                    if (((screenWidth >= 500 && screenWidth != 1024) && installmentIndex % 4 === 0) || (screenWidth === 1024 && installmentIndex % 3 === 0) || (screenWidth < 500 && installmentIndex % 2 === 0))
                        content += '</div><div class="payment-installment-boxs">';
                });
                content += '</div>';

                $('#pnlInstallmentInformation').append(content).removeClass("hidden");
            }

        },
        error: function (msg) {
            alert('Error Virtual Pos Installment Information: ' + msg.responseText);
        },
        complete: function () {
            if (localNotes)
                fireTooltips();

            $('.stepContainer').height($('.payment-installment-boxs-wrapper').outerHeight() + 20);
            //$.ajax({
            //    type: "POST",
            //    url: "PaymentPage.aspx/GetVirtualPosAnnouncementMessage",
            //    data: "{}",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    async: true,
            //    success: function (data) {
            //        if (data.d !== '[]') {
            //            var installmentMessage = '';
            //            $.each(jQuery.parseJSON(data.d), function () {
            //                installmentMessage += '<li data-title="' + this['Header'] + '">' + this['Content'] + '</li>';
            //            });

            //            $('#pnlInstallmentMessage').html('<div class="info alert-no-margin"><ul>' + installmentMessage + '</ul></div>').removeClass("hidden-all");
            //        }
            //        else {
            //            $('#pnlInstallmentMessage').empty().remove();
            //        }
            //    }//,
            //    //error: function (msg) {
            //    //    alert('Error Payment Page - Virtual Pos Announcement Message ' + msg.responseText);
            //    //},
            //    //complete: function () {}
            //});
        }
    });
}

function fireTooltips() {
    $('.tooltip').tooltipster({
        position: (screenWidth <= 500) ? 'bottom' : 'left',
        speed: 100,
        maxWidth: (screenWidth <= 500) ? 300 : null,
        theme: 'tooltipster-light',
        animation: 'fade', // fade, grow, swing, slide, fall
        multiple: true,
        trigger: (screenWidth <= 500) ? 'click' : 'hover',
        //position: 'bottom-left',
        contentAsHTML: true,
        contentCloning: false,
        positionTracker: true
    });
    $('.tooltip').removeClass('tooltip');
}


//$(document).ready(function () {
//
//    fireVirtualPosInstallmentInformation();
//});