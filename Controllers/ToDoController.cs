using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace todo_web.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        // 取登入者ID
        private int getUID()
        {
            int uid = 0;
            if (Request.Cookies.ContainsKey(Const.SESSION)) // 如果Cookie中有這個session，
            {
                uid = Const.ParseUID(Request.Cookies[Const.SESSION]); // 解析Cookie中的Session，取出uid
            }

            return uid; // 回傳uid
        }

        // GET api/todo
        [HttpGet] // 定義todo清單的api的路由
        public IEnumerable<Todo> Get() // 取todo清單的功能
        {
            int uid = this.getUID(); // 登入者ID

            return Todo.GetUserTodo(uid); // 回傳登入者的todo清單
        }

        // POST api/todo
        [HttpPost] // 定義建立todo的api的路由
        public IEnumerable<Todo> Post([FromBody] string title) // title = 請求資料，新增todo的功能
        {
            int uid = this.getUID(); // 登入者ID

            List<Todo> l = Todo.New(uid, title); // 新增todo，並且回傳登入者的todo清單

            return l; // 回傳todo清單
        }

        // PUT api/todo/5
        [HttpPut("{id}")] // 定義編輯todo的api的路由
        public IEnumerable<Todo> Put(int id, [FromBody] Todo todo) // id = todo的ID, todo = 編輯資料，編輯todo的功能
        {
            int uid = this.getUID(); // 登入者ID
            return Todo.EditTodo(uid, todo); // 編輯Todo並且回傳todo清單
        }

        // DELETE api/todo/completed
        [HttpDelete("completed")] // 定義刪除「已完成」的todo的api的路由
        public IEnumerable<Todo> DeleteCompleted() // 刪除「已完成」todo的功能
        {
            int uid = this.getUID(); // 登入者ID
            return Todo.RemoveCompletedTodo(uid); // 刪除todo
        }

        // DELETE api/todo/5
        [HttpDelete("{id}")] // 定義刪除「指定」的todo的api的路由
        public IEnumerable<Todo> Delete(int id) // 刪除「指定」todo的功能
        {
            int uid = this.getUID(); // 登入者ID
            return Todo.RemoveTodo(uid, id);  // 刪除todo
        }
    }
}
