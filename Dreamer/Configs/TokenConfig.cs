namespace Dreamer.API.Configs
{
    public class TokenConfig
    {
        public string Domain { get; set; }
        public string TokenKey { get; set; }
        public int ExpiredInHours { get; set; }
    }
}
