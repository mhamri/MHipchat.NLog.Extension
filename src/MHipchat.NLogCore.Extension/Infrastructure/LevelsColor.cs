using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;

namespace MHipchat.NLogCore.Extension.Infrastructure
{
    public class LevelsColor
    {
        //
        // Summary:
        //     Debug log level.
        public MessageColor Debug = MessageColor.Purple;

        //
        // Summary:
        //     Error log level.
        public MessageColor Error = MessageColor.Red;

        //
        // Summary:
        //     Fatal log level.
        public MessageColor Fatal = MessageColor.Red;

        //
        // Summary:
        //     Info log level.
        public MessageColor Info = MessageColor.Green;

        //
        // Summary:
        //     Off log level.
        public MessageColor Off = MessageColor.Gray;

        //
        // Summary:
        //     Trace log level.
        public MessageColor Trace = MessageColor.Yellow;

        //
        // Summary:
        //     Warn log level.
        public MessageColor Warn = MessageColor.Yellow;

        public LevelsColor(IList<LevelColorConfiguration> levelColorConfigurations)
        {
            foreach (var levelColorConfiguration in levelColorConfigurations)
            {
                SetColor(levelColorConfiguration.Level, levelColorConfiguration.Color);
            }
        }

        private void SetColor(string level, string color)
        {
            MessageColor errColor;
            var thisType = this.GetType();
            var severityColorConfig = thisType.GetFields().FirstOrDefault(x => x.Name.ToLower() == level.ToLower());
            
            if (severityColorConfig !=null && Enum.TryParse(color, true, out errColor))
            {
                severityColorConfig.SetValue(this, errColor);
            }
        }

        public MessageColor GetColor(LogLevel level)
        {
            MessageColor errColor;
            var thisType = this.GetType();
            var severityColorConfig = thisType.GetFields().FirstOrDefault(x => x.Name.ToLower() == level.Name.ToLower());
            var colorText = severityColorConfig?.GetValue(this).ToString();

            if (colorText != null && Enum.TryParse(colorText, true, out errColor))
            {
                return errColor;
            }

            return MessageColor.Gray;
        }
    }

    public enum MessageColor
    {
        Yellow = 0,
        Red = 1,
        Green = 2,
        Purple = 3,
        Gray = 4
    }
}