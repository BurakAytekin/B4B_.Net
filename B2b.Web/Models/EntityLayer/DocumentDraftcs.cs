using B2b.Web.v4.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class DocumentDraftcs : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Header { get; set; }
        public int Type { get; set; }
        public string EmailBody { get; set; }
        public string HeaderBody { get; set; }
        public string ContentBody { get; set; }
        public string FooterBody { get; set; }
        public float HeaderHeight { get; set; }
        public float FooterHeight { get; set; }
        public string EmailAddress { get; set; }
        public bool IsSendEmail { get; set; }
        public bool IsSendSalesman { get; set; }
        public bool IsSendCustomer { get; set; }
        public bool IsPageNumber { get; set; }
        public bool IsDisplayHeader { get; set; }
        public bool IsDisplayFooter { get; set; }
        public bool DefaultData { get; set; }

        #endregion

        #region Methods

        public static DocumentDraftcs GetDocumentDraft(int pType,bool pDefault)
        {
           DocumentDraftcs list = new DocumentDraftcs();
            DataTable dt = DAL.GetDocumentDraft(pType, pDefault);

            foreach (DataRow row in dt.Rows)
            {
                DocumentDraftcs obj = new DocumentDraftcs()
                {
                    Id = row.Field<int>("Id"),
                    Type = row.Field<int>("Type"),
                    Header = row.Field<string>("Header"),
                    EmailBody = row.Field<string>("EmailBody"),
                    HeaderBody = row.Field<string>("HeaderBody"),
                    ContentBody = row.Field<string>("ContentBody"),
                    FooterBody = row.Field<string>("FooterBody"),
                    HeaderHeight = row.Field<float>("HeaderHeight"),
                    FooterHeight = row.Field<float>("FooterHeight"),
                    EmailAddress = row.Field<string>("EmailAddress"),
                    IsSendEmail = row.Field<bool>("IsSendEmail"),
                    IsSendSalesman = row.Field<bool>("IsSendSalesman"),
                    IsSendCustomer = row.Field<bool>("IsSendCustomer"),
                    IsPageNumber = row.Field<bool>("IsPageNumber"),
                    IsDisplayHeader = row.Field<bool>("IsDisplayHeader"),
                    IsDisplayFooter = row.Field<bool>("IsDisplayFooter"),
                    DefaultData = row.Field<bool>("DefaultData"),
                };
                list = obj;
            }
            return list ;
        }

        public bool Add()
        {
            return DAL.InsertDocumentDraft(Header,Type, EmailBody, HeaderBody, ContentBody,FooterBody, HeaderHeight,FooterHeight,EmailAddress,IsSendEmail,IsSendSalesman,IsSendCustomer,IsPageNumber,IsDisplayHeader,IsDisplayFooter,DefaultData, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateDocumentDraft(Id, Header,Type, EmailBody, HeaderBody, ContentBody, FooterBody, HeaderHeight, FooterHeight, EmailAddress, IsSendEmail, IsSendSalesman, IsSendCustomer, IsPageNumber, IsDisplayHeader, IsDisplayFooter, DefaultData, EditId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetDocumentDraft(int pType,bool pDefault)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_DocumentDraftByType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pDefault });
        }

        public bool UpdateDocumentDraft(int pId,string pHeader, int pType, string pEmailBody, string pHeaderBody, string pContentBody, string pFooterBody, float pHeaderHeight, float pFooterHeight, string pEmailAddress, bool pIsSendEmail, bool pIsSendSalesman, bool pIsSendCustomer, bool pIsPageNumber, bool pIsDisplayHeader, bool pIsDisplayFooter, bool pDefaultData, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_DocumentDraft", MethodBase.GetCurrentMethod().GetParameters(), new object[] {pId, pHeader, pType, pEmailBody, pHeaderBody, pContentBody, pFooterBody, pHeaderHeight, pFooterHeight, pEmailAddress, pIsSendEmail, pIsSendSalesman, pIsSendCustomer, pIsPageNumber, pIsDisplayHeader, pIsDisplayFooter, pDefaultData, pEditId });
        }

        public bool InsertDocumentDraft(string pHeader, int pType, string pEmailBody, string pHeaderBody,string pContentBody,string pFooterBody, float pHeaderHeight, float pFooterHeight, string pEmailAddress, bool pIsSendEmail, bool pIsSendSalesman, bool pIsSendCustomer, bool pIsPageNumber, bool pIsDisplayHeader, bool pIsDisplayFooter, bool pDefaultData, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_DocumentDraft", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeader, pType, pEmailBody, pHeaderBody, pContentBody, pFooterBody, pHeaderHeight, pFooterHeight, pEmailAddress, pIsSendEmail, pIsSendSalesman, pIsSendCustomer, pIsPageNumber, pIsDisplayHeader, pIsDisplayFooter, pDefaultData, pCreateId });
        }
    }
}