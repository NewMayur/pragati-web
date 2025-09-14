using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Pragati.Web.Models;

namespace Pragati.Web.Controllers
{
    public class TasksController : Controller
    {
        private string connStr = ConfigurationManager.ConnectionStrings["ToDoConn"].ConnectionString;

        public ActionResult Index()
        {
            EnsureTableExists();
            var tasks = LoadTasks();
            return View(tasks);
        }

        [HttpPost]
        public ActionResult Create(string task)
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

        private List<Task> LoadTasks()
        {
            var tasks = new List<Task>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Id, TaskName FROM dbo.Tasks ORDER BY Id DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    tasks.Add(new Task { Id = (int)row["Id"], TaskName = row["TaskName"].ToString() });
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
