namespace itedu_assitant.Model.ForvView
{
    public class ContentDelivery
    {
        public string type { get; set; } // json, form
        public Dictionary<string, object> jsoncontent { get; set; } = null;
        public Dictionary<string, object> formcontent { get; set; } = null;
    }
}
