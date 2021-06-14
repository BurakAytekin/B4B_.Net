using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Users : DataAccess
    {
        #region Properties

        public AuthorityUser AuthorityUser { get; set; }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string RuleCode { get; set; }
        public bool Type { get; set; }
        public string Tel { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Gsm { get; set; }
        public string Mail { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public string AvatarPath { get { return (String.IsNullOrEmpty(Avatar) ? "/Content/images/avatar/noavatar.png" : "/Content/images/avatar/" + Avatar); } }
        public string Avatar { get; set; }
        public string AuthenticatorGuid { get; set; }
        public bool IsAuthenticator { get; set; }
        #endregion

        #region Methods
        public static List<Users> GetUserListByCustomerId(int customerId)
        {
            List<Users> list = new List<Users>();
            DataTable dt = DAL.GetUserListByCustomerId(customerId);

            foreach (DataRow row in dt.Rows)
            {
                Users users = new Users()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    City = row.Field<string>("City"),
                    Tel = row.Field<string>("Tel"),
                    Address = row.Field<string>("Address"),
                    RuleCode = row.Field<string>("RuleCode"),
                    Gsm = row.Field<string>("Gsm"),
                    Mail = row.Field<string>("Mail"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Latitude = row.Field<string>("Latitude"),
                    Longitude = row.Field<string>("Longitude"),
                    Rate = row.Field<double>("Rate"),
                    Avatar = row.Field<string>("Avatar"),
                    Password = row.Field<string>("Password"),
                    Status = row.Field<bool>("Status"),
                    IsAuthenticator = row.Field<bool>("IsAuthenticator"),
                    AuthenticatorGuid = row.Field<string>("AuthenticatorGuid"),
                    Type=row.Field<bool>("Type"),
                    AuthorityUser = new AuthorityUser()
                    {
                        Id = row.Field<int>("AuthorityUserId"),
                        UserId = row.Field<int>("Id"),
                        _Android = row.Field<bool>("_Android"),
                        _B2b = row.Field<bool>("_B2b"),
                        _CampaignStatu = row.Field<bool>("_CampaignStatu"),
                        _Ios = row.Field<bool>("_Ios"),
                        _LockUser = row.Field<bool>("_LockUser"),
                        _ProductRestoration = row.Field<bool>("_ProductRestoration"),
                        _RuleActive = row.Field<bool>("_RuleActive"),
                        _ShowQuantity = row.Field<bool>("_ShowQuantity")
                    }

                };
                list.Add(users);
            }

            return list;
        }

        public static Users GetById(int id)
        {
            Users list = new Users();
            DataTable dt = DAL.GetUserById(id);

            foreach (DataRow row in dt.Rows)
            {
                Users users = new Users()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    City = row.Field<string>("City"),
                    Tel = row.Field<string>("Tel"),
                    RuleCode = row.Field<string>("RuleCode"),
                    Gsm = row.Field<string>("Gsm"),
                    Mail = row.Field<string>("Mail"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Latitude = row.Field<string>("Latitude"),
                    Longitude = row.Field<string>("Longitude"),
                    Rate = row.Field<double>("Rate"),
                    Avatar = row.Field<string>("Avatar"),
                    Password = row.Field<string>("Password"),
                    Status = row.Field<bool>("Status"),
                    IsAuthenticator = row.Field<bool>("IsAuthenticator"),
                    AuthenticatorGuid = row.Field<string>("AuthenticatorGuid"),
                    AuthorityUser = new AuthorityUser()
                    {
                        Id = row.Field<int>("AuthorityUserId"),
                        UserId = row.Field<int>("Id"),
                        _Android = row.Field<bool>("_Android"),
                        _B2b = row.Field<bool>("_B2b"),
                        _CampaignStatu = row.Field<bool>("_CampaignStatu"),
                        _Ios = row.Field<bool>("_Ios"),
                        _LockUser = row.Field<bool>("_LockUser"),
                        _ProductRestoration = row.Field<bool>("_ProductRestoration"),
                        _RuleActive = row.Field<bool>("_RuleActive"),
                        _ShowQuantity = row.Field<bool>("_ShowQuantity")
                    }

                };
                list = (users);
            }

            return list;
        }

        public bool Update()
        {
            return DAL.UpdateUser(Id, Code, Name, Password, RuleCode, Type, Tel, City, Gsm, Mail, Rate, Status, Latitude, Longitude, IsAuthenticator, Address);
        }

        public bool UpdatePassword()
        {
            return DAL.UpdatePassword(Id, Password);
        }

        public bool Add()
        {
            return DAL.AddUser(CustomerId, Code, Name, Password, RuleCode, Type, Tel, City, Gsm, Mail, Rate, Status,
                Latitude, Longitude, CreateId, Address);
        }

        public bool UpdateAuthenticatorValue()
        {
            return DAL.UpdateUserAuthenticatorValue(Id, AuthenticatorGuid, EditId);
        }

        #endregion
    }
    public partial class DataAccessLayer
    {
        public bool UpdateUserAuthenticatorValue(int pId, string pAuthenticatorGuid, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_UpdateUserAuthenticatorGuid", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pAuthenticatorGuid, pEditId });
        }

        public bool UpdatePassword(int pId, string pPassword)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_UserPassword", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pPassword });

        }

        public bool UpdateUser(int pId, string pCode, string pName, string pPassword, string pRuleCode, bool pType, string pTel, string pCity, string pGsm, string pMail, double pRate, bool pStatus, string pLatitude, string pLongitude, bool pIsAuthenticator, string pAddress)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_UserDetail2", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCode, pName, pPassword, pRuleCode, pType, pTel, pCity, pGsm, pMail, pRate, pStatus, pLatitude, pLongitude, pIsAuthenticator , pAddress });

        }
        public bool AddUser(int pCustomerId, string pCode, string pName, string pPassword, string pRuleCode, bool pType, string pTel, string pCity, string pGsm, string pMail, double pRate, bool pStatus, string pLatitude, string pLongitude, int pCreateId, string pAddress)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_User2", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pCode, pName, pPassword, pRuleCode, pType, pTel, pCity, pGsm, pMail, pRate, pStatus, pLatitude, pLongitude, pCreateId , pAddress });

        }

        public DataTable GetUserListByCustomerId(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_UserListByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }

        public DataTable GetUserById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_UserById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

    }
}
