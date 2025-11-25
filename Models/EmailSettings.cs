namespace ULIP_proj.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
