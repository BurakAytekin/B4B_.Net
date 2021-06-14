using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Mars_SerialNumber : DataAccess
    {
        public int Id { get; set; }
        public string SeriNo { get; set; }
        public string StokKodu { get; set; }
        public string EvrakNo { get; set; }
        public DateTime Tarih { get; set; }
        public string HesapKodu { get; set; }
        public string UUID { get; set; }
        public bool IsProcessed { get; set; }
        public string DepoKodu { get; set; }

        public Mars_SerialNumber()
        {
            IsProcessed = false;
        }


        public bool Save()
        {
            return DAL.InsertMars_SerialNumber(SeriNo, StokKodu, EvrakNo, Tarih, HesapKodu, UUID, IsProcessed, DepoKodu);
        }

        public bool Update()
        {
            return DAL.UpdateMars_SerialNumber(Id, SeriNo, StokKodu, EvrakNo, Tarih, HesapKodu, UUID, IsProcessed);
        }

        public static List<Mars_SerialNumber> GetList()
        {
            List<Mars_SerialNumber> list = new List<Mars_SerialNumber>();
            DataTable dt = DAL.GetMars_SerialNumbeList();

            foreach (DataRow row in dt.Rows)
            {
                Mars_SerialNumber obj = new Mars_SerialNumber()
                {
                    Id = row.Field<int>("Id"),
                    SeriNo = row.Field<string>("SeriNo"),
                    StokKodu = row.Field<string>("StokKodu"),
                    EvrakNo = row.Field<string>("EvrakNo"),
                    Tarih = row.Field<DateTime>("Tarih"),
                    HesapKodu = row.Field<string>("HesapKodu"),
                    UUID = row.Field<string>("UUID"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    IsProcessed = row.Field<bool>("IsProcessed"),
                    DepoKodu = row.Field<string>("DepoKodu"),
                };
                list.Add(obj);
            }

            return list;
        }

        public static List<Mars_SerialNumber> GetListUnprocessed()
        {
            List<Mars_SerialNumber> list = new List<Mars_SerialNumber>();
            DataTable dt = DAL.GetMars_SerialNumbeListUnprocessed();

            foreach (DataRow row in dt.Rows)
            {
                Mars_SerialNumber obj = new Mars_SerialNumber()
                {
                    Id = row.Field<int>("Id"),
                    SeriNo = row.Field<string>("SeriNo"),
                    StokKodu = row.Field<string>("StokKodu"),
                    EvrakNo = row.Field<string>("EvrakNo"),
                    Tarih = row.Field<DateTime>("Tarih"),
                    HesapKodu = row.Field<string>("HesapKodu"),
                    UUID = row.Field<string>("UUID"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    IsProcessed = row.Field<bool>("IsProcessed"),
                    DepoKodu = row.Field<string>("DepoKodu"),
                };
                list.Add(obj);
            }

            return list;
        }
    }

    public partial class DataAccessLayer
    {
        public bool InsertMars_SerialNumber(string pSeriNo, string pStokKodu, string pEvrakNo, DateTime pTarih, string pHesapKodu, string pUUID, bool pIsProcessed, string pDepoKodu)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Mars_SerialNumber", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSeriNo, pStokKodu, pEvrakNo, pTarih, pHesapKodu, pUUID, pIsProcessed, pDepoKodu });
        }

        public bool UpdateMars_SerialNumber(int pId, string pSeriNo, string pStokKodu, string pEvrakNo, DateTime pTarih, string pHesapKodu, string pUUID, bool pIsProcessed)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_Mars_SerialNumber", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pSeriNo, pStokKodu, pEvrakNo, pTarih, pHesapKodu, pUUID, pIsProcessed });
        }

        public DataTable GetMars_SerialNumbeList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Mars_SerialNumber");
        }

        public DataTable GetMars_SerialNumbeListUnprocessed()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Mars_SerialNumberUnprocessed");
        }


    }
}