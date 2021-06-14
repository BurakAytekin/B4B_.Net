using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Files : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string PicturePath { get; set; }
        public string FileType { get; set; }
        public bool Remote { get; set; }
        public bool Checked { get; set; }
        public int Restriction { get; set; }
        #endregion

        #region Methods


        public static List<Files> GetFileList()
        {
            List<Files> list = new List<Files>();
            DataTable dt = DAL.GetFileList();

            foreach (DataRow row in dt.Rows)
            {
                Files obj = new Files()
                {
                    Id = row.Field<int>("Id"),
                    Title = row.Field<string>("Title"),
                    Name = row.Field<string>("Name"),
                    Path = GlobalSettings.FtpServerAddressFull +  row.Field<string>("Path"),
                    PicturePath = row.Field<bool>("Remote") ?  GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath") : "../../"+ row.Field<string>("PicturePath"),
                    FileType = row.Field<string>("FileType")
                };
                list.Add(obj);
            }
            return list;
        }
        public static List<Files> GetFileListType()
        {
            List<Files> list = new List<Files>();
            DataTable dt = DAL.GetFileListType();

            foreach (DataRow row in dt.Rows)
            {
                Files obj = new Files()
                {
                   
                    FileType = row.Field<string>("FileType")
                };
                list.Add(obj);
            }
            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteFile(Id,EditId);
        }

        

        public bool Add()
        {
            return DAL.InsertFile(Title, Name,Path,PicturePath,FileType, Remote, Restriction, CreateId);
        }

        #endregion


    }

    public partial class DataAccessLayer
    {
        public bool InsertFile(string pTitle, string pName, string pPath,string pPicturePath,string pFileType, bool pRemote,int pRestriction, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_File", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTitle, pName, pPath, pPicturePath, pFileType, pRemote , pRestriction , pCreateId });
        }

        public bool DeleteFile(int pId,int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_File", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public DataTable GetFileList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Files");
        }
        public DataTable GetFileListType()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_File_Type");
        }
    }

}