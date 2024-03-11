namespace NotifyUserApp.Models
{
    public class EmailModel
    {
        public string EmailTo { get; set; }
        public string Message { get; set; }
        public string Subject { get; internal set; }
    }
}