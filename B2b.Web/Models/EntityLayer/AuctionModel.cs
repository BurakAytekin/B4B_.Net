using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class AuctionModel : DataAccess
    {
        public double ProductPrice { get; set; }
        public int TimeRemaining
        {
            get
            {
                return (int)Math.Abs(GetTimeRemaining().TotalSeconds);
            }
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        //public DateTime LastDate { get; set; }
        public double MinPrice { get; set; }
        public double IncrementAmountPerBid { get; set; }
        public bool ShowBidCustomer { get; set; }
        public int BidsTotal { get; set; }
        public DateTime LastBid { get; set; }

        public double ValueLastBid { get; set; }
        public double ValueNextBid { get { return ValueLastBid + IncrementAmountPerBid; } }
        public string LastUserBid { get; set; }
        public double Quantity { get; set; }
        public Product Product { get; set; }
        public string EndTimeFullText
        {
            get { return string.Format("{0:MM/dd/yyyy HH\\:mm\\:ss}", LastBid); }
        }

        public AuctionModel() { }

        public AuctionModel(AuctionModel auctionModel)
        {
            BidsTotal = 0;
            IncrementAmountPerBid = auctionModel.IncrementAmountPerBid;
            LastBid = DateTime.Now;
            StartDate = auctionModel.StartDate;
            //LastDate = auctionModel.EndDate;
            ValueLastBid = auctionModel.MinPrice;
            EndTime = auctionModel.EndDate;
        }

        //public TimeSpan GetTimeElapsed()
        //{
        //    return DateTime.Now.Subtract(LastBid);
        //}

        public TimeSpan GetTimeRemaining()
        {
            return DateTime.Now.Subtract(EndTime);
        }

        //public void SetEndTime()
        //{
        //    EndTime = LastBid.Add(GetTimeElapsed().Add(TimeSpan.FromSeconds(IncrementAmountPerBid)));
        //}

        public void PlaceBid(double valueLastBid, Customer lastUserBid)
        {
            ValueLastBid = valueLastBid;
            LastUserBid = lastUserBid.Users.Code;
            BidsTotal++;
            LastBid = DateTime.Now;
            //SetEndTime();
            // Arttırma log detayı kayıt

        }

        #region Methods

        public static AuctionModel GetAuction()
        {
            AuctionModel item = new AuctionModel();
            DataTable dt = DAL.GetAuction();

            foreach (DataRow row in dt.Rows)
            {
                item.Id = row.Field<int>("Id");
                item.StartDate = row.Field<DateTime>("StartDate");
                item.EndDate = row.Field<DateTime>("EndDate");
                item.Quantity = row.Field<double>("Quantity");
                item.MinPrice = row.Field<double>("MinPrice");
                item.IncrementAmountPerBid = row.Field<double>("IncrementAmountPerBid");
                item.ShowBidCustomer = row.Field<bool>("ShowBidCustomer");
                item.ProductId = row.Field<int>("ProductId");
                item.Product = new Product()
                {
                    Id = row.Field<int>("ProductId"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    Manufacturer = row.Field<string>("Manufacturer"),
                    ManufacturerCode = row.Field<string>("ManufacturerCode"),
                    PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath")
                };
            }

            return item;

        }


        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetAuction()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Auction", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
    }
}