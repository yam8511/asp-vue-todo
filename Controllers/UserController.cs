using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace todo_web.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        // GET api/user/current
        [HttpGet("current")] // 定義登入者api的路由
        public Account CurrentUser() // 取登入者帳戶資料
        {
            int uid = 0;
            if (Request.Cookies.ContainsKey(Const.SESSION)) // 如果Cookie中有這個session，
            {
                uid = Const.ParseUID(Request.Cookies[Const.SESSION]);// 解析Cookie中的Session，取出uid
            }
            return Account.GetAccountByID(uid); // 回傳指定uid的帳戶資料
        }


        // POST api/user/register
        [HttpPost("register")] // 定義註冊api的路由
        public Account Register([FromBody] string username) // 註冊功能
        {
            Account.New(username); // 建立新帳戶
            return this.Login(username); // 註冊完自動登入
        }

        // POST api/user/login
        [HttpPost("login")] // 定義登入api的路由
        public Account Login([FromBody] string username) // 登入功能
        {
            Account user = Account.GetAccountByName(username); // 使用帳號取帳戶資料
            Response.Cookies.Append(Const.SESSION, user.id.ToString()); // 然後把帳戶的ID寫入到Cookie中，以提供後續驗證用
            return user; // 回傳帳戶資料
        }


        // DELETE api/user/logout
        [HttpDelete("logout")] // 定義登出api的路由
        public void Logout()
        {
            Response.Cookies.Delete(Const.SESSION); // 清出Cookie中的Session
        }
    }
}
