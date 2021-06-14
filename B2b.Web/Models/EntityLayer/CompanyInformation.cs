

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class CompanyInformation : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public string AddressTitle { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Gsm { get; set; }
        public string Fax { get; set; }
        public byte[] Picture { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        public string RegistrationNo { get; set; }
        public int Status { get; set; }
        public string MapPath { get; set; }
        public string BranchOffice { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string MersisNo { get; set; }
        public string Base64String { get { return Picture == null ? string.Empty : "data:image/jpg;base64," + Convert.ToBase64String(Picture, 0, Picture.Length); } }
        #endregion

        #region Methods
        public static CompanyInformation GetByStatus(int pStatus)
        {
            DataTable dt = DAL.GetCompanyInformationByStatus(pStatus);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                CompanyInformation companyInformation = new CompanyInformation()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),
                    Surname = row.Field<string>("Surname"),
                    Title = row.Field<string>("Title"),
                    WebSite = row.Field<string>("WebSite"),
                    Picture = row.Field<byte[]>("Picture"),
                    Phone1 = row.Field<string>("Phone1"),
                    Phone2 = row.Field<string>("Phone2"),
                    Fax = row.Field<string>("Fax"),
                    Gsm = row.Field<string>("Gsm"),
                    Email1 = row.Field<string>("Email1"),
                    Email2 = row.Field<string>("Email2"),
                    Address = row.Field<string>("Address"),
                    RegistrationNo = row.Field<string>("RegistrationNo"),
                    MapPath = row.Field<string>("MapPath"),
                    TaxOffice = row.Field<string>("TaxOffice"),
                    TaxNumber = row.Field<string>("TaxNumber"),
                    MersisNo = row.Field<string>("MersisNo"),


                };
                return companyInformation;
            }

            return null;
        }

        public static List<CompanyInformation> GetAll()
        {
            DataTable dt = DAL.GetCompanyInformationByAll();
            List<CompanyInformation> list = new List<CompanyInformation>();
            foreach (DataRow row in dt.Rows)
            {
                CompanyInformation companyInformation = new CompanyInformation()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),
                    AddressTitle = row.Field<string>("AddressTitle"),
                    Surname = row.Field<string>("Surname"),
                    Title = row.Field<string>("Title"),
                    WebSite = row.Field<string>("WebSite"),
                    Picture = row.Field<byte[]>("Picture"),
                    Phone1 = row.Field<string>("Phone1"),
                    Phone2 = row.Field<string>("Phone2"),
                    Fax = row.Field<string>("Fax"),
                    Gsm = row.Field<string>("Gsm"),
                    Email1 = row.Field<string>("Email1"),
                    Email2 = row.Field<string>("Email2"),
                    Address = row.Field<string>("Address"),
                    RegistrationNo = row.Field<string>("RegistrationNo"),
                    MapPath = row.Field<string>("MapPath"),
                    TaxOffice = row.Field<string>("TaxOffice"),
                    TaxNumber = row.Field<string>("TaxNumber"),
                    MersisNo = row.Field<string>("MersisNo"),


                };
                list.Add(companyInformation);
            };
            return list;



        }
        public bool Add()
        {
            return DAL.InsertContact(Title, Phone1, Phone2, Fax, WebSite, Email1, Email2, Address, MapPath, TaxOffice, TaxNumber, MersisNo, Picture, AddressTitle, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateContact(Id, Title, Phone1, Phone2, Fax, WebSite, Email1, Email2, Address, MapPath, TaxOffice, TaxNumber, MersisNo, Picture, AddressTitle, EditId);
        }
        public static bool Delete(int id)
        {
            return DAL.DeleteContact(id);
        }

        #endregion
    }
    public partial class DataAccessLayer
    {
        public bool InsertContact(string pTitle, string pPhone1, string pPhone2, string pFax, string pWebSite, string pEmail1, string pEmail2, string pAddress, string pMapPath, string pTaxOffice, string pTaxNumber, string pMersisNo, byte[] pPicture, string pAddressTitle, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_CompanyInformation", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTitle, pPhone1, pPhone2, pFax, pWebSite, pEmail1, pEmail2, pAddress, pMapPath, pTaxOffice, pTaxNumber, pMersisNo, pPicture, pAddressTitle, pCreateId });
        }

        public bool UpdateContact(int pId, string pTitle, string pPhone1, string pPhone2, string pFax, string pWebSite, string pEmail1, string pEmail2, string pAddress, string pMapPath, string pTaxOffice, string pTaxNumber, string pMersisNo, byte[] pPicture, string pAddressTitle, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_CompantInformation", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pTitle, pPhone1, pPhone2, pFax, pWebSite, pEmail1, pEmail2, pAddress, pMapPath, pTaxOffice, pTaxNumber, pMersisNo, pPicture, pAddressTitle, pEditId });
        }
        public bool DeleteContact(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_CompantInformation", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public DataTable GetCompanyInformationByStatus(int pStatus)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CompanyInformationByStatus", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStatus });
        }
        public DataTable GetCompanyInformationByAll()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CompanyInformationByAll", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

    }
}