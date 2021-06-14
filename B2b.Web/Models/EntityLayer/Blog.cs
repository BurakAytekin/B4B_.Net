using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Blog : DataAccess
    {
        public Blog()
        {

        }

        #region Properties
        public int Id { get; set; }
        public string Category { get; set; }
        public int SalesmanId { get; set; }
        public Salesman Salesman { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string ShortContent { get; set; }
        public string Path { get; set; }
        public int Type { get; set; }
        public bool IsShowCommentUser { get; set; }
        public bool ApprovalComment { get; set; }
        public int LikeCount { get; set; }
        public int SeeCount { get; set; }
        public int CommentCount { get; set; }
        public string TypeStr { get { return (Type == 0 ? "Resim" : "Video"); } }
        public string SalesmanName { get { return Salesman.Name; } }
        public int DateDay { get { return CreateDate.Day; } }
        public string DateMonth { get { return CreateDate.ToString("MMM"); } }
        public bool IsLockComment { get; set; }
        #endregion

        #region Methods

        public static List<Blog> GetBlogList(string pCategry, int pStart, int pList)
        {
            List<Blog> list = new List<Blog>();
            DataTable dt = DAL.GetBlogList(pCategry, pStart, pList);

            foreach (DataRow row in dt.Rows)
            {
                Blog item = new Blog()
                {
                    Id = row.Field<int>("Id"),
                    Category = row.Field<string>("Category"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        PicturePath = row.Field<string>("PicturePath")
                    },
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    ShortContent = row.Field<string>("ShortContent"),
                    IsLockComment = row.Field<bool>("IsLockComment"),
                    Path = row.Field<int>("Type") == 0 ? GlobalSettings.FtpServerAddressFull + row.Field<string>("Path") : row.Field<string>("Path"),
                    Type = row.Field<int>("Type"),
                    IsShowCommentUser = row.Field<bool>("IsShowCommentUser"),
                    ApprovalComment = row.Field<bool>("ApprovalComment"),
                    LikeCount = row.Field<int>("LikeCount"),
                    SeeCount = row.Field<int>("SeeCount"),
                    CreateDate = row.Field<DateTime>("CreateDate")

                };
                list.Add(item);
            }

            return list;
        }

        public static List<Blog> GetBlogListByAll()
        {
            List<Blog> list = new List<Blog>();
            DataTable dt = DAL.GetBlogListByAll();

            foreach (DataRow row in dt.Rows)
            {
                Blog item = new Blog()
                {
                    Id = row.Field<int>("Id"),
                    Category = row.Field<string>("Category"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        PicturePath = row.Field<string>("PicturePath")
                    },
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    ShortContent = row.Field<string>("ShortContent"),
                    IsLockComment = row.Field<bool>("IsLockComment"),
                    Path = row.Field<int>("Type") == 0 ? GlobalSettings.FtpServerAddressFull + row.Field<string>("Path") : row.Field<string>("Path"),
                    Type = row.Field<int>("Type"),
                    IsShowCommentUser = row.Field<bool>("IsShowCommentUser"),
                    ApprovalComment = row.Field<bool>("ApprovalComment"),
                    LikeCount = row.Field<int>("LikeCount"),
                    SeeCount = row.Field<int>("SeeCount"),
                    CreateDate = row.Field<DateTime>("CreateDate")

                };
                list.Add(item);
            }

            return list;
        }

        public static List<Blog> GetBlogCategoryList()
        {
            List<Blog> list = new List<Blog>();
            DataTable dt = DAL.GetBlogCategoryList();

            foreach (DataRow row in dt.Rows)
            {
                Blog item = new Blog()
                {
                    Category = row.Field<string>("Category"),
                };
                list.Add(item);
            }

            return list;
        }

        public bool Add()
        {
            return DAL.InsertBlog(Category, SalesmanId, Header, Content, Path, Type, IsShowCommentUser, ApprovalComment,ShortContent,IsLockComment, CreateId);
        }
        public bool Update()
        {
            return DAL.UpdateBlog(Id, Category, SalesmanId, Header, Content, Path, Type, IsShowCommentUser, ApprovalComment, ShortContent, IsLockComment, EditId);
        }

        public bool Delete()
        {
            return DAL.DeleteBlog(Id, EditId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public bool InsertBlog(string pCategory, int pSalesmanId, string pHeader, string pContent, string pPath, int pType, bool pIsShowCommentUser, bool pApprovalComment, string pShortContent, bool pIsLockComment, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Blog", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCategory, pSalesmanId, pHeader, pContent, pPath, pType, pIsShowCommentUser, pApprovalComment, pShortContent, pIsLockComment, pCreateId });
        }

        public bool UpdateBlog(int pId, string pCategory, int pSalesmanId, string pHeader, string pContent, string pPath, int pType, bool pIsShowCommentUser, bool pApprovalComment,string pShortContent,bool pIsLockComment , int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Blog", MethodBase.GetCurrentMethod().GetParameters(), new object[] {pId, pCategory, pSalesmanId, pHeader, pContent, pPath, pType, pIsShowCommentUser, pApprovalComment, pShortContent,pIsLockComment, pEditId });
        }

        public bool DeleteBlog(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Blog", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public DataTable GetBlogList(string pCategory, int pStart, int pList)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BlogListByCategory", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCategory, pStart, pList });
        }

        public DataTable GetBlogListByAll()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_BlogListByAll", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable GetBlogCategoryList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BlogCategory", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
    }
}