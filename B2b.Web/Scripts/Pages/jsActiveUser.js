var vEryazActiveUser;

var vUsers = new Array();
$(document).ready(function () {

    vEryazActiveUser = $.connection.EryazActiveUsers;
    if ($.connection.EryazActiveUsers !== undefined) {
        vEryazActiveUser.client.onlineUsers = function (onlineUsers) {
            vUsers = new Array();
            $.each(onlineUsers, function () {
                var vUser = new Array();
                vUser.push(this["Key"]);
                vUser.push(this["Value"][0]);
                vUsers.push(vUser);
                //$('#listOnlineClients').append('<option value="' + this["Value"][0] + '">' + this["Key"] + '</option>');
            });
        }

        vEryazActiveUser.client.disconnectFromAdmin = function (value) {
            $.connection.hub.stop();
            $.colorbox({
                html: '<div style="width:250px;text-align:center;font-weight:bold;">Oturumunuz sonlandırılmıştır !</div>',
                onClosed: function () {
                    $.ajax({
                        type: "POST",
                        url: "Default.aspx/DeleteAllSessions",
                        data: "{}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: true,
                        success: function (data) {
                            window.location.href = 'Logout.aspx';
                        },
                        error: function () { }

                    });
                }
            });
        }

        $.connection.hub.stateChanged(function (state) {
            // Transitioning from connecting to connected
            if (state.oldState === $.signalR.connectionState.connecting && state.newState === $.signalR.connectionState.connected) {
                // Start sending
            }
        });
        $.connection.hub.start(); //{ 'UserId': $('#hfUserId').val(), 'Name': $('#hfUserName').val() }

    }




});

function disconnectActiveUser(pName, pContextId) {
    vEryazActiveUser.server.disconnectUser(pName, pContextId);
}
