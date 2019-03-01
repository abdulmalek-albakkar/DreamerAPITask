namespace Dreamer.EmailNotifier.Dto
{
    public class MailConfig
    {
        public string CompanyMail { get; set; }
        public string CompanyMailPassword { get; set; }
        public string STMP_ClientHost { get; set; }
        public int STMP_ClientPort { get; set; }
    }
}
