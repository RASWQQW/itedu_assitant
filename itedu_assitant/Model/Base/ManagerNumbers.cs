namespace itedu_assitant.Model.Base
{
    public class ManagerNumbers
    {
        public int id { get; set; }
        public string phoneNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date.ToUniversalTime();
    }
}
