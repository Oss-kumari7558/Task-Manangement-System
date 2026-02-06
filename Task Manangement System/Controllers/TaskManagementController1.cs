
using System.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Task_Manangement_System.Models;
using Task_Manangement_System.Views.TaskManagementController1.PartialView.LoadTaskCreateForm;

namespace Task_Manangement_System.Controllers
{
    public class TaskManagementController1 : Controller
    {
        private readonly SqlConnection _sqlConnection;
        private readonly IConfiguration _configuration;
        public TaskManagementController1(SqlConnection sqlConnection, IConfiguration configuration)
        {
            _sqlConnection = sqlConnection;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var tasks = new List<TaskCreateRequestModel>();
            try

            {

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = "SELECT * FROM TasksManagement";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await con.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tasks.Add(new TaskCreateRequestModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    DueDate = reader["DueDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DueDate"]),
                                    Status = reader["Status"].ToString(),
                                    Remarks = reader["Remarks"].ToString(),
                                    UpdatedBy = reader["LastUpdatedBy"].ToString(),
                                    LastUpdatedOn = reader["LastUpdatedOn"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["LastUpdatedOn"]),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    CreatedOn = reader["CreatedOn"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedOn"])
                                });
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
            return View(tasks);
        }
        [HttpPost]

        public async Task<IActionResult> TaskCreate(TaskCreateRequestModel model)
        {
            

            try
            {
                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("SPTaskCreate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Title", model.Title);
                        cmd.Parameters.AddWithValue("@Description", model.Description);
                        cmd.Parameters.AddWithValue("@DueDate", model.DueDate);
                        cmd.Parameters.AddWithValue("@Status", model.Status);
                        cmd.Parameters.AddWithValue("@Remarks", model.Remarks);

                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedById", 1);

                        cmd.Parameters.AddWithValue("@Mode", "create");

                        await con.OpenAsync();
                        var newId = await cmd.ExecuteScalarAsync();

                        return Json(new { success = true, taskId = newId });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        public async Task<IActionResult> LoadTaskForm(int id)
        {
            var tasks = new LoadTaskCreateFormPageModel();

            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("SPTaskCreate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Mode", "getall");

                        await con.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tasks.Id = Convert.ToInt32(reader["Id"]);
                                tasks.Title = reader["Title"].ToString();
                                tasks.Description = reader["Description"].ToString();
                                tasks.DueDate = reader["DueDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DueDate"]);
                                tasks.Status = reader["Status"].ToString();
                                tasks.Remarks = reader["Remarks"].ToString();
                                tasks.UpdatedBy = reader["LastUpdatedBy"].ToString();
                                tasks.LastUpdatedOn = reader["LastUpdatedOn"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["LastUpdatedOn"]);
                                tasks.CreatedBy = reader["CreatedBy"].ToString();
                                tasks.CreatedOn = reader["CreatedOn"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedOn"]);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
            return View("~/Views/TaskManagementController1/PartialView/LoadTaskCreateForm/LoadTaskCreateForm.cshtml", tasks);
        }
        [HttpPost]
        public async Task<IActionResult> TaskFormUpdate(LoadTaskCreateFormPageModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("SPTaskCreate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        cmd.Parameters.AddWithValue("@Title", model.Title);
                        cmd.Parameters.AddWithValue("@Description", model.Description);
                        cmd.Parameters.AddWithValue("@DueDate", model.DueDate);
                        cmd.Parameters.AddWithValue("@Status", model.Status);
                        cmd.Parameters.AddWithValue("@Remarks", model.Remarks);
                        cmd.Parameters.AddWithValue("@LastUpdatedBy", model.UpdatedBy);
                        cmd.Parameters.AddWithValue("@Mode", "update");

                        con.Open();
                        var newId = await cmd.ExecuteScalarAsync();
                        con.Close();

                        return Json(new { success = true, id = newId });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> TaskSearch(string searchText )
        {
            var tasks = new List<TaskCreateRequestModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("SPTaskCreate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@searchText", string.IsNullOrEmpty(searchText) ? null: searchText);
                       

                        cmd.Parameters.AddWithValue("@Mode", "search");

                        await con.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tasks.Add(new TaskCreateRequestModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    DueDate = reader["DueDate"] == DBNull.Value
                                        ? DateTime.MinValue
                                        : Convert.ToDateTime(reader["DueDate"]),
                                    Status = reader["Status"].ToString(),
                                    Remarks = reader["Remarks"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }

            return PartialView("~/Views/TaskManagementController1/PartialView/Search.cshtml", tasks);
        }

        [HttpPost]
        public async Task<IActionResult> TaskDelete(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("SPTaskCreate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Mode", "delete");

                        await con.OpenAsync();
                        var deletedId = await cmd.ExecuteScalarAsync();

                        return Json(new
                        {
                            success = true,
                            deletedTaskId = deletedId
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

    }
}
