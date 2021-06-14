using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class CollectingHeader : DataAccess
    {
        #region Properties
        public int Id { get; set; }

        public double Amount { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Currency { get; set; }

        public Price PriceTotal { get { return new Price(Amount, Currency, 1); } }
        public string PriceTotalStr
        {
            get { return PriceTotal.ToString(); }
        }

        public string StatusStr { get; set; }

        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public string DocumentNo { get; set; }
        public new DateTime CreateDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public int Status { get; set; }
        public byte[] Pdf { get; set; }

        public List<Collecting> CollectingList { get; set; }
        #endregion
        public void Save()
        {
            Status = 0;
            DataTable dt = DAL.InsertCollectingHeader(CustomerId, SalesmanId, DocumentNo, Status, UserId);
            if (dt.Rows.Count > 0)
            {
                Id = dt.Rows[0].Field<int>(0);
                DocumentNo = dt.Rows[0].Field<string>(1);
            }
            else
            {
                Id = -99;
                DocumentNo = "Başlık Oluşturma Hatası";
            }

        }


        public bool Update()
        {
           return DAL.UpdateCollectingHeader(Id);
        }

        public bool Delete()
        {
            return DAL.DeleteCollectingHeader(Id);
        }




        public static CollectingHeader GetById(int id)
        {
            CollectingHeader obj = null;
            DataTable dt = DAL.GetListCollectingHeaderById(id);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                obj = new CollectingHeader()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),

                    UserId = row.Field<int>("UserId"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    DocumentNo = row.Field<string>("DocumentNo"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    ConfirmDate = row.Field<DateTime?>("ConfirmDate"),
                    Status = row.Field<int>("Status"),
                };
                obj.GetCollectingList();
            }

            return obj;
        }


        public static List<CollectingHeader> GetCollectingHeaderList(DateTime startDate, DateTime finishDate, int status)
        {
            List<CollectingHeader> obj = new List<CollectingHeader>();
            DataTable dt = DAL.GetCollectingHeaderList(startDate, finishDate, status);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                CollectingHeader c = new CollectingHeader()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    CustomerName = row.Field<string>("Name"),
                    UserId = row.Field<int>("UserId"),
                    Amount = row.Field<double>("Amount"),
                    Currency = row.Field<string>("Currency"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    DocumentNo = row.Field<string>("DocumentNo"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    ConfirmDate = row.Field<DateTime?>("ConfirmDate"),
                    Status = row.Field<int>("Status"),

                };
                c.StatusStr = c.Status == 0 ? @"<span class=""label bg-cyan"">Beklemede</span>" : @"<span class=""label bg-greensea"">Onaylandı</span>";
                obj.Add(c);
            }
            return obj;
        }



        public void GetCollectingList()
        {
            CollectingList = Collecting.GetListByHeaderId(Id);
        }
    }
    public partial class DataAccessLayer
    {
        public DataTable InsertCollectingHeader(int pCustomerId, int pSalesmanId, string pDocumentNo, int pStatus, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Insert_CollectingHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pSalesmanId, pDocumentNo, pStatus, pUserId });
        }
        public bool UpdateCollectingHeader(int pHeaderId)
        {
          return  DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_CollectingHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId });
        }
        public bool DeleteCollectingHeader(int pHeaderId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_CollectingHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId });
        }
        public DataTable GetListCollectingHeaderById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_CollectingHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
        public DataTable GetCollectingHeaderList(DateTime pStartDate, DateTime pFinishDate, int pStatus)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_CollectingHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartDate, pFinishDate, pStatus });
        }

    }
}