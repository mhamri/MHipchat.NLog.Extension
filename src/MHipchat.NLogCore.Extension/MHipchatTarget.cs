using MHipchat.NLogCore.Extension.Infrastructure;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MHipchat.NLogCore.Extension
{
    /// <summary>
    /// HipChat API v1
    /// https://www.hipchat.com/docs/apiv2/method/send_room_notification
    /// </summary>
    [Target("MHipChat")]
    public class MHipchatTarget : TargetWithLayout
    {
        private const string EndPoint = "https://api.hipchat.com/v2/room/{RoomId}/notification";

        [RequiredParameter]
        public string AuthToken { get; set; }

        [RequiredParameter]
        public string RoomId { get; set; }

        public Layout From { get; set; }


        [DefaultValue("Error")]
        public string NotifyMinLevel { get; set; }

        [DefaultValue("yellow")]
        public string Color { get; set; }

        [DefaultValue("text")]
        public string MessageFormat { get; set; }

        public LevelsColor ColorLevel { get; set; }

        [DefaultValue("false")]
        public bool FireAndForget { get; set; }

        /// <summary>
        /// Gets the row highlighting rules.
        /// </summary>
        /// <docgen category='Highlighting Rules' order='10' />
        [ArrayParameter(typeof(LevelColorConfiguration), "level-color")]
        public IList<LevelColorConfiguration> LevelColorConfigurations { get; private set; }

        [DefaultValue("true")]
        public bool TryToFindCode { get; set; }

        private static LevelsColor TranslatedLevelsColor { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            if (TranslatedLevelsColor == null)
            {
                TranslatedLevelsColor = new LevelsColor(LevelColorConfigurations);
            }

            var message = Layout.Render(logEvent) ?? "";

            if (message.Length > 10000)
            {
                message = message.Substring(0, 9990) + " TRUNCATED";
            }
            var from = From.Render(logEvent);

            if (from.Length > 64)
            {
                from = from.Substring(0, 64);
            }

            IEnumerable<KeyValuePair<string, string>> postData = new Dictionary<string, string>()
            {
                ["from"] = from,
                ["message_format"] = MessageFormat,
                ["notify"] = logEvent.Level >= LogLevel.FromString(NotifyMinLevel) ? "true" : "false",
                ["message"] = message.TrimEnd(new[] { '\r', '\n', ' ' }),
                ["color"] = TranslatedLevelsColor.GetColor(logEvent.Level).ToString().ToLower()
            };

            IEnumerable<KeyValuePair<string, string>> headerData = new Dictionary<string, string>()
            {
                ["Authorization"] = $"Bearer {AuthToken}"
            };

            var endPoint = EndPoint.Replace("{RoomId}", RoomId);
            if (FireAndForget)
            {
                Task.Run(async () => await PostFormUrlEncoded(endPoint, postData, headerData)).ConfigureAwait(false);
            }
            else
            {
                PostFormUrlEncoded(endPoint, postData, headerData).GetAwaiter().GetResult();
            }
        }

        public static async Task PostFormUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData, IEnumerable<KeyValuePair<string, string>> headerData)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var keyValuePair in headerData)
                {
                    httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                }

                using (var content = new FormUrlEncodedContent(postData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    using (HttpResponseMessage response = await httpClient.PostAsync(url, content))
                    {
                        if (response.StatusCode != HttpStatusCode.NoContent)
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            throw new HttpRequestException($"{(int)response.StatusCode} - {errorMessage}");
                        }
                    }
                }
            }
        }

        public MHipchatTarget()
        {
            LevelColorConfigurations = new List<LevelColorConfiguration>();
            From = "MHipchat";
            Color = "yellow";
            MessageFormat = "text";
            TryToFindCode = true;
            FireAndForget = false;
            NotifyMinLevel = "Error";
        }
    }
}