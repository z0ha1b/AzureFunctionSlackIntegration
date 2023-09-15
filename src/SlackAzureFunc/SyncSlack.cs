using System;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SlackAzureFunc
{
    public class SyncSlack
    {
        private readonly ILogger _logger;

        public SyncSlack(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SyncSlack>();
        }

        [Function("SyncSlack")]
        public async Task Run([TimerTrigger("0/1 * * * * *")] MyInfo myTimer)
        {
            var message = "Azure loves you!";
            await MakeSlackRequest(message);

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus?.Next}");
        }

        private async Task<string> MakeSlackRequest(string message)
        {
            var content = "{'text': '" + message + "'}";

            using (var client = new HttpClient())
            {
                var requestData = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("your slack hook address", requestData);

                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus? ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
