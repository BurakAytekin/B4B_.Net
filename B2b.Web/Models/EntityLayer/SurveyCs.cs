using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class SurveyCs : DataAccess
    {
        #region Ctor
        public SurveyCs()
        {
            Questions = new List<SurveyQuetion>();
        }
        #endregion
        #region Properties
        public int Id { get; set; }
        public string Header { get; set; }
        public List<SurveyQuetion> Questions { get; set; }

        #endregion
        #region Methods

        public static List<SurveyCs> GetList()
        {
            DataTable dt = DAL.GetSurvetList();
            List<SurveyCs> list = new List<SurveyCs>();
            foreach (DataRow row in dt.Rows)
            {
                SurveyCs item = new SurveyCs();
                {
                    item.Id = row.Field<int>("Id");
                    item.Header = row.Field<string>("Header");
                    item.CreateDate = row.Field<DateTime>("CreateDate");
                };
                list.Add(item);
            }
            return list;
        }
        public static SurveyCs GetItemById(int Id)
        {
            DataSet ds = DAL.GetSurveyById(Id);
            SurveyCs obj = null;

            if (ds.Tables[0].Rows.Count > 0)
            {
                obj = new SurveyCs()
                {
                    Id = ds.Tables[0].Rows[0].Field<int>("Id"),
                    Header = ds.Tables[0].Rows[0].Field<string>("Header"),
                };

                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        SurveyQuetion surveyQuetion = new SurveyQuetion()
                        {
                            Id = row.Field<int>("Id"),
                            Question = row.Field<string>("Content"),
                            Type = row.Field<SurveyQuestionType>("Type"),
                        };

                        if (row.Field<SurveyQuestionType>("Type") == SurveyQuestionType.Multi || row.Field<SurveyQuestionType>("Type") == SurveyQuestionType.Single)
                        {
                            surveyQuetion.Options.AddRange(SurveyQuestionOptions.GetById(surveyQuetion.Id));
                        }

                        obj.Questions.Add(surveyQuetion);
                    }
                }
            }
            return obj;

        }
        public static bool SaveAnswer(int surveyId, int questionId, int customerId, string answer)
        {
            return DAL.InsertSurveyAnswer(surveyId, questionId, customerId, answer);
        }

        #endregion
    }
    public class SurveyQuetion
    {
        #region Ctor
        public SurveyQuetion()
        {
            Answer = string.Empty;
            Note = string.Empty;
            Options = new List<SurveyQuestionOptions>();
            SelectedOptions = new List<string>();
        }


        #endregion

        #region Properties
        public int Id { get; set; }
        public string Question { get; set; }

        [Required(ErrorMessage = "Lütfen Bu Soruyu Boş Bırakmayınız.")]
        public string Answer
        {
            get
            {
                if (Type == SurveyQuestionType.Multi)
                {
                    return string.Join(",", SelectedOptions);
                }
                else
                    return answer;

            }
            set { answer = value; }
        }
        private string answer;
        public List<SurveyQuestionOptions> Options { get; set; }
        public List<string> SelectedOptions { get; set; }
        public SurveyQuestionType Type { get; set; }
        public string Note { get; set; }

        #endregion
    }

    public class SurveyQuestionOptions : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Option { get; set; }
        #endregion
        #region Methods
        public static List<SurveyQuestionOptions> GetById(int Id)
        {

            DataTable dt = DAL.GetSurveyQuestionOptionsById(Id);
            List<SurveyQuestionOptions> list = new List<SurveyQuestionOptions>();
            foreach (DataRow row in dt.Rows)
            {
                SurveyQuestionOptions item = new SurveyQuestionOptions();
                {
                    item.Id = row.Field<int>("Id");
                    item.Option = row.Field<string>("Content");
                };
                list.Add(item);
            }
            return list;

        }
        #endregion
    }

    public enum SurveyQuestionType
    {
        Single = 1,
        Multi = 2,
        Text = 3,
        DateTime = 4
    }

    public partial class DataAccessLayer
    {
        public DataTable GetSurvetList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Survey");
        }
        public DataSet GetSurveyById(int pId)
        {
            return DatabaseContext.ExecuteReaderDs(CommandType.StoredProcedure, "_GetItem_SurveyById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
        public DataTable GetSurveyQuestionOptionsById(int pQuestionId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_SurveyQuestionOptionsById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pQuestionId });
        }
        public bool InsertSurveyAnswer(int pSurveyId, int pQuestionId, int pCustomerId, string pAnswer)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_SurveyAnswer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSurveyId, pQuestionId, pCustomerId, pAnswer });
        }
    }
}