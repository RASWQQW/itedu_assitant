using System.ComponentModel;

namespace itedu_assitant.Model.Base
{
    public class Users
    {
        public int id { get; set; }
        public string userName { get; set; }               
        public string userPhoneNumber { get; set; }
        public Groups userGroup { get; set; }
        public string userStatus { get; set; }
        [DefaultValue(value:false)]
        public bool hasContact { get; set; } = false;
    }
}
