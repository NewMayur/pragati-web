using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Pragati.Web.Models;
using ToDoItem = Pragati.Web.Models.ToDoItem;

namespace Pragati.Web.Controllers
{
    public class HomeController : Controller
    {
        private string connStr = ConfigurationManager.ConnectionStrings["ToDoConn"].ConnectionString;

        public ActionResult Index()
        {
            EnsureTableExists();
            var tasks = LoadTasks();
            return View(tasks);
        }

        [HttpPost]
        public ActionResult Index(string task)
        {
            if (!string.IsNullOrEmpty(task))
            {
                AddTask(task.Trim());
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DeleteTask(id);
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void EnsureTableExists()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string checkAndCreateQuery = @"
                    IF NOT EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.TABLES
                        WHERE TABLE_NAME = 'Tasks' AND TABLE_SCHEMA = 'dbo'
                    )
                    BEGIN
                        CREATE TABLE dbo.Tasks (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            TaskName NVARCHAR(255) NOT NULL
                        )
                    END";
                SqlCommand cmd = new SqlCommand(checkAndCreateQuery, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private List<ToDoItem> LoadTasks()
        {
            var tasks = new List<ToDoItem>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Id, TaskName FROM dbo.Tasks ORDER BY Id DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    tasks.Add(new ToDoItem { Id = (int)row["Id"], TaskName = row["TaskName"].ToString() });
                }
            }
            return tasks;
        }

        private void AddTask(string task)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO dbo.Tasks (TaskName) VALUES (@Task)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Task", task);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteTask(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM dbo.Tasks WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
