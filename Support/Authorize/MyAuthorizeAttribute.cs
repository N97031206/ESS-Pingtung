using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Support.Authorize
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
    }

    /// <summary>
    /// 角色類型
    /// </summary>
    public enum RoleType
    {
        系統管理員 = 0,
        一般使用者 = 1,
        參觀帳號 = 2
    }




}