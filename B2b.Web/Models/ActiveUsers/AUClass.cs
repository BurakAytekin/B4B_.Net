using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ActiveUsers
{
    public class AUClass
    {
        public AUClass()
        {
            Id = "-1";
            UserName = string.Empty;
            IsActive = UserState.Disconnected;
        }
        #region Properties
        public string Id { get; set; }
        public string UserName { get; set; }
        public UserState IsActive { get; set; }
        public int TerminalNo { get; set; }
        public string UserId { get; set; }
        public LoginType LoginType { get; set; }
        #endregion

    }

    public enum UserState
    {
        Connected,
        Connecting,
        Disconnected,
        Disconnecting,
        Reset
    }
}