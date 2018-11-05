using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
          
            
            #region 定時觸發
            ////https://codertw.com/前端開發/259860/
            ////https://hk.saowen.com/a/dc79b033b235d1d55c38282ba657c81a4e53bc69f9dec11c3df19aa2f7ca3d05
            ////定義定時器
            ////1000表示1秒的意思
            //System.Timers.Timer myTimer = new System.Timers.Timer(1000);
            ////TaskAction.SetContent 表示要調用的方法
            //myTimer.Elapsed += new System.Timers.ElapsedEventHandler(TaskAction.SetContent);
            //myTimer.Enabled = true;
            //myTimer.AutoReset = true;
            #endregion

        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpCookie MyLang = Request.Cookies["MyLang"];
            if (MyLang != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo(MyLang.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                    new System.Globalization.CultureInfo(MyLang.Value);
            }
        }

        #region 定時觸發

        //void Session_End(object sender, EventArgs e)
        //{
        //    //下面的代碼是關鍵，可解決IIS應用進程池自動回收的問題
        //    System.Threading.Thread.Sleep(1000);
        //    //觸發事件, 寫入提示信息
        //    TaskAction.SetContent();
        //    //這裏設置你的web地址，可以隨便指向你的任意一個aspx頁面甚至不存在的頁面，目的是要激發Application_Start
        //    //使用您自己的URL
        //    string url = "http://localhost:1698";
        //    System.Net.HttpWebRequest myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
        //    System.Net.HttpWebResponse myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();
        //    System.IO.Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回寫的字節流

        //    // 在會話結束時運行的代碼。
        //    // 注意: 只有在 Web.config 文檔中的 sessionstate 模式設置為 InProc 時，才會引發 Session_End 事件。
        //    // 如果會話模式設置為 StateServer
        //    // 或 SQLServer，則不會引發該事件。
        //}

        //public static class TaskAction
        //{
        //    private static string content = "";
        //    /// <summary>
        //    /// 輸出信息存儲的地方.
        //    /// </summary>
        //    public static string Content
        //    {
        //        get { return TaskAction.content; }
        //        set { TaskAction.content += "<div>" + value + "</div>"; }
        //    }

        //    /// <summary>
        //    /// 定時器委託任務 調用的方法
        //    /// </summary>
        //    /// <param name="source"></param>
        //    /// <param name="e"></param>
        //    public static void SetContent(object source, ElapsedEventArgs e)
        //    {
        //        //if (DateTime.Now.ToString("HH:mm:ss") == "11:17:00")
        //        //{
        //        //    //這裏寫你定時執行的代碼
        //        //    Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "下班了！");
        //        //}
        //    }
        //    /// <summary>
        //    /// 應用池回收的時候調用的方法
        //    /// </summary>
        //    public static void SetContent()
        //    {
        //        Content = "END: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    }
        //}

        #endregion

    }
}
