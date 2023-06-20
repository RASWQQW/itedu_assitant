namespace itedu_assitant.Model.Base
{
    public class Groups
    {
        public int id { get; set; }
        public string name { get; set; }
        public string group_id { get; set; }
        public string group_link { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now.Date.ToUniversalTime();

       
    }
}
