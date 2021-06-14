using B2b.Web.v4.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace B2b.Web.v4.Areas.Admin.Models
{
    public class JobMarsEntegrationFrom : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // klasör içinde yeni dosya var mı kontrolü. 
            string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress + "MarsLogistics/OUT/";
            string fullFtpFilePathArchive = "/MarsLogistics/OUT/Archive/";
            List<string> outFiles = FtpHelper.ListDirectory(fullFtpFilePath);
            outFiles.RemoveAt(0); // Archive klasörü silinecek

            //klasör varsa,  verileri oku. 
            foreach (string fileName in outFiles)
            {

                // veriyi oku
                string fileUri = fullFtpFilePath + fileName;
                string fileUriArchive = fullFtpFilePathArchive + fileName;

                byte[] fileBytes = FtpHelper.DownloadRemoteServer(fileUri);
                string fileStr = Encoding.UTF8.GetString(fileBytes);

                //seri takibi için serial xml hatası var. Bunu düzeltmek amaçlı eklendi. 
                fileStr = fileStr.Replace("</SERIAL>\r\n<SERIAL>", "");
                //fileStr = Regex.Replace(input, @"\r\n?|\n", replacementString);

                TRANSACTIONPACKET marsPacket = ExtensionMethods.Deserialize<TRANSACTIONPACKET>(fileStr);


                WAYBILLMASTER marsPacketMaster = marsPacket.MASTERS.MASTER;

                // Kullanılacak dataların hazırlanaması
                string marsTransactionNo = marsPacketMaster.TRANSACTION_NO;
                string marsWaybillNo = marsPacketMaster.CONSIGMENT_NOTE_NUMBER;
                DateTime marsWaybillDate = DateTime.ParseExact(marsPacketMaster.CONSIGMENT_NOTE_DATE, "dd/MM/yyyy", CultureInfo.CurrentCulture);
                string marsOrderNo = marsPacketMaster.CUSTOMER_ORDER_NUMBER; //marsPacketMaster.WORKORDER_NO;
                string natraOrderNo = marsPacketMaster.CUSTOMER_ORDER_NUMBER.Replace("HNT-", "").Replace("HNT", "");
                string marsCustomerNo = marsPacketMaster.FIRMID;
                string natraCustomerNo = marsPacketMaster.FIRMID.Replace("HNT-", "").Replace("HNT.", "").Replace("HNT", "");


                Customer natraCustomer = Customer.GetCustomerByCode(natraCustomerNo);
                OrderHeader.UpdateOrderShippingStatu(natraOrderNo, ShippingStatu.Shipped);

                //try
                //{
                //    List<SalesmanOfCustomer> salesmanOfCustomers = SalesmanOfCustomer.GetSalesmanOfCustomer(natraCustomer.Id);

                //    MailMessage mail = new MailMessage();
                //    mail.Subject = $"İrsaliye Düzenlenmiştir, {natraOrderNo}";
                //    mail.Body = $"";
                //    mail.IsBodyHtml = true;

                //    if (EmailHelper.IsValidEmail(natraCustomer.Mail))
                //        mail.To.Add(natraCustomer.Mail);

                //    foreach (var item in salesmanOfCustomers)
                //    {
                //        if (EmailHelper.IsValidEmail(item.Salesman.Email))
                //            mail.To.Add(item.Salesman.Email);
                //    }

                //    if (mail.To.Count > 0)
                //    {
                //        EmailHelper.Send(mail);
                //        Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Information, v4.Models.Log.ClientType.B2BWeb, "timer_mars_from_mail", $"{natraOrderNo} : Mail Gönderim Başarılı. {mail.To.Select(x => x.Address).Aggregate((i, j) => i + ";" + j)}", string.Empty);
                //    }
                //    else
                //        Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Information, v4.Models.Log.ClientType.B2BWeb, "timer_mars_from_mail", $"{natraOrderNo} : Gönderilecek Mail Adresi Bulunamadı", string.Empty);


                //}
                //catch (Exception ex)
                //{
                //    Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Error, v4.Models.Log.ClientType.B2BWeb, "timer_mars_from_mail", ex.Message, string.Empty);
                //}

                string natraWaybillNo = string.Empty;
                int natraWaybillNoInt = 0;

                List<MarsLojistik_RowDetail> marsDetailList = new List<MarsLojistik_RowDetail>();
                foreach (var item in marsPacketMaster.DETAILS.DETAIL)
                {
                    MarsLojistik_RowDetail detailObj = new MarsLojistik_RowDetail();
                    detailObj.ProductCode = item.MATERIAL_CODE;
                    detailObj.Quantity = Convert.ToDouble(item.QUANTITY);
                    if (item.SERIAL != null)
                        detailObj.SerialNoList.AddRange(item.SERIAL.SERIALNO);
                    detailObj.OrderNo = item.ORDER_NUMBER;
                    detailObj.Natra_IrsaliyeD_UUID = Guid.NewGuid().ToString().ToUpper();

                    marsDetailList.Add(detailObj);
                }

                //if (natraOrderNo == "SSP0000835")
                //{
                //    string a = "a";
                //}

                if (GlobalSettings.MarsEntegration)
                {
                    string query = string.Empty;
                    DataTable dt;



                    // Natrada bu numarada sipariş var mı kontrolü
                    query = string.Format("SELECT EvrakNo FROM siparis_h WHERE EvrakNo='{0}'", natraOrderNo);
                    dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                    // NATRA İŞLEMLERİ
                    if (dt.Rows.Count > 0)
                    {
                        query = string.Format("SELECT * FROM siparis_h WHERE EvrakNo='{0}'", natraOrderNo);
                        dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                        Natra_SiparisH natra_SiparisH = dt.DataTableToItem<Natra_SiparisH>();

                        // natra son irsaliye numarasını al
                        query = "SELECT EvrakNo FROM evrakno WHERE Modul_ID=4;";
                        dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);
                        natraWaybillNo = dt.Rows[0][0].ToString();
                        natraWaybillNo = natraWaybillNo.Substring(3);
                        natraWaybillNoInt = Convert.ToInt32(natraWaybillNo);

                        // natra yeni irsaliye numarasını oluştur
                        natraWaybillNoInt++;
                        natraWaybillNo = string.Format("SIR{0}", natraWaybillNoInt.ToString().PadLeft(7, '0'));

                        // natra irsaliye sayacını güncelle
                        query = "UPDATE evrakno SET EvrakNo='" + natraWaybillNo + "' WHERE Modul_ID=4; SELECT 1;";
                        dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);


                        // natra irsaliyenin oluşması
                        {
                            // irsaliye_no eski kayıt varsa sil
                            query = string.Format("DELETE FROM irsaliye_no where evrakno = '{0}'; SELECT 1;", natraWaybillNo);
                            dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                            // irsaliye_no yeni kayıt ekle
                            query = string.Format("INSERT INTO irsaliye_no (evrakno,belgeno,seri,seri2,etkin) values ('{0}','{1}','A','',1); SELECT 1;",
                                natraWaybillNo,
                                marsWaybillNo
                                );
                            dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                            // irsaliye için satır verilerinin oluşturulması
                            {
                                // siparis_d den satırların getirilmesi
                                query = string.Format("SELECT * FROM siparis_d WHERE EvrakNo='{0}'", natraOrderNo);
                                dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                                List<Natra_SiparisD> natraSiparisDList = dt.DataTableToList<Natra_SiparisD>();

                                foreach (Natra_SiparisD natra_SiparisDRow in natraSiparisDList)
                                {
                                    if (marsDetailList.Any(p => p.ProductCode == natra_SiparisDRow.StokKodu && p.NatraSiparisD == null))
                                        marsDetailList.First(p => p.ProductCode == natra_SiparisDRow.StokKodu && p.NatraSiparisD == null).NatraSiparisD = natra_SiparisDRow;
                                }

                                foreach (MarsLojistik_RowDetail marsRow in marsDetailList)
                                {
                                    query = string.Format("SELECT * FROM stok_h WHERE StokKodu = '{0}' LIMIT 1", marsRow.ProductCode);
                                    dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                                    if (dt.Rows.Count > 0)
                                        marsRow.NatraStokH = dt.DataTableToItem<Natra_StokH>();
                                }
                            }

                            // irsaliye başlık verirsinin içeri alınması
                            query = "INSERT INTO irsaliye_h (EvrakNo, HesapKodu, Seri, BelgeNo, BelgeTarihi, BelgeTuru, KDV, DovizKodu, DovizKuru, OzelNotlar, Aciklama1, Aciklama2, Aciklama3, Aciklama4, OzelKod1, OzelKod2, BrutTutar, GenelIskonto, KDVToplam, GenelToplam, IskontoOrani1, IskontoOrani2, IskontoTutar1, IskontoTutar2, KDVToplam1, KDVToplam2, Tipi, Depokodu, IscilikToplam, YedekParcaToplam, DepoKodu2, VadeTarihi, VadeGunu, Aciklama, TeslimatTarihi, TeslimatAdresi, HesapAciklamasi, Adres, VergiNo, VergiDairesi, Il, Ilce, Kargo, YetkiSeviyesi, MusteriTemsilcisi, IskontoKodu, IrsaliyeNotlari, Yazdir, Inceleme, Onay, ProjeKodu, EvrakNoTalep, KargoOdemeTipi, OdemeKodu, MaliyetNoktasi, EvrakNoFatura, BelgeSaati, EvrakNoUretim, ToplamHacim, ToplamAgirlik, Okuma, OkumaTarihi, EvrakNoStokUretim, Bedelsiz, BayiNo, MusteriNo, USER, MUSER, MRECDATE, RECDATE, BirlestirilmisKayit, SatirdaDepoCikis, SiparisYonetimID, SMS, SMSTarihi, EvrakNoRecete, SatirIskontoToplam, SatirIskontoToplamDvz, GenelIskontoDvz, KDVToplamDvz, GenelToplamDvz, AraToplam, AraToplamDvz, BrutTutarDvz, Dokuman, EIrsaliyeSeri, EIrsaliye, EIrsaliyeDurum, EIrsaliyeTipi, KontrolDepartmani, UUID, EIrsaliyeSenaryo, TasiyiciVKN, TasiyiciUnvan, AracPlakaNo, DorsePlakaNo, SurucuAdi, SurucuSoyadi, SurucuTCKN, SurucuAdi2, SurucuSoyadi2, SurucuTCKN2, InternetSatis, OdemeTarihi, OdemeSekli, OdemeSekliDiger, OdemeAracisiAdi, SatisWebAdresi, SiparisHUUID, PartiNoDurumu) VALUES   ('{EvrakNo}', '{HesapKodu}', '{Seri}', '{BelgeNo}', '{BelgeTarihi}', 'Açık', 'Hariç', '{DovizKodu}', {DovizKuru}, NULL, '{Aciklama1}', '{Aciklama2}', '{Aciklama3}', '{Aciklama4}', '{OzelKod1}', '', {BrutTutar}, {GenelIskonto}, {KDVToplam}, {GenelToplam}, {IskontoOrani1}, {IskontoOrani2}, {IskontoTutar1}, {IskontoTutar2}, {KDVToplam1}, {KDVToplam2}, '{Tipi}', '{Depokodu}', {IscilikToplam}, {YedekParcaToplam}, NULL, '{VadeTarihi}', {VadeGunu}, 'İrsaliyemiz', '{TeslimatTarihi}', '{TeslimatAdresi}', '{HesapAciklamasi}', '{Adres}', '{VergiNo}', '{VergiDairesi}', '{Il}', '{Ilce}', '{Kargo}', {YetkiSeviyesi}, '', '', '{IrsaliyeNotlari}', 0, 0, 0, '', NULL, 0, '', NULL, NULL, '{BelgeSaati}', NULL, 0, {ToplamAgirlik}, NULL, NULL, NULL, 0, '', '', 'ERYAZ', NULL, NULL, '{RECDATE}', NULL, NULL, NULL, NULL, NULL, NULL, {SatirIskontoToplam}, {SatirIskontoToplamDvz}, {GenelIskontoDvz}, {KDVToplamDvz}, {GenelToplamDvz}, {AraToplam}, {AraToplamDvz}, {BrutTutarDvz}, NULL, '', 0, NULL, NULL, NULL, NULL, NULL, '', '', '', '', '', '', '', '', '', '', 0, NULL, '', NULL, '', '', '', NULL); SELECT 1;";

                            query = query
                                .Replace("{EvrakNo}", natraWaybillNo)
                                .Replace("{HesapKodu}", natraCustomer.Code)
                                .Replace("{Seri}", "A")
                                .Replace("{BelgeNo}", marsWaybillNo)
                                .Replace("{BelgeTarihi}", marsWaybillDate.ToString("yyyy-MM-dd")) // DateTime.Now.ToString("yyyy-MM-dd"))
                                .Replace("{DovizKodu}", natra_SiparisH.DovizKodu)
                                .Replace("{DovizKuru}", natra_SiparisH.DovizKuru.ToString("0.0000", CultureInfo.InvariantCulture))
                                .Replace("{Aciklama1}", natra_SiparisH.Aciklama1.Replace("'", "''"))
                                .Replace("{Aciklama2}", natra_SiparisH.Aciklama2.Replace("'", "''"))
                                .Replace("{Aciklama3}", natra_SiparisH.Aciklama3.Replace("'", "''"))
                                .Replace("{Aciklama4}", marsTransactionNo) //natra_SiparisH.Aciklama4)
                                .Replace("{OzelKod1}", "MARS")
                                .Replace("{GenelIskonto}", "0") // ??
                                .Replace("{GenelIskontoDvz}", "0") // ?? 
                                .Replace("{IskontoOrani1}", "0") // ??
                                .Replace("{IskontoOrani2}", "0") // ??
                                .Replace("{IskontoTutar1}", "0") // ??
                                .Replace("{IskontoTutar2}", "0") // ??
                                .Replace("{KDVToplam1}", "NULL")
                                .Replace("{KDVToplam2}", "NULL")
                                .Replace("{BrutTutar}", marsDetailList.Sum(p => p.BrutTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{BrutTutarDvz}", marsDetailList.Sum(p => p.DvzBrutFiyat).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{SatirIskontoToplam}", marsDetailList.Sum(p => p.IskontoTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{SatirIskontoToplamDvz}", marsDetailList.Sum(p => p.DvzIskontoTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{AraToplam}", marsDetailList.Sum(p => p.NetTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{AraToplamDvz}", marsDetailList.Sum(p => p.DvzNetTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{KDVToplam}", marsDetailList.Sum(p => p.KdvTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{KDVToplamDvz}", marsDetailList.Sum(p => p.DvzKdvTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{GenelToplam}", marsDetailList.Sum(p => p.GenelTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{GenelToplamDvz}", marsDetailList.Sum(p => p.DvzGenelTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{Tipi}", "S")
                                .Replace("{Depokodu}", natra_SiparisH.Depokodu)
                                .Replace("{IscilikToplam}", "0")
                                .Replace("{YedekParcaToplam}", marsDetailList.Sum(p => p.NetTutar).ToString("0.000000", CultureInfo.InvariantCulture))
                                .Replace("{VadeTarihi}", marsWaybillDate.ToString("yyyy-MM-dd")) // DateTime.Now.ToString("yyyy-MM-dd")) // ??
                                .Replace("{VadeGunu}", "0") // ??
                                .Replace("{TeslimatTarihi}", marsWaybillDate.ToString("yyyy-MM-dd")) //  DateTime.Now.ToString("yyyy-MM-dd"))
                                .Replace("{TeslimatAdresi}", natra_SiparisH.TeslimatAdresi.Replace("'", "''")) // ??
                                .Replace("{HesapAciklamasi}", natra_SiparisH.HesapAciklamasi.Replace("'", "''"))
                                .Replace("{Adres}", natra_SiparisH.Adres.Replace("'", "''"))
                                .Replace("{VergiNo}", natra_SiparisH.VergiNo.Replace("'", "''"))
                                .Replace("{VergiDairesi}", natra_SiparisH.VergiDairesi.Replace("'", "''"))
                                .Replace("{Il}", natra_SiparisH.Il.Replace("'", "''"))
                                .Replace("{Ilce}", natra_SiparisH.Ilce.Replace("'", "''"))
                                .Replace("{Kargo}", natra_SiparisH.Kargo.Replace("'", "''"))
                                .Replace("{YetkiSeviyesi}", "3")
                                .Replace("{IrsaliyeNotlari}", natra_SiparisH.SiparisNotlari.Replace("'", "''"))
                                .Replace("{BelgeSaati}", "00:00:00") //DateTime.Now.ToString("HH:mm:ss"))
                                .Replace("{RECDATE}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Replace("{ToplamAgirlik}", marsDetailList.Sum(p => p.ToplamAgirlik).ToString("0.00", CultureInfo.InvariantCulture));

                            dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);


                            int c = 1;
                            // irsaliye satır verirsinin içeri alınması
                            foreach (var marsDetailItem in marsDetailList)
                            {
                                query = "INSERT INTO irsaliye_d (HesapKodu, EvrakNo, KalemTipi, StokKodu, StokAciklamasi, Miktar, OlcuBirimi, BirimFiyat, BrutTutar, DovizKodu, DovizKuru, KDV, NetTutar, IskontoOrani1, IskontoOrani2, DvzBirimFiyat, DvzBrutFiyat, EvrakNoFatura, EvrakNoSiparis, OlcuBirimi1, Carpan, SiraNo, OzelKod1, OzelKod2, OzelKod3, OzelKod4, FaturaD_ID, GecisID, SiparisID, SeriMiktar, NetBirimFiyat, Onay, KontrolMiktar, RenkKodu, BedenKodu, IskontoOrani3, IskontoOrani4, StokKodu2, StokAciklamasi2, FiyatDegisim, SonAlisFiyati, Boy, En, Adet, StokDetayID, EvrakNoServisCagrisi, DepoKodu, EvrakNoIsEmri, PaketHazirlik, HataTanimi, GonderilisNedeni, Aciklama, AlinanKarar, OperasyonID, USER, MUSER, RECDATE, MRECDATE, EvrakNoKantar, UUID, KaliteOnay, OzelKodSecim1, OzelKodSecim2, OzelKodSecim3, OzelKodSecim4, IskontoOrani5, SiparisYonetimSevk) VALUES ('{HesapKodu}', '{EvrakNo}', 'Stok', '{StokKodu}', '{StokAciklamasi}', {Miktar}, 'ADET', {BirimFiyat}, {BrutTutar}, '{DovizKodu}', {DovizKuru}, 18, {NetTutar}, {IskontoOrani1}, {IskontoOrani2}, {DvzBirimFiyat}, {DvzBrutFiyat}, NULL, '{EvrakNoSiparis}', 'ADET', 1, {SiraNo}, '', '', '', '', 0, NULL, NULL, 0, {NetBirimFiyat}, NULL, NULL, NULL, NULL, {IskontoOrani3}, {IskontoOrani4}, '', '', 0, 0, 0, 0, 0, 0, '', '', '', 0, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, '{UUID}', NULL, '', '', '', '', {IskontoOrani5}, 0); SELECT @@identity; ";

                                query = query.Replace("{HesapKodu}", natraCustomer.Code)
                                    .Replace("{EvrakNo}", natraWaybillNo)
                                    .Replace("{StokKodu}", marsDetailItem.ProductCode)
                                    .Replace("{StokAciklamasi}", marsDetailItem.StokAciklamasi.Replace("'", "''"))
                                    .Replace("{Miktar}", marsDetailItem.Quantity.ToString())
                                    .Replace("{BirimFiyat}", marsDetailItem.BirimFiyat.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{BrutTutar}", marsDetailItem.BrutTutar.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{DovizKodu}", marsDetailItem.DovizKodu) // hesaplanacak
                                    .Replace("{DovizKuru}", marsDetailItem.DovizKuru.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{NetTutar}", marsDetailItem.NetTutar.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{IskontoOrani1}", marsDetailItem.IskontoOrani1.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{IskontoOrani2}", marsDetailItem.IskontoOrani2.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{IskontoOrani3}", marsDetailItem.IskontoOrani3.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{IskontoOrani4}", marsDetailItem.IskontoOrani4.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{IskontoOrani5}", marsDetailItem.IskontoOrani5.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{DvzBirimFiyat}", marsDetailItem.DvzBirimFiyat.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{DvzBrutFiyat}", marsDetailItem.DvzBrutFiyat.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{EvrakNoSiparis}", natraOrderNo)
                                    .Replace("{SiraNo}", c.ToString()) // artacak
                                    .Replace("{NetBirimFiyat}", marsDetailItem.NetBirimFiyat.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{UUID}", marsDetailItem.Natra_IrsaliyeD_UUID);
                                dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);
                                marsDetailItem.Natra_IrsaliyeD_Id = Convert.ToInt32(dt.Rows[0][0]);
                                c++;
                            }



                            // natra cihaz seri numaralarının girilmesi  (seritakip)
                            foreach (var marsDetailItem in marsDetailList)
                            {
                                marsDetailItem.SerialNoProblemList = new List<string>();
                                foreach (string serialNo in marsDetailItem.SerialNoList)
                                {
                                    Mars_SerialNumber ms = new Mars_SerialNumber()
                                    {
                                        SeriNo = serialNo,
                                        StokKodu = marsDetailItem.ProductCode,
                                        EvrakNo = natraWaybillNo,
                                        Tarih = marsWaybillDate, //DateTime.Now,
                                        HesapKodu = natraCustomer.Code,
                                        UUID = marsDetailItem.Natra_IrsaliyeD_UUID,
                                        DepoKodu = natra_SiparisH.Depokodu,
                                    };


                                    // G satırı var mı kontrolü. 

                                    query = "SELECT * FROM seritakibi WHERE SeriNo = '{SeriNo}' AND StokKodu = '{StokKodu}' AND Tipi='G' AND (EvrakNo LIKE 'ITF%' OR EvrakNo LIKE 'AFT%' OR EvrakNo LIKE 'IM%') LIMIT 1";
                                    query = query
                                        .Replace("{SeriNo}", serialNo)
                                        .Replace("{StokKodu}", marsDetailItem.ProductCode);
                                    dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                                    // G satırı var mı kontrolü. 
                                    if (dt.Rows.Count > 0)
                                    {
                                        Natra_Seritakibi natraSeritakibiG = dt.DataTableToItem<Natra_Seritakibi>();

                                        //Ç satırının eklenemesi
                                        query = "INSERT INTO seritakibi (SeriNo, StokKodu, EvrakNo, EvrakTipi, Tarih, DepoKodu, GarantiSure, Tipi, Aciklama, UretimTarihi, SonKullanmaTarihi, Durumu, EvrakNoCikis, CikisID, DepoKodu2, GirisID, Eklendi, HesapKodu, SirketKodu, BirimFiyat, BirimMaliyet, KDV, CUUID, UUID, USER, ComputerName ) VALUES ('{SeriNo}', '{StokKodu}', '{EvrakNo}', NULL, '{Tarih}', '{DepoKodu}', {GarantiSure}, '{Tipi}', NULL, NULL, NULL, {Durumu}, NULL, NULL, NULL, NULL, 0, '{HesapKodu}', COALESCE((SELECT SIRKET_KODU FROM Yonetim.sirket WHERE SIRKET_KODU LIKE 'HNTC%' AND SIRKET_ETKIN=1),''), 0, 0, NULL, '{CUUID}', '{UUID}', '{USER}', NULL); SELECT 1;";
                                        query = query
                                            .Replace("{SeriNo}", natraSeritakibiG.SeriNo)
                                            .Replace("{StokKodu}", natraSeritakibiG.StokKodu)
                                            .Replace("{EvrakNo}", natraWaybillNo)
                                            .Replace("{Tarih}", marsWaybillDate.ToString("yyyy-MM-dd")) // DateTime.Now.ToString("yyyy-MM-dd"))
                                            .Replace("{DepoKodu}", natra_SiparisH.Depokodu)
                                            .Replace("{GarantiSure}", natraSeritakibiG.GarantiSure.ToString())
                                            .Replace("{Tipi}", "Ç")
                                            .Replace("{Durumu}", "0")
                                            .Replace("{HesapKodu}", natraCustomer.Code)
                                            .Replace("{CUUID}", natraSeritakibiG.CUUID)
                                            .Replace("{UUID}", marsDetailItem.Natra_IrsaliyeD_UUID)
                                            .Replace("{USER}", "ERYAZ");
                                        dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                                        // G satırının güncellenmesi
                                        query = "UPDATE seritakibi SET EvrakNoCikis='{EvrakNoCikis}', Eklendi=0 WHERE ID={ID}";
                                        query = query
                                            .Replace("{EvrakNoCikis}", natraWaybillNo)
                                            .Replace("{ID}", natraSeritakibiG.ID.ToString());
                                        dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                                        ms.IsProcessed = true;
                                    }
                                    // G satırı yoksa çıkışı yapılmayacak.
                                    else
                                    {
                                        marsDetailItem.SerialNoProblemList.Add(serialNo);
                                        ms.IsProcessed = false;
                                    }

                                    // gelen seri numaraların b4b dbsine kayıt edilmesi
                                    ms.Save();
                                }
                            }

                            // natra stok_d tablosunun güncellenemsi
                            foreach (var marsDetailItem in marsDetailList)
                            {
                                query = "INSERT INTO stok_d(HesapKodu, StokKodu, BelgeNo, Aciklama, GirenMiktar, CikanMiktar, DepoKodu, DovizKodu, DovizKuru, NetFiyat, Tarih, IslemTipi, EvrakNoIrsaliye, EvrakNoFatura, ModulID, OlcuBirimi, BrutFiyat, KDVTutar, KDV, KDVDahil, HareketTipi, IskontoTutar, Tipi, Kalan, MaliyetTipi, BirimMaliyet, BirimMaliyet2, Sira, Sifir, RenkKodu, BedenKodu, GirisRetMiktar, GirisNumuneMiktar, GirisNumuneHataliMiktar, Onay, KaliteDegerlendirme, OnayDurumu, KaliteOnayTarihi, Okuma, OkumaTarihi, PartiNo, ModulID2, EvrakNoRecete, BirimCarpan, OnayDurumu2, EvrakNoServisCagrisi, HatirlatmaYapildi, MasrafKodu, EtiketKontrol, GozleKontrol, KontrolRaporu, UretimTipi, MarkaKodu, Seri, UUID, DovizKoduMaliyet, BirimMaliyetDoviz, SubeKodu, ProjeKodu, KaliteHareket, ProjeStokCikis, AmortismanOran, GenelGiderOran, NakliyeOran, ArtiMaliyetOran, FireOran, DovizKoduEvrak, DovizKuruEvrak, ReceteOlcuBirimi, ReceteCarpan, ReceteDegeri, StokKoduRecete, MUSER, USER, MRECDATE, RECDATE, PartiMiktar) VALUES ('{HesapKodu}', '{StokKodu}', '{BelgeNo}', 'İrsaliyemiz', 0, {CikanMiktar}, '{DepoKodu}', '{DovizKodu}', {DovizKuru}, {NetFiyat}, '{Tarih}', 'Satış İrsaliye', '{EvrakNoIrsaliye}', '', {ModulID}, '{OlcuBirimi}', {BrutFiyat}, {KDVTutar}, {KDV}, '{KDVDahil}', -1, {IskontoTutar}, 'S', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, NULL, 0, 0, 0, 0, NULL, 'A', NULL, NULL, NULL, '', NULL, 0, 0, 0, 0, 0, 0, 0, '{DovizKoduEvrak}', {DovizKuruEvrak}, NULL, 1, 1, NULL, NULL, '{USER}', NULL, '{RECDATE}', 0); SELECT 1;";
                                query = query.Replace("{HesapKodu}", natraCustomer.Code)
                                    .Replace("{StokKodu}", marsDetailItem.ProductCode)
                                    .Replace("{BelgeNo}", marsWaybillNo)
                                    .Replace("{CikanMiktar}", marsDetailItem.Quantity.ToString()) // adet gelecek
                                    .Replace("{DepoKodu}", natra_SiparisH.Depokodu)
                                    .Replace("{DovizKodu}", marsDetailItem.DovizKodu)
                                    .Replace("{DovizKuru}", marsDetailItem.DovizKuru.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{NetFiyat}", marsDetailItem.NetBirimFiyat.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{Tarih}", marsWaybillDate.ToString("yyyy-MM-dd")) // DateTime.Now.Date.ToString("yyyy-MM-dd"))
                                    .Replace("{EvrakNoIrsaliye}", natraWaybillNo)
                                    .Replace("{ModulID}", marsDetailItem.Natra_IrsaliyeD_Id.ToString())
                                    .Replace("{OlcuBirimi}", marsDetailItem.OlcuBirimi)
                                    .Replace("{BrutFiyat}", marsDetailItem.BrutTutar.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{KDVTutar}", marsDetailItem.KdvTutar.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{KDV}", marsDetailItem.KDV.ToString())
                                    .Replace("{KDVDahil}", "H")
                                    .Replace("{IskontoTutar}", marsDetailItem.IskontoTutar.ToString("0.000000", CultureInfo.InvariantCulture))
                                    .Replace("{DovizKoduEvrak}", natra_SiparisH.DovizKodu)
                                    .Replace("{DovizKuruEvrak}", natra_SiparisH.DovizKuru.ToString("0.0000", CultureInfo.InvariantCulture))
                                    .Replace("{USER}", "ERYAZ")
                                    .Replace("{RECDATE}", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                                dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);
                            }



                            // natra satış siparişinin güncellenmesi 
                            foreach (var marsDetailItem in marsDetailList.Where(p => p.NatraSiparisD != null))
                            {
                                query = "UPDATE siparis_d SET EvrakNoIrsaliye='{EvrakNoIrsaliye}', Kalan={Kalan} WHERE EvrakNo='{EvrakNo}' AND HesapKodu='{HesapKodu}' AND StokKodu='{StokKodu}'";
                                query = query.Replace("{EvrakNo}", natraOrderNo)
                                    .Replace("{HesapKodu}", natraCustomer.Code)
                                    .Replace("{StokKodu}", marsDetailItem.ProductCode)
                                    .Replace("{EvrakNoIrsaliye}", natraWaybillNo)
                                    .Replace("{Kalan}", (marsDetailItem.Quantity >= marsDetailItem.NatraSiparisD.Kalan ? 0 : marsDetailItem.NatraSiparisD.Kalan - marsDetailItem.Quantity).ToString());
                                dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);
                            }
                        }

                        // okunan dosyayı arşiv klasörü içine taşı
                        FtpHelper.RelocateFile(fileUri, fileUriArchive);

                        // Bilgilendirme epostası gönderilmesi
                        {
                            string body = "Merhaba,</br>";
                            body += "<b>" + natraWaybillNo + "</b> irsaliye numaralı <b>" + natraCustomer.Code + " " + natraCustomer.Name + "</b> carisine ait MARS sevkiyat entegrasyonunuz tamamlanmıştır. Natra üzerinden faturala işlemini gerçekleştirebilirsiniz.</br></br>";

                            body += "<table>";
                            body += "<tr>";
                            body += "<th>Stok Kodu</th>";
                            body += "<th>Stok Açıklama</th>";
                            body += "<th>Miktar</th>";
                            body += "</tr>";
                            foreach (var item in marsDetailList)
                            {
                                body += "<tr>";
                                body += "<td>" + item.ProductCode + "</td>";
                                body += "<td>" + item.StokAciklamasi + "</td>";
                                body += "<td>" + item.Quantity + "</td>";
                                body += "</tr>";
                            }
                            body += "</table></br></br>";


                            // seritakibi girilemeyen satırların bilgilendirme maillerinin gönderilmesi.
                            if (marsDetailList.Any(p => p.SerialNoProblemList.Count > 0))
                            {
                                body += "Entegrasyon sırasında bazı kalemlerdeki seri numaraların giriş kayıtları bulunamadığı için işleme işlemi yapılamamıştır. Bilgiler aşağıda listelenmiştir.</br></br>";
                            }

                            body += "<table>";
                            body += "<tr>";
                            body += "<th>Stok Kodu</th>";
                            body += "<th>Stok Açıklama</th>";
                            body += "<th>Sevk Edilen Seri Numaralar</th>";
                            body += "<th>İşlenemeyen Seri Numaralar</th>";
                            body += "</tr>";
                            foreach (var item in marsDetailList)
                            {
                                body += "<tr>";
                                body += "<td>" + item.ProductCode + "</td>";
                                body += "<td>" + item.StokAciklamasi + "</td>";
                                body += "<td>" + string.Join(", ", item.SerialNoList) + "</td>";
                                body += "<td>" + string.Join(", ", item.SerialNoProblemList) + "</td>";
                                body += "</tr>";
                            }
                            body += "</table>";


                            MailMessage mail = new MailMessage();
                            mail.To.Add("satis@hantech.com.tr");
                            mail.To.Add("o.kokel@hantech.com.tr");
                            mail.To.Add("s.ince@hantech.com.tr");
                            //mail.CC.Add("murat.akcura@eryaz.net");

                            mail.Body = body;
                            mail.IsBodyHtml = true;
                            mail.Subject = "[B2B] MARS Sevkiyat Tamamlandı " + natraCustomer.Code + " " + natraWaybillNo;



                            Tuple<bool, string> resultEmial = EmailHelper.Send(mail);

                        }



                        // logla 
                        Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Information, v4.Models.Log.ClientType.B2BWeb, "timer_mars_from", "OK: " + natraOrderNo, string.Empty);
                    }
                    else
                    {
                        // Natrada bu sipariş bulanamadığı için işlem yapılmıyor. 

                        // logla 
                        Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Information, v4.Models.Log.ClientType.B2BWeb, "timer_mars_from", "NOTFOUND: " + natraOrderNo, string.Empty);
                    }

                }
            }


            return null;
        }
    }
}