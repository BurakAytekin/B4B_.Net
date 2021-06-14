using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Picture:DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int PictureId { get; set; }
        public bool IsDefault { get; set; }
        public Product Product { get; set; }

        #endregion

        #region Methods

        public static List<Picture> GetPicturePathByProductId(int pProductId)
        {
            DataTable dt = DAL.GetPicturePathByProductId(pProductId);
            List<Picture> list = new List<Picture>();

            foreach (DataRow row in dt.Rows)
            {
                Picture item = new Picture();
                {
                    item.Id = row.Field<int>("Id");
                    item.IsDefault = row.Field<bool>("IsDefault");
                    item.Path = GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");

                };
                list.Add(item);
            }

            return list;
        }

        public static int GetPictureMaxId(int pProductId)
        {
            DataTable dt = DAL.GetPictureMaxId(pProductId);

            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0][0]);
            else
                return 0;
            
        }


        public bool Add()
        {
            return DAL.InsertPicture(Path,ProductId, ProductCode, PictureId,CreateId);
        }

        public bool Update()
        {
            return DAL.UpdatePicture(Id, Deleted, IsDefault, EditId);
        }


        #endregion

    }

    public partial class DataAccessLayer
    {
        public bool UpdatePicture(int pId, bool pDeleted, bool pIsDefault, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Picture", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pDeleted, pIsDefault, pEditId });
        }

        public bool InsertPicture(string pPath,int pProductId,string pProductCode,int pPictureId, int pCreateId)
        {
           return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Picture", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pPath, pProductId, pProductCode, pPictureId, pCreateId });
        }

        public DataTable GetPicturePathByProductId(int pProductId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_PicturePathByProductId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId });
        }

        public DataTable GetPictureMaxId(int pProductId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_PictureMaxId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId });
        }

    }
}