using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Messages : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int GroupId { get; set; }
        public bool Checked { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string SalesmanId { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public new DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public int SenderId { get; set; }
        public int ReadId { get; set; }
        public bool ReadStatu { get; set; }
        public bool DeletedByRead { get; set; }
        public bool Important { get; set; }
        public bool Archive { get; set; }
        public bool DeletedBySender { get; set; }
        public MessageSentertype Type { get; set; }
        public LoginType AddType { get; set; }

        #endregion

        #region Methods


        public static List<Messages> GetMessageList(int customerId,int userId,int type,int addType,int salesmanId, MessageBoxStatu messagebox,string searchText)
        {
            DataTable dt = DAL.GetMessageList(customerId,userId,type,addType,salesmanId,(int)messagebox, searchText);
            List<Messages> list = new List<Messages>();

            foreach (DataRow row in dt.Rows)
            {
                Messages item = new Messages();
                {
                    item.Id = row.Field<int>("Id");
                    item.Header = row.Field<string>("Header");
                    item.Content = row.Field<string>("Content");
                    item.CreateDate = row.Field<DateTime>("CreateDate");
                    item.ReadDate = row["ReadDate"] as DateTime?;
                    item.ReadStatu = row.Field<bool>("ReadStatu");
                    item.Important = row.Field<bool>("Important");
                    
                
                };
                list.Add(item);
            }

            return list;
        }


        public bool Add()
        {
           return DAL.InsertMessage(CustomerId, UserId, SalesmanId, GroupId, Header, Content, SenderId, (int)Type,(int)AddType);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public bool InsertMessage(int pCustomerId, int pUserId, string pSalesmanId, int pGroupId, string pHeader, string pContent, int pSenderId, int pType,int pAddType)
        {
          return  DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Message", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pGroupId, pHeader, pContent, pSenderId, pType, pAddType });
        }

        public DataTable GetMessageList(int pCustomerId, int pUserId, int pType,int pAddType,int pSalesmanId,int pMessagebox,string pSearchText)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_MessageList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId,pUserId,pType,pAddType,pSalesmanId,pMessagebox, pSearchText });
        }

    }

    public enum MessageBoxStatu
    {
        Inbox = 0,
        SendBox = 1,
        Important = 2,
        Archive = 3,
        Deleted = 4
    }

    public enum MessageSentertype
    {
        Customer = 0 ,
        Salesman = 1
    }
}