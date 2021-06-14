using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using B2b.Web.v4.Models.EntityLayer;
using Quartz;
using Quartz.Impl;

namespace B2b.Web.v4.Areas.Admin.Models
{
    public class BistCurrecyUpdate
    {
        static IScheduler scheduler;
        public static void Start()
        {
            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;

            if (!scheduler.IsStarted)
                scheduler.Start().Wait();
            IJobDetail process = JobBuilder.Create<UpdateBistCurrency>().Build();
            ITrigger tetikle = TriggerBuilder.Create().WithDailyTimeIntervalSchedule
            (s =>
             s.WithIntervalInHours(24)
            .OnEveryDay()
            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(07, 05)))
            .Build();
            scheduler.ScheduleJob(process, tetikle);
            
        }
    }
    
}
public class UpdateBistCurrency : IJob
{


    public Task Execute(IJobExecutionContext context)
    {
        var curencyList = Currency.GetList().Where(x => x.CheckBist && x.Type != "TL");
        var xmlDoc = new XmlDocument();
        xmlDoc.Load("http://www.tcmb.gov.tr/kurlar/today.xml");
        foreach (var currency in curencyList)
        {
            string value = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + currency.Type + "']/ForexSelling").InnerXml.Replace(".", ",");
            currency.Rate = Convert.ToDouble(value);
            currency.AddOrUpdate();
        }
        
        return null;

    }

}


