using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class OemBlackList : DataAccess
    {
        #region Constructors

        public OemBlackList()
        {
            ProductId = -1;
            BrandName = string.Empty;
        }
        #endregion

        #region Properties

        public int Id { get; set; }
        public int OemId { get; set; }
        public Oem Oem { get; set; }
        public string OemNo { get; set; }
        public string Note { get; set; }
        public BlackListType Type { get; set; }
        public int AddType { get; set; }
        public bool IsConfirm { get; set; }
        public string BrandName { get; set; }
        public int ProductId { get; set; }
        public string OldValue { get; set; }
        public Product Product { get; set; }
        public Customer Customer { get; set; }
        public Salesman Salesman { get; set; }
        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertOemBlackList(OemId, OemNo, Note, CreateId, (int)Type, AddType, ProductId, BrandName);
        }

        public bool Update()
        {
            return DAL.UpdateOemBlackList(Id, EditId, IsConfirm);
        }
        public bool Delete()
        {
            return DAL.DeleteOemBlackList(Id, EditId);
        }

        public static List<OemBlackList> GetOemBlackListByType(int type, int oemType)
        {
            List<OemBlackList> list = new List<OemBlackList>();
            DataTable dt = DAL.GetOemBlackListByType(type, oemType);

            foreach (DataRow row in dt.Rows)
            {
                OemBlackList item = new OemBlackList()
                {
                    Id = row.Field<int>("Id"),
                    OemNo = row.Field<string>("OemNo"),
                    OemId = row.Field<int>("OemId"),
                    Note = row.Field<string>("Note"),
                    Type = (BlackListType)row.Field<int>("Type"),
                    AddType = row.Field<int>("AddType"),
                    ProductId = row.Field<int>("ProductId"),
                    OldValue = row.Field<string>("OldValue"),
                    Product = new Product()
                    {
                        Code = row.Field<string>("ProductCode"),
                        Name = row.Field<string>("ProductName"),
                        Manufacturer = row.Field<string>("Manufacturer"),
                        Id = row.Field<int>("ProductId")
                    },
                    Customer = new Customer()
                    {
                        Id = Convert.ToInt32(row["CustomerId"]),
                        Code = row.Field<string>("CustomerCode"),
                        Name = row.Field<string>("CustomerName"),
                        Users = new Users()
                        {
                            Id = row.Field<int>("CreateId"),
                            Code = row.Field<string>("UserCode"),
                            Name = row.Field<string>("UserName")
                        }
                    },
                    Salesman = new Salesman()
                    {
                        Id = Convert.ToInt32(row["SalesmanId"]),
                        Code = row.Field<string>("SalesmanCode"),
                        Name = row.Field<string>("SalesmanName")
                    },
                    Oem = new Oem()
                    {
                        Id = row.Field<int>("OemId"),
                        OemNo = row.Field<string>("OldValue"),
                        Brand = row.Field<string>("Brand"),
                        Type = Convert.ToInt32(row["OemType"])
                    },
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    CreateId = row.Field<int>("CreateId")

                };
                list.Add(item);
            }

            return list;
        }

        #endregion


    }

    public enum BlackListType
    {
        BlackList = 0,
        Edit = 1,
        OemInsert = 2,
        RivalInsert = 3
    }


    public partial class DataAccessLayer
    {
        public DataTable GetOemBlackListByType(int pType, int pOemType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_OemBlackListByType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pOemType });
        }

        public bool InsertOemBlackList(int pOemId, string pOemNo, string pNote, int pCreateId, int pType, int pAddType, int pProductId, string pBrandName)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_OemBlackList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOemId, pOemNo, pNote, pCreateId, pType, pAddType, pProductId, pBrandName });
        }

        public bool UpdateOemBlackList(int pId, int pEditId, bool pIsConfirm)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_OemBlackList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId, pIsConfirm });
        }

        public bool DeleteOemBlackList(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_OemBlackList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

    }
}