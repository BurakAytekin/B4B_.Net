using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class SolrSearch
    {
        public SolrSearch()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static string QueryBuilder(string t9text, string manufaturer, string vehicle_brand, string vehicle_model)
        {
            string query = string.Empty;
            string url = "http://mysql.eryaz.net:8983/solr/xxxxx/select?q=";

            t9text = t9text == null ? null : (t9text.Replace("+", " ").Replace("-", " ").Replace("&", " ").Replace("!", " ").Replace("(", " ").
                Replace("[", " ").Replace("]", " ").Replace(")", " ").Replace("{", " ").Replace("}", " ").Replace("\"", " ").Replace("/", " ").
                Replace("~", " ").Replace("^", " ").Replace(":", " ").Replace("?", " ").Replace(".", " ").Replace(",", " ").Replace(";", " "));


            // t9
            if (!string.IsNullOrEmpty(t9text))
            {
                t9text = t9text.Trim();
                string[] t9Arr = t9text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < t9Arr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";
                    query += " +T9Text:*" + t9Arr[i].ToUpper().Replace("Ö", "O").Replace("Ü", "U").Replace("İ", "I").Replace("Ş", "S").Replace("Ğ", "G").Replace("Ç", "C") + "*";
                }
                url += query;
            }
            else
                url += "*:*";

            // manufacturer
            if (!string.IsNullOrEmpty(manufaturer))
                url += "&fq=Manufacturer:\"" + manufaturer + "\"";

            if (!string.IsNullOrEmpty(vehicle_brand))
                url += "&fq=T9VahicleBrand:\"" + vehicle_brand + "\"";

            if (!string.IsNullOrEmpty(vehicle_model))
                url += "&fq=T9VehicleModel :\"" + vehicle_model + "\"";


            url += "&rows=13000&fl=id&wt=json";
            return url;
        }

        public static string SplitGroupIds(List<Doc> docs)
        {
            string groupidStr = string.Empty;
            foreach (var item in docs)
            {
                if (!string.IsNullOrEmpty(groupidStr))
                    groupidStr += ",";
                groupidStr += item.id;
            }

            return groupidStr;
        }

        public static RootObject Search(string link)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Timeout = 10000;
            WebResponse response = request.GetResponse();
            Encoding enc = Encoding.GetEncoding("iso-8859-9");
            StreamReader sRed = new StreamReader(response.GetResponseStream(), enc);

            return JsonSerializer.Deserialize<RootObject>(sRed.ReadToEnd());
        }

        public class Params
        {
            public string q { get; set; }
            public string fl { get; set; }
            public string rows { get; set; }
            public string wt { get; set; }
        }

        public class ResponseHeader
        {
            public int status { get; set; }
            public int QTime { get; set; }
            public Params @params { get; set; }
        }

        public class Doc
        {
            public string id { get; set; }
        }

        public class Response
        {
            public int numFound { get; set; }
            public int start { get; set; }
            public List<Doc> docs { get; set; }
        }

        public class RootObject
        {
            public ResponseHeader responseHeader { get; set; }
            public Response response { get; set; }
        }
    }
}