namespace itedu_assitant.Model.Base
{
    public class Instance
    {
        public int id { get; set; }
        public string idInstance { get; set; }
        public string ApiToken { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date.ToUniversalTime();

    }
}
