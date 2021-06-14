using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Announcements : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProductId { get; set; }
        public int AnnouncementsType { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MinOrderNumber { get; set; }
        public string Unit { get; set; }
        public string PicturePath { get; set; }
        public string PicturePathShow { get { return (String.IsNullOrEmpty(PicturePath) ? "#" : GlobalSettings.FtpServerAddressFull + PicturePath); } }
        public string Query { get; set; }
        public string PriceStr { get; set; }
        public Salesman Salesman { get; set; }
        public double Priority { get; set; }
        public string ImageBase { get; set; }
        public bool Checked { get; set; }
        #endregion

        #region Methods
        public static List<Announcements> GetAllByType(AnnouncementsType pAnnouncamentsType)
        {
            List<Announcements> list = new List<Announcements>();
            DataTable dt = DAL.GetAnnouncementsByAllAnnouncamentsType((int)pAnnouncamentsType);

            foreach (DataRow row in dt.Rows)
            {
                Announcements obj = new Announcements()
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    PicturePath = row.Field<string>("PicturePath"),
                    Query = row.Field<string>("Query"),
                    ProductId = row.Field<int>("ProductId"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    PriceStr = row.Field<string>("PriceStr"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("CreateId"),
                        Code = row.Field<string>("SalesmanCode"),
                        Name = row.Field<string>("SalesmanName"),
                        PicturePath = row.Field<string>("SalesmanPicturePath") == string.Empty ? GlobalSettings.B2bAddress + "Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("SalesmanPicturePath"),
                    }
                };
                list.Add(obj);
            }

            return list;
        }
        public static List<Announcements> GetAnnouncementHeaderList(int type, int isActive)
        {
            List<Announcements> list = new List<Announcements>();
            DataTable dt = DAL.GetAnnouncementHeaderList(type, isActive);

            foreach (DataRow row in dt.Rows)
            {
                Announcements announcement = new Announcements
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Priority = row.Field<double>("Priority"),
                    Content = row.Field<string>("Content"),
                    PicturePath = row.Field<string>("PicturePath"),

                    Query = row.Field<string>("Query"),
                    ProductId = row.Field<int>("ProductId"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    PriceStr = row.Field<string>("PriceStr"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    EndDate = row.Field<DateTime>("EndDate"),
                    IsActive = Convert.ToBoolean(row["IsActive"])


                };

                list.Add(announcement);
            }
            return list;
        }

        public bool Add()
        {
            return DAL.InsertAnnouncement(Header, Content, StartDate, EndDate, ProductId, AnnouncementsType, IsActive, PicturePath, Query, PriceStr, CreateId);
        }

        public static bool Delete(string DeleteIds, int DeleteId)
        {
            return DAL.DeleteAnnouncements(DeleteIds, DeleteId);
        }


        public static bool ChangePriority(string ids, int editId)
        {
            return DAL.ChangeAnnouncementPriority(ids, editId);
        }
        public static bool UpdateAnnouncements(Announcements announcement)
        {
            return DAL.UpdateAnnouncements(announcement.Id, announcement.Header, announcement.Content, announcement.PicturePath, announcement.Query, announcement.ProductId, announcement.PriceStr, announcement.StartDate, announcement.EndDate, announcement.IsActive, announcement.Deleted, announcement.EditId);
        }

        #endregion
    }

    public partial class DataAccessLayer
    {
        public bool InsertAnnouncement(string pHeader, string pContent, DateTime pStartDate, DateTime pEndDate, int pProductId, int pAnnouncementsType, bool pIsActive, string pPicturePath, string pQuery, string pPriceStr, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Announcement", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeader, pContent, pStartDate, pEndDate, pProductId, pAnnouncementsType, pIsActive, pPicturePath, pQuery, pPriceStr, pCreateId });
        }

        public DataTable GetAnnouncementsByAllAnnouncamentsType(int pAnnouncamentsType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Announcements_GetListPictureByAllAnnouncamentsType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pAnnouncamentsType });
        }
        public DataTable GetAnnouncementHeaderList(int pType, int pIsActive)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_AnnouncementHeader_ByType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pIsActive });
        }
        public bool ChangeAnnouncementPriority(string pIds, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Change_Announcement_Priority", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pIds, pEditId });
        }
        public bool UpdateAnnouncements(int pId, string pHeader, string pContent, string pPicturePath, string pQuery, int pProductId, string pPriceStr, DateTime pStartDate, DateTime pEndDate, bool pIsActive, bool pDeleted, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Announcements", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pHeader, pContent, pPicturePath, pQuery, pProductId, pPriceStr, pStartDate, pEndDate, pIsActive, pDeleted, pEditId });
        }

        public bool DeleteAnnouncements(string pDeleteIds, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Announcements", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pDeleteIds, pEditId });
        }
    }



}