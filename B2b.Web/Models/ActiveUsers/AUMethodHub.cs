using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2b.Web.v4.Models.ActiveUsers
{
    [HubName("EryazActiveUsers")]
    public class AUMethodHub : Hub
    {
        #region Properties
        private readonly static ConnectionMapping<string> _connectingUsers = new ConnectionMapping<string>();
        private readonly AUMethod _auMethod;
        #endregion
        #region Constructor
        public AUMethodHub() : this(AUMethod.Instance) { }
        public AUMethodHub(AUMethod pAuMethod)
        {
            _auMethod = pAuMethod;
        }
        #endregion
        #region Methods
        public List<KeyValuePair<string, HashSet<string>>> GetActiveUserList()
        {
            var vResult = _connectingUsers.GetConnections();
            return vResult;
        }



        public void DisconnectUser(string pName, string pContextId)
        {
            Clients.Client(pContextId).disconnectFromAdmin(pName);
        }
        public override Task OnConnected()
        {

            _connectingUsers.Add(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {

            _connectingUsers.Remove(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {

            if (Context.User != null && !_connectingUsers.GetConnections(Context.User.Identity.Name).Contains(Context.ConnectionId))
            {
                _connectingUsers.Add(Context.User.Identity.Name, Context.ConnectionId);
            }
            return base.OnReconnected();
        }
        #endregion
    }
}