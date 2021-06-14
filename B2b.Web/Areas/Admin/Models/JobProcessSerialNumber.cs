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
    public class JobProcessSerialNumber : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {

            List<Mars_SerialNumber> serialNoList = Mars_SerialNumber.GetListUnprocessed();

            // natra cihaz seri numaralarının girilmesi  (seritakip)

            foreach (Mars_SerialNumber serialObj in serialNoList)
            {
                // G satırı var mı kontrolü. 

                string query = "SELECT * FROM seritakibi WHERE SeriNo = '{SeriNo}' AND StokKodu = '{StokKodu}' AND Tipi='G' AND (EvrakNo LIKE 'ITF%' OR EvrakNo LIKE 'AFT%' OR EvrakNo LIKE 'IM%') LIMIT 1";
                query = query
                    .Replace("{SeriNo}", serialObj.SeriNo)
                    .Replace("{StokKodu}", serialObj.StokKodu);
                DataTable dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                // G satırı var mı kontrolü. 
                if (dt.Rows.Count > 0)
                {
                    Natra_Seritakibi natraSeritakibiG = dt.DataTableToItem<Natra_Seritakibi>();

                    //Ç satırının eklenemesi
                    query = "INSERT INTO seritakibi (SeriNo, StokKodu, EvrakNo, EvrakTipi, Tarih, DepoKodu, GarantiSure, Tipi, Aciklama, UretimTarihi, SonKullanmaTarihi, Durumu, EvrakNoCikis, CikisID, DepoKodu2, GirisID, Eklendi, HesapKodu, SirketKodu, BirimFiyat, BirimMaliyet, KDV, CUUID, UUID, USER, ComputerName ) VALUES ('{SeriNo}', '{StokKodu}', '{EvrakNo}', NULL, '{Tarih}', '{DepoKodu}', {GarantiSure}, '{Tipi}', NULL, NULL, NULL, {Durumu}, NULL, NULL, NULL, NULL, 0, '{HesapKodu}', COALESCE((SELECT SIRKET_KODU FROM Yonetim.sirket WHERE SIRKET_KODU LIKE 'HNTC%' AND SIRKET_ETKIN=1),''), 0, 0, NULL, '{CUUID}', '{UUID}', '{USER}', NULL); SELECT 1;";
                    query = query
                        .Replace("{SeriNo}", natraSeritakibiG.SeriNo)
                        .Replace("{StokKodu}", natraSeritakibiG.StokKodu)
                        .Replace("{EvrakNo}", serialObj.EvrakNo)
                        .Replace("{Tarih}", serialObj.Tarih.ToString("yyyy-MM-dd")) // DateTime.Now.ToString("yyyy-MM-dd"))
                        .Replace("{DepoKodu}", serialObj.DepoKodu)
                        .Replace("{GarantiSure}", natraSeritakibiG.GarantiSure.ToString())
                        .Replace("{Tipi}", "Ç")
                        .Replace("{Durumu}", "0")
                        .Replace("{HesapKodu}", serialObj.HesapKodu)
                        .Replace("{CUUID}", natraSeritakibiG.CUUID)
                        .Replace("{UUID}", serialObj.UUID)
                        .Replace("{USER}", "ERYAZ");
                    dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                    // G satırının güncellenmesi
                    query = "UPDATE seritakibi SET EvrakNoCikis='{EvrakNoCikis}', Eklendi=0 WHERE ID={ID}";
                    query = query
                        .Replace("{EvrakNoCikis}", serialObj.EvrakNo)
                        .Replace("{ID}", natraSeritakibiG.ID.ToString());
                    dt = ErpHelper.FireServiceMethodForMarsIntegrations(query);

                    serialObj.IsProcessed = true;
                    serialObj.Update();
                }
                // G satırı yoksa çıkışı yapılmayacak.
                else
                {
                  // işlem yok.
                }
               
            }


            // logla 
            //Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Information, v4.Models.Log.ClientType.B2BWeb, "process_serial", "OK: " + natraOrderNo, string.Empty);


            return null;
        }
    }
}