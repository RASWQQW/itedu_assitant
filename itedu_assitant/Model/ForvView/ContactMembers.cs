using itedu_assitant.Model.Base;

namespace itedu_assitant.Model.ForvView
{
    public class ContactMembers
    {
        public int? groupId { get; set; } = null;
        public bool admininc { get; set; } = false;
        public List<Contact>? contacts { get; set; } = null;
    }
}
