using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models
{
    public class MarsLojistik_RowDetail
    {
        public string ProductCode { get; set; }
        public double Quantity { get; set; }
        public List<string> SerialNoList { get; set; }
        public List<string> SerialNoProblemList { get; set; }
        public string OrderNo { get; set; }

        public Natra_SiparisD NatraSiparisD { get; set; }
        public Natra_StokH NatraStokH { get; set; }

        public double BirimFiyat { get { return NatraSiparisD != null ? NatraSiparisD.BirimFiyat : 0; } }
        public double DvzBirimFiyat { get { return NatraSiparisD != null ? NatraSiparisD.DvzBirimFiyat : 0; } }
        public double NetBirimFiyat { get { return NatraSiparisD != null ? NatraSiparisD.NetBirimFiyat : 0; } }

        public double BrutTutar { get { return NatraSiparisD != null ? NatraSiparisD.BirimFiyat * Quantity : 0; } }
        public double DvzBrutFiyat { get { return NatraSiparisD != null ? NatraSiparisD.DvzBirimFiyat * Quantity : 0; } }

        public double NetTutar { get { return NatraSiparisD != null ? NatraSiparisD.NetBirimFiyat * Quantity : 0; } }
        public double DvzNetTutar { get { return NetTutar / DovizKuru; } }

        public int KDV { get { return NatraSiparisD != null ? NatraSiparisD.KDV : 18; } }

        public double IskontoTutar { get { return BrutTutar - NetTutar; } }
        public double DvzIskontoTutar { get { return IskontoTutar / DovizKuru; } }

        public double KdvTutar { get { return NetTutar * KDV / 100; } }
        public double DvzKdvTutar { get { return KdvTutar / DovizKuru; } }

        public double GenelTutar { get { return NetTutar + KdvTutar; } }
        public double DvzGenelTutar { get { return GenelTutar / DovizKuru; } }


        public string StokAciklamasi { get { return NatraSiparisD != null ? NatraSiparisD.StokAciklamasi : string.Empty; } }
        public string OlcuBirimi { get { return NatraSiparisD != null ? NatraSiparisD.OlcuBirimi : "ADET"; } }
        public string DovizKodu { get { return NatraSiparisD != null ? NatraSiparisD.DovizKodu : string.Empty; } }
        public double DovizKuru { get { return NatraSiparisD != null ? NatraSiparisD.DovizKuru : 1; } }
        public double IskontoOrani1 { get { return NatraSiparisD != null ? NatraSiparisD.IskontoOrani1 : 0; } }
        public double IskontoOrani2 { get { return NatraSiparisD != null ? NatraSiparisD.IskontoOrani2 : 0; } }
        public double IskontoOrani3 { get { return NatraSiparisD != null ? NatraSiparisD.IskontoOrani3 : 0; } }
        public double IskontoOrani4 { get { return NatraSiparisD != null ? NatraSiparisD.IskontoOrani4 : 0; } }
        public double IskontoOrani5 { get { return NatraSiparisD != null ? NatraSiparisD.IskontoOrani5 : 0; } }

        public double ToplamAgirlik { get { return NatraStokH != null ? NatraStokH.Agirlik * Quantity : 0; } }



        public int Natra_IrsaliyeD_Id { get; set; }
        public string Natra_IrsaliyeD_UUID { get; set; }



        public MarsLojistik_RowDetail()
        {
            SerialNoList = new List<string>();
            NatraSiparisD = null;
            NatraStokH = null;
        }
    }


    public class Natra_SiparisD
    {
        public string StokKodu { get; set; }
        public string StokAciklamasi { get; set; }
        public string OlcuBirimi { get; set; }

        public string EvrakNo { get; set; }
        public string HesapKodu { get; set; }

        public double Miktar { get; set; }
        public double Kalan { get; set; }


        public double BirimFiyat { get; set; }
        public double DvzBirimFiyat { get; set; }
        public double NetBirimFiyat { get; set; }

        public double BrutTutar { get; set; }
        public double DvzBrutFiyat { get; set; }
        public double NetTutar { get; set; }


        public string DovizKodu { get; set; }
        public double DovizKuru { get; set; }
        public int KDV { get; set; }

        public double IskontoOrani1 { get; set; }
        public double IskontoOrani2 { get; set; }
        public double IskontoOrani3 { get; set; }
        public double IskontoOrani4 { get; set; }
        public double IskontoOrani5 { get; set; }

        public int SiraNo { get; set; }

    }

    public class Natra_SiparisH
    {
        public string EvrakNo { get; set; }
        public string HesapKodu { get; set; }
        public string Seri { get; set; }
        public string BelgeNo { get; set; }
        public DateTime BelgeTarihi { get; set; }
        public string DovizKodu { get; set; }
        public double DovizKuru { get; set; }
        public double BrutTutar { get; set; }
        public double GenelIskonto { get; set; }
        public double KDVToplam { get; set; }
        public double GenelToplam { get; set; }
        public double IskontoOrani1 { get; set; }
        public double IskontoOrani2 { get; set; }
        public DateTime VadeTarihi { get; set; }
        public int VadeGunu { get; set; }
        public string Aciklama1 { get; set; }
        public string Aciklama2 { get; set; }
        public string Aciklama3 { get; set; }
        public string Aciklama4 { get; set; }
        public string Kargo { get; set; }
        public string SiparisNotlari { get; set; }
        public DateTime TeslimatTarihi { get; set; }
        public string TeslimatAdresi { get; set; }
        public string HesapAciklamasi { get; set; }
        public string Adres { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string Depokodu { get; set; }
    }

    public class Natra_Seritakibi
    {
        public int ID { get; set; }
        public string SeriNo { get; set; }
        public string StokKodu { get; set; }
        public string EvrakNo { get; set; }
        public double GarantiSure { get; set; }
        public string UUID { get; set; }
        public string CUUID { get; set; }
    }

    public class Natra_StokH
    {
        public string StokKodu { get; set; }
        public string StokAciklamasi { get; set; }
        public double Agirlik { get; set; }
    }
}