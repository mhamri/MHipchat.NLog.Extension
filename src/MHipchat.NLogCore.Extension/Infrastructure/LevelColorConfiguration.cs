using NLog.Config;

namespace MHipchat.NLogCore.Extension.Infrastructure
{
    [NLogConfigurationItem]
    public class LevelColorConfiguration
    {
        public LevelColorConfiguration(){}
        public string Level { get; set; }

        public string Color { get; set; }
    }
}