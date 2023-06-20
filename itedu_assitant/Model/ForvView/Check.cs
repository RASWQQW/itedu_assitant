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
    }
}
