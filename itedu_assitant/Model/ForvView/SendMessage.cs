namespace itedu_assitant.Model.ForvView
{

    // This model is a very first model for sending message
    public class SendMessage
    {
        // for who it message goes
        public string Chat_id { get; set; }
        // property of self message
        public string? Message { get; set; } = null;
        public string? UserName { get; set; } = null;
    }
}
