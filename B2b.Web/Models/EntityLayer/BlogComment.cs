using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class BlogComment : DataAccess
    {
        public BlogComment()
        {

        }

        #region Properties

        public int Id { get; set; }
        public int BlogId { get; set; }
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public string UserName { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public bool IsApproval { get; set; }
        public Salesman Salesman { get; set; }
        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertComment(BlogId, CustomerId, UserId, SalesmanId, Header, Content, IsApproval);
        }

        public bool Delete()
        {
            return DAL.DeleteComment(Id,EditId);
        }
        public bool Update()
        {
            return DAL.UpdateComment(Id, IsApproval);
        }

        public static List<BlogComment> GetBlogCommentList(int customerId, int userId, int blogId)
        {
            List<BlogComment> list = new List<BlogComment>();
            DataTable dt = DAL.GetBlogCommentList(customerId, userId, blogId);

            foreach (DataRow row in dt.Rows)
            {
                BlogComment item = new BlogComment()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    UserId = row.Field<int>("UserId"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    UserName = row.Field<string>("UserName"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    IsApproval = row.Field<bool>("IsApproval"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("SalesmanCode"),
                        Name = row.Field<string>("SalesmanName"),
                        Avatar = row.Field<string>("SalesmanAvatar")
                    },
                    Customer = new Customer()
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Users = new Users()
                        {
                            Id = row.Field<int>("UserId"),
                            Code = row.Field<string>("UserCode"),
                            Name = row.Field<string>("UserName"),
                            Avatar = row.Field<string>("Avatar")
                        }
                    },

                };
                list.Add(item);
            }

            return list;
        }


        public static List<BlogComment> GetBlogCommentListById(int id)
        {
            List<BlogComment> list = new List<BlogComment>();
            DataTable dt = DAL.GetBlogCommentListById(id);

            foreach (DataRow row in dt.Rows)
            {
                BlogComment item = new BlogComment()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    UserId = row.Field<int>("UserId"),
                    BlogId = id,
                    SalesmanId = row.Field<int>("SalesmanId"),
                    UserName = row.Field<string>("UserName"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    IsApproval = row.Field<bool>("IsApproval"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("SalesmanCode"),
                        Name = row.Field<string>("SalesmanName"),
                        Avatar = row.Field<string>("SalesmanAvatar")
                    },
                    Customer = new Customer()
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Users = new Users()
                        {
                            Id = row.Field<int>("UserId"),
                            Code = row.Field<string>("UserCode"),
                            Name = row.Field<string>("UserName"),
                            Avatar = row.Field<string>("Avatar")
                        }
                    },

                };
                list.Add(item);
            }

            return list;
        }

        public static List<BlogComment> GetBlogCommentListForApprove()
        {
            List<BlogComment> list = new List<BlogComment>();
            DataTable dt = DAL.GetBlogCommentListForApprove();

            foreach (DataRow row in dt.Rows)
            {
                BlogComment item = new BlogComment()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    UserId = row.Field<int>("UserId"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    UserName = row.Field<string>("UserName"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    IsApproval = row.Field<bool>("IsApproval"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("SalesmanCode"),
                        Name = row.Field<string>("SalesmanName"),
                        Avatar = row.Field<string>("SalesmanAvatar")
                    },
                    Customer = new Customer()
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Users = new Users()
                        {
                            Id = row.Field<int>("UserId"),
                            Code = row.Field<string>("UserCode"),
                            Name = row.Field<string>("UserName"),
                            Avatar = row.Field<string>("Avatar")
                        }
                    },

                };
                list.Add(item);
            }

            return list;
        }

        public static List<BlogComment> GetBlogCommentListByBanner()
        {
            List<BlogComment> list = new List<BlogComment>();
            DataTable dt = DAL.GetBlogCommentListByBanner();

            foreach (DataRow row in dt.Rows)
            {
                BlogComment item = new BlogComment()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    UserId = row.Field<int>("UserId"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    UserName = row.Field<string>("UserName"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    IsApproval = row.Field<bool>("IsApproval"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Customer = new Customer()
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Users = new Users()
                        {
                            Id = row.Field<int>("UserId"),
                            Code = row.Field<string>("UserCode"),
                            Name = row.Field<string>("UserName"),
                            Avatar = row.Field<string>("Avatar")
                        }
                    },

                };
                list.Add(item);
            }

            return list;
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetBlogCommentList(int pCustomerId, int pUserId, int pBlogId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BlogComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pBlogId });
        }


        public DataTable GetBlogCommentListById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_BlogComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public DataTable GetBlogCommentListForApprove()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_BlogCommentForApprove", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable GetBlogCommentListByBanner()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BlogCommentByBanner", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public bool InsertComment(int pBlogId, int pCustomerId, int pUserId, int pSalesmanId, string pHeader, string pContent, bool pIsApproval)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_BlogComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pBlogId, pCustomerId, pUserId, pSalesmanId, pHeader, pContent, pIsApproval });

        }

        public bool DeleteComment(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_BlogComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });

        }

        public bool UpdateComment(int pId, bool pIsApproval)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_BlogComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pIsApproval });

        }

    }
}