using System;
namespace B2b.Web.v4.Models.ActiveUsers
{
    public class AUMethod
    {
        #region Properties

        private readonly static Lazy<AUMethod> _instance = new Lazy<AUMethod>(() => new AUMethod());
        //private readonly static object _userStateLock = new object();
        //private readonly ConcurrentDictionary<string, AUClass> _aU = new ConcurrentDictionary<string, AUClass>();
        //private readonly List<AUClass> _onlineUsers = new List<AUClass>();
        //private readonly Lazy<HubConnectionContext> _clientInstance = new Lazy<HubConnectionContext>(() => (HubConnectionContext) GlobalHost.ConnectionManager.GetHubContext<AUMethodHub>().Clients);
        //private EnumClass.UserState _userState = EnumClass.UserState.Disconnected;
        //private static AUClass _aUUser;
        //private bool _updatingAUUser = false;
        //private readonly object _updatingAUUserLock = new object();
        //private Timer _timer;
        public static AUMethod Instance { get { return _instance.Value; } }
        #endregion
        public AUMethod()
        {
        }
    }
}