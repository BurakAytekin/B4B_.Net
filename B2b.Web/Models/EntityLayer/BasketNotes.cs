using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class BasketNotes : DataAccess
    {
        public BasketNotes()
        {
        }

        #region Properties

        public int Id { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public new DateTime CreateDate { get; set; }
        public string Note { get; set; }
        public string Header { get; set; }

        #endregion

        #region Methods

        public bool Add()
        {
           return DAL.InsertBasketNotes(CustomerId,UserId,SalesmanId,Header,Note);
        }

        public bool Update()
        {
          return  DAL.UpdateBasketNotes(Id, Header, Note,EditId,Deleted);
        }

        public static List<BasketNotes> GetBasketNoteList(int customerId, int userId, int salesmanId)
        {
            List<BasketNotes> list = new List<BasketNotes>();
            DataTable dt = DAL.GetBasketNoteList(customerId, userId, salesmanId);

            foreach (DataRow row in dt.Rows)
            {
                BasketNotes item = new BasketNotes()
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Note = row.Field<string>("Note"),
                    
                };
                list.Add(item);
            }

            return list;
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetBasketNoteList( int pCustomerId, int pUserId, int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BasketNotes", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId });
        }

        public bool InsertBasketNotes(int pCustomerId, int pUserId, int pSalesmanId, string pHeader, string pNote)
        {
           return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_BasketNotes", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pHeader, pNote });
        }

        public bool UpdateBasketNotes(int pId, string pHeader, string pNote,int pEditId,bool pDeleted)
        {
          return  DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_BasketNotes", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pHeader, pNote, pEditId , pDeleted });
        }
    }
}