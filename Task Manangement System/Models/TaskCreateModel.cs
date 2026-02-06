using System.ComponentModel.DataAnnotations;

namespace Task_Manangement_System.Models
{
    public class TaskCreateRequestModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        // 🔹 update fields (nullable allowed)
        public string UpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public TaskCreateRequestModel()
        {
            CreatedBy = "Admin User";
        }
    }

    public class TaskCreateResponseModel
    {
    }
}
