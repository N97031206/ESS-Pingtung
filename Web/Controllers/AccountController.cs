using PagedList;
using Service.ESS.Model;
using Service.ESS.Provider;
using Support.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web.Models;
using NLog;
using Support.Authorize;

namespace Web.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        private AccountService accountService = new AccountService();
        private RoleService roleService = new RoleService();
        private OrginService orginService = new OrginService();
        //分頁
        private int PageSizes() { if (!int.TryParse(ConfigurationManager.AppSettings["PageSize"], out int s)) { s = 10; } return s; }
        //Log檔
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        #region 登出入
        [AllowAnonymous]
        public ActionResult Login()
        {
            HttpCookie Cookie = new HttpCookie("MyLang", "zh-TW");
            try
            {
                ViewBag.Logo = ConfigurationManager.AppSettings["LogoInfo"];
                ViewBag.Warning = null;

                if (User.Identity.IsAuthenticated)
                {
                    //取得使用者名稱
                    Char delimiter = ',';
                    string[] x = User.Identity.Name.ToString().Split(delimiter);
                    Session["UserName"] = x[0];
                    return RedirectToAction("Index", "Tab");
                }
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ViewBag.Warning = "請輸入正確格式";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection LogInfo)
        {
            HttpCookie Cookie = new HttpCookie("MyLang", "zh-TW");
            try
            {
                var UserName = LogInfo["UserName"].Trim();
                var Password = LogInfo["Password"].Trim();
                ViewBag.Logo = ConfigurationManager.AppSettings["LogoInfo"];
                ViewBag.Warning = null;

                //驗證資料
                Account account = accountService.ReadBy(UserName, Password);

                if (account == null)//查無資料
                {
                    if (accountService.ReadByName(UserName) == null)
                    {
                        ViewBag.name = "使用者名稱輸入有誤";
                    }
                    else if (accountService.ReadByName(Password) == null)
                    {
                        ViewBag.password = "密碼輸入有誤";
                    }
                    return View();
                }
                else //有資料
                {
                    LoginFormsAuthentication(account, UserName, Password);

                    string ClientIP = Support.Http.IP.GetClientIP(Request);
                    logger.Info("user:"+UserName+";IP:"+ ClientIP);

                  
                    return RedirectToAction("Index", "Tab");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ViewBag.Warning = "請輸入正確格式";
                return View();
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Logout()
        {
            // 登入時清空所有 Session 資料
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }
        /// <summary>
        /// 遺忘密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forget(FormCollection LogInfo)
        {
            var UserName = LogInfo["UserName"].Trim();
            var Email = LogInfo["Email"].Trim();

            #region Task Mail
            Task.Run(() => {
                //設定收件人
                //使用者
                List<forget> userMails = new List<forget>();

                if (!string.IsNullOrEmpty(UserName))
                {
                    string mail = accountService.ReadByName(UserName).Email;
                    if (string.IsNullOrEmpty(mail))
                    {
                        ViewBag.alert = "查無資料";
                    }
                    else
                    {
                        userMails.Add(new forget
                        {
                            UserGuest = accountService.ReadByName(UserName).UserName,
                            UserMail = accountService.ReadByName(UserName).Email,
                            UserPassword = accountService.ReadByName(UserName).Password
                        });
                        ViewBag.alert = "已發郵件";
                    }
                }
                //郵件
                if (!string.IsNullOrEmpty(Email))
                {
                    var mails = accountService.ReadByMail(Email);

                    if (mails.First().UserName.FirstOrDefault() == 0)
                    {
                        ViewBag.alert = "查無資料";
                    }
                    else
                    {
                        foreach (var mail in mails)
                        {
                            userMails.Add(new forget
                            {
                                UserGuest = mail.UserName,
                                UserMail = mail.Email,
                                UserPassword = mail.Password
                            });
                            ViewBag.alert = "已發郵件";
                        }
                    }
                }
                //有無收件人
                if (userMails == null)
                {
                    ViewBag.alert = "使用者名稱輸入有誤";
                }
                else
                {
                    foreach (var mail in userMails)
                    {
                        var subject = ConfigurationManager.AppSettings["MailServer_DisplayName"];
                        var user = mail.UserGuest.Trim();
                        var pass = mail.UserPassword.Trim();
                        var content = "使用者:" + user + ",密碼是:" + pass;
                        Gmail.Send(mail.UserMail.Trim(), subject, content);
                    }
                }
            });
            #endregion

            return RedirectToAction("Login", "Account");
        }

        #endregion

        #region Create
        [Authorize]
        public ActionResult Create(string info)
        {
            TagTitle();
            RoleTypeList();
            OrginTypeList();
            ViewBag.chkUsername = info;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection From)
        {
            var UserName = From["UserName"].Trim();
            var Password = From["Password"].Trim();
            var Tel = From["Tel"].Trim();
            var Email = From["Email"].Trim();
            var Role = Guid.Parse(From["RoleId"].Trim());
            var Orgin = Guid.Parse(From["OrginId"].Trim());
            var CreaterOrgin = From["CreaterOrgin"].Trim();

           //資料庫內無相同名稱
            if (accountService.ReadByName(UserName)==null)
            {
                //無自行新增單位部門
                if (!string.IsNullOrEmpty(CreaterOrgin))
                {
                    Orgin orgin = new Orgin()
                    {
                        OrginName = CreaterOrgin
                    };
                    Orgin = orginService.Create(orgin);
                }

                Account account = new Account()
                {
                    UserName = UserName,
                    Password = Password,
                    Tel = Tel,
                    Email = Email,
                    RoleId = Role,
                    OrginId = Orgin
                };
                Guid accountId = accountService.Create(account);

                return RedirectToAction("List", "Account");
            }
            else
            {
                return RedirectToAction("Create", "Account",new { info = "此名稱有人使用" } );
            }         
        }
        #endregion

        #region  List
        [Authorize]
        public ActionResult List(int page = 1)
        {
            TagTitle();
            var user = User.Identity.Name.Split(',')[0];
            var type= User.Identity.Name.Split(',')[2];
            //依權限取表
            List <Account> accounts = accountService.ReadByType(type, user);

            //分頁
            int currentPage = page < 1 ? 1 : page;
            var result = accounts.ToPagedList(currentPage, PageSizes());

            return View(result);
        }
        #endregion

        #region Edit
        [Authorize]
        public ActionResult Edit(Guid id)
        {
            TagTitle();
            RoleTypeList();
            Account accounts = accountService.ReadByID(id);
            return View(accounts);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection From)
        {
            var id= Guid.Parse(From["id"].Trim());
            var UserName = From["UserName"].Trim();
            var Password = From["Password"].Trim();
            var Tel = From["Tel"].Trim();
            var Email = From["Email"].Trim();
            var Role = Guid.Parse(From["RoleId"].Trim());

            Account account = new Account()
            {
                Id= id,
                UserName = UserName,
                Password = Password,
                Tel = Tel,
                Email = Email,
                RoleId = Role
            };

            Guid accountId = accountService.Update(account);

            return RedirectToAction("List", "Account");
        }
        #endregion

        #region Disable
        [Authorize]
        public ActionResult Disable(Guid id)
        {
            TagTitle();
            RoleTypeList();
            Account accounts = accountService.ReadByID(id);
            return View(accounts);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disable(FormCollection From)
        {
            Guid id = Guid.Parse(From["id"].Trim());
            var Disable = From["Disabled"];

            string[] check = Disable.Split(',');
            bool chk= check[0] == "true"? true: false;

            Account account = new Account()
            {
                Id = id,
                Disabled =chk
            };

            Guid accountId = accountService.Update(account);

            return RedirectToAction("List", "Account");
        }
        #endregion

        #region 身分驗證
                /// <summary>
        /// 身分驗證
        /// </summary>
        /// <param name="account"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void LoginFormsAuthentication(Account account, string username, string password)
        {
            // 登入時清空所有 Session 資料
            Session.RemoveAll();
            Session["UserName"] = username.Trim();
            Session["Type"] = account.Role.Type.ToString();

            FormsAuthenticationTicket ticket =
                new FormsAuthenticationTicket(
                    1,
                     string.Format("{0},{1},{2}",
                     account.UserName, account.Password, account.Role.Type.ToString()),//你想要存放在 User.Identy.Name 的值，通常是使用者帳號
                     DateTime.Now,
                     DateTime.Now.AddMinutes(30),
                     false,//將管理者登入的 Cookie 設定成 Session Cookie                 
                     account.Role.Type.ToString(),//userdata看你想存放啥
                     FormsAuthentication.FormsCookiePath
                     );
            string encTicket = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        #endregion

        #region 其他
        /// <summary>
        /// 查Seesion登入資訊
        /// </summary>
        protected internal void TagTitle()
        {
            ViewBag.Logo = ConfigurationManager.AppSettings["LogoInfo"];
            if (Session["UserName"] != null)
            {
                string un = Session["UserName"].ToString();
                ViewBag.User = string.IsNullOrEmpty(un) ? null : un;
            }
        }

        /// <summary>
        /// 權限列表
        /// </summary>
        private void RoleTypeList()
        {
            List<Role> RT = roleService.ReadAll().ToList();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var rt in RT)
            {
                items.Add(new SelectListItem()
                {
                    Text = rt.Type.ToString(),
                    Value = rt.Id.ToString()
                });
            }
            ViewBag.Role = items;
        }
      
        /// <summary>
        /// 單位列表
        /// </summary>
        private void OrginTypeList()
        {
            List<Orgin> List= orginService.ReadAll().ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            List.ForEach(x=> {
                items.Add(new SelectListItem() 
                {
                    Text = x.OrginName.ToString().Trim(),
                    Value = x.Id.ToString().Trim()
                });
            });
            ViewBag.Orgin = items;
        }
    }
    #endregion

}