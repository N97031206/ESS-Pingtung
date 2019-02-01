using Owin;
using System;
using System.Diagnostics;

//[assembly: OwinStartup(typeof(Startup))]

namespace Web
{
    public class Startup
    {
        #region 定時觸發       
        //https://hk.saowen.com/a/c5e3fa0a7bb568fe0d47384f61b7a537fdbbdd0cb7fcd57d5c10e70c0cf45732
        //Hangfire的配置     
        public void Configuration(IAppBuilder app)
        {
            //// 指定Hangfire使用記憶體儲存任務
            //GlobalConfiguration.Configuration
            //    .UseSqlServerStorage("ESSContext")
            //    .UseMemoryStorage();
            //// 啟用HanfireServer
            //app.UseHangfireServer();
            //// 啟用Hangfire的Dashboard
            //app.UseHangfireDashboard();

            //var jobId = BackgroundJob.Enqueue(() => WriteLog("隊列任務"));
            //RecurringJob.AddOrUpdate(() => WriteLog("每分鐘執行任務"), Cron.Minutely); //注意最小單位是分鐘

        }

        //public void WriteLog(string msg)
        //{
        //    Debug.WriteLine($"Hangfire於{DateTime.Now}執行了任務[{msg}]");
        //}
        #endregion
    }
}