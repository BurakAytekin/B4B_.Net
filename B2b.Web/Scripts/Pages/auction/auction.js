var auctionHub;
var appMain;
// Define the auction module
ns_sra.app.auction = (function (appMain) {

    // Private members
    //var
    //    // Get the proxy
    //    auctionHub = $.connection.auctionHub,

    // Initiialize
    init = function () {

    },

    refreshAuction = function (model) {
        $('#prod-value-bid').val(model.ValueNextBid);
        $('#total-bids span').text(model.BidsTotal);

        //$('#value-time').text(utilTimer.getRemainingTime(new Date(model.EndTimeFullText)));

        //utilTimer.initializeDecrementTimeRemaining();
    },

    notifyNewBid = function (notification) {
        $('#recent-bids ul').prepend('<li>' + notification + '</li>');
    };


    //validBid = function () {
    //    var value = $('#prod-value-bid').val(),
    //        user = $('#user input:text').val();

    //    return value !== '' && user !== '';
    //};

    // Call when a instance of the module is created
    init();

    // Public members
    return {
        //validBid: validBid,
        refreshAuction: refreshAuction,
        notifyNewBid: notifyNewBid
    };
});



$(document).ready(function () {
    var startDate = new Date(auctionModel.StartDate);
    var endDate = new Date(auctionModel.EndDate);
    var newDate = startDate.getFullYear() + '/' + (startDate.getMonth() + 1) + '/' + startDate.getDate() + " " + startDate.getHours() + ":" + startDate.getMinutes() + ":" + startDate.getSeconds();

    if (auctionModel.Id > 0 && startDate < new Date() && endDate > new Date()) {
      actionStart2();
    }
    else if (auctionModel.Id = 0 || (startDate < new Date() && endDate < new Date())) {
        $.fancybox.open('<div>Şu anda bir açık arttırma bulunmamaktadır </div>', {
            closeBtn: false,
            'showCloseButton': false
        });
        $.connection.hub.stop();
    }
    else {

    $.fancybox.open('<div>Açık Arttırmaya Kalan Süre <div id="getting-open">00:00</div></div>', {
        closeBtn: false,
        'showCloseButton': false,
        'onComplete': function () {

            $('#getting-open').countdown(newDate, function (event) {
                $(this).html(event.strftime('%H:%M:%S'));
            }).on('finish.countdown', function () {
                $.fancybox.close();

                //if (auctionModel.Id > 0 && startDate < new Date() && endDate > new Date()) {
                //    actionStart2();
                //}
                //else
                //if (auctionModel.Id = 0 || (startDate < new Date() && endDate < new Date())) {
                //    $.fancybox.open('<div>Şu anda bir açık arttırma bulunmamaktadır </div>', {
                //        closeBtn: false,
                //        'showCloseButton': false
                //    });
                //}
                //else {

                    actionStart();
                //}
            });
        }
    });
    }



});


function actionStart2() {

    $.connection.hub.start().done(function () {

        console.log('Now connected, connection ID=' + $.connection.hub.id);
        var date = new Date(auctionModel.EndDate);
        var newDate = date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();

        $('#getting-started').countdown(newDate, function (event) {
            $(this).html(event.strftime('%H:%M:%S'));
        }).on('finish.countdown', function () {
            $.fancybox.open('<div>Açık arttırma kapanmıştır </div>', {
                closeBtn: false,
                'showCloseButton': false
            });
        });

    }).fail(function () {

        actionStart();

    });
}

function actionStart() {

    //if ($.connection.hub.id != '') {
    //    alert(1);
    //}
    //else {
    //    alert(2);
    //}
    $.connection.hub.stop();
    $.connection.hub.start().done(function () {
        auctionHub.server.callRefresh();
    });

    var date = new Date(auctionModel.EndDate);
    var newDate = date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();

    $('#getting-started').countdown(newDate, function (event) {
        $(this).html(event.strftime('%H:%M:%S'));
    }).on('finish.countdown', function () {
        $.fancybox.open('<div>Açık arttırma kapanmıştır </div>', {
            closeBtn: false,
            'showCloseButton': false
        });
    });

    //$.connection.hub.stop();
    auctionHub.server.resetCurrentAuction(auctionModel).done(function () {
        appMain.showMessageNotification('Açık Arttırma Başladı');
    });
    
}

function resetAction() {
    auctionHub.server
     .resetCurrentAuction(auctionModel).done(function () {
         appMain.showMessageNotification('Açık Arttırma Başladı');
     });
}



$(function () {

    //utilTimer = ns_sra.util.timer(),  
    auctionHub = $.connection.auctionHub;
    var appAuction = ns_sra.app.auction(appMain);

    appMain = ns_sra.app.main();
    // Declare a function on the chat hub so the server can invoke it
    auctionHub.client.auctionRefresh = function (model) {
        appAuction.refreshAuction(model);
    };

    auctionHub.client.notifyNewBid = function (notification) {
        appAuction.notifyNewBid(notification);
    };

    auctionHub.client.addMessage = function (message) {
        $('#message').append('<p>' + message + '</p>');
    };

    $("#prod-btn-bid").click(function () {
        //if (!appAuction.validBid()) {
        //    appMain.showMessageNotification('Kimsin ?');
        //    return;
        //}

        auctionHub.server
            .placeBid($('#prod-value-bid').val(), currentCustomer)
            .done(function () {
                appMain.showMessageNotification('Arttırma gerçekleştirildi');
            });

        //auctionHub.server
        //    .reset().done(function () {
        //        appMain.showMessageNotification('Açık Arttırma Sıfırlandı');
        //    });
    });

    $("#btn-reset").click(function (e) {
        $.connection.hub.stop();
        auctionHub.server.resetCurrentAuction(auctionModel).done(function () {
            alert(1);
        });

        e.preventDefault();
    });

    // Start connection
   
});