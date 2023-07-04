namespace itedu_assitant.Model.ForvView
{
    public class ContentDelivery
    {
        public string type { get; set; } // json, form wether one of them and gets one of it 
        public Dictionary<string, object> jsoncontent { get; set; } = null; // its stands null
        public Dictionary<string, object> formcontent { get; set; } = null; // or it or both stands present
    }
}
