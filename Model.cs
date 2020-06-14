using System;
using System.Collections.Generic;

namespace todo_web
{
    public class Const
    {
        // 設定在Cookies的 Session 名稱
        static public string SESSION = "sid";

        // 解析Session，取出登入者ID
        static public int ParseUID(string session)
        {
            int uid = 0; // 預設登入者ID = 0
            try
            {
                uid = Int32.Parse(session); // 試著把session轉成數字
            }
            catch (System.Exception) // 如果轉成數字失敗，catch錯誤，不要導致伺服器壞掉
            {
            }

            return uid;
        }
    }

    // 帳戶物件
    public class Account
    {
        public string username { get; set; }         // 帳號
        public int id; // ID
        static int count; // 紀錄目前使用者數量
        static public Dictionary<int, Account> users = new Dictionary<int, Account>(); // 帳戶資料庫(字典)

        // 產生新的ID當作uid
        static int NewID()
        {
            return ++Account.count;
        }

        // 建立新帳戶
        static public Account New(string username)
        {
            Account account = GetAccountByName(username); // 用帳號取帳戶資料
            if (account.id > 0) // 如果已經存在這個帳號，就直接回傳該帳戶資料
            {
                return account;
            }


            // 如果這個帳號還沒有使用過，則創建一個信帳戶資料
            Account newAccount = new Account
            {
                id = NewID(),
                username = username,
            };

            users.Add(newAccount.id, newAccount); // 新創帳戶加到帳戶資料庫(字典)
            return newAccount; // 並且回傳帳戶資料
        }

        // 用帳號取帳戶資料
        static public Account GetAccountByName(string username)
        {
            foreach (KeyValuePair<int, Account> user in users)
            {
                // 檢查帳號有沒有存在資料庫，有就回傳
                if (user.Value.username == username)
                {
                    return user.Value;
                }
            }

            // 帳號不存在則回傳預設
            return new Account();
        }

        // 用uid取帳戶資料
        static public Account GetAccountByID(int id)
        {
            // 用uid從帳戶資料庫(字典)取出帳戶資料，若不存在回傳預設帳戶
            return users.GetValueOrDefault(id, new Account());
        }
    }

    // 待辦事項
    public class Todo
    {
        public string title { get; set; }         // 標題
        public int id { get; set; }             // ID
        public bool completed { get; set; }          // 完成否
        static int count; // 紀錄目前todo數量
        static public Dictionary<int, List<Todo>> todos = new Dictionary<int, List<Todo>>(); // Todo資料庫

        // 產生新的ID當作uid
        static int NewID()
        {
            return ++Todo.count;
        }

        static public List<Todo> New(int uid, string title)
        {
            Todo t = new Todo // 建立ToDo物件
            {
                title = title,
                id = NewID(),
                completed = false
            };

            List<Todo> l = todos.GetValueOrDefault(uid, new List<Todo>()); // 取指定uid的todo清單, 若不存在資料庫則建立預設空陣列
            l.Add(t); // 把新的清單加入清單

            if (!todos.ContainsKey(uid)) // 若指定uid的todo清單不存在資料庫，
            {
                todos.Add(uid, l); // 則把新的todo物件加入資料庫中
            }

            return todos[uid]; // 回傳指定uid的todo清單
        }

        // 取指定uid的todo清單
        static public List<Todo> GetUserTodo(int uid)
        {
            // 從資料庫取指定uid的todo清單，若uid不存在，則回傳預設空清單
            return todos.GetValueOrDefault(uid, new List<Todo>());
        }

        // 編輯指定uid的待辦事項
        static public List<Todo> EditTodo(int uid, Todo editTodo)
        {

            List<Todo> userTodos = GetUserTodo(uid); // 先取指定uid的todo清單
            int index = userTodos.FindIndex(0, userTodos.Count, todo => todo.id == editTodo.id); // 找出要修改的todo在todo清單中的位子
            if (index >= 0) // 位子若 >= 0 則表示有在清單中
            {
                userTodos[index] = editTodo; // 把要修改的todo，覆蓋到指定位子的todo
            }

            return userTodos; // 回傳使用者的todo清單
        }

        // 移除指定uid的todo
        static public List<Todo> RemoveTodo(int uid, int id)
        {

            List<Todo> userTodos = GetUserTodo(uid); // 先取指定uid的todo清單
            int index = userTodos.FindIndex(0, userTodos.Count, todo => todo.id == id);// 找出要刪除的todo在todo清單中的位子
            if (index >= 0)
            {
                userTodos.RemoveAt(index); // 刪除上面找到的位子的todo物件
            }

            return userTodos; // 回傳使用者的todo清單
        }

        // 移除指定uid的todo清單裡的「已完成」待辦事項
        static public List<Todo> RemoveCompletedTodo(int uid)
        {

            List<Todo> userTodos = GetUserTodo(uid); // 先取指定uid的todo清單
            List<Todo> activeTodos = new List<Todo>(); // 先額外建立一組空的todo清單，後續用來存還在「執行中的todo」

            foreach (Todo todo in userTodos) // 開始跑指定uid的todo清單裡的每個todo
            {
                if (!todo.completed) // 如果todo還沒完成，則加到activeTodos中
                {
                    activeTodos.Add(todo);
                }
            }
            todos[uid] = activeTodos; // 把activeTodos塞回資料庫，覆蓋uid的todo清單

            return activeTodos;
        }
    }
}
