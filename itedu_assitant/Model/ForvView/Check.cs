using System.ComponentModel.DataAnnotations;

namespace itedu_assitant.Model.ForvView
{
    public class Check
    {
        [Required]
        public string groupname { get; set; }
        [Required]
        public List<string> users { get; set; }
        public List<string>? admins { get; set; } = null;
        public IFormFile? group_image { get; set; } = null;
        public string Hello_Message { get; set; } = null;
        public bool Save_Contacts { get; set; } = true;
        public bool Save_Admin_Contacts { get; set; } = true;
    }
}
