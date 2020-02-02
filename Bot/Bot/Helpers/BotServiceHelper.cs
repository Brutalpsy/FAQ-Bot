using Bot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Helpers
{
    public class BotServiceHelper
    {
        private const string BaseUrl = "https://directline.botframework.com";
        private const string Endpont = "v3/directline/conversations";
        private const string AuthorizationKey = "Authorization";
        private const string AuthorizationValue = "gJtvtssznbk.0n7rhU4xRZOqjdlTRxRGGtyCozKUycj-ljhF8EAP4zU";

        public event EventHandler<BotResponseEventArgs> MessageRecieved;
        private Conversation _conversation { get; set; }

        public BotServiceHelper()
        {

        }

        public async Task CreateConversation()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Add(AuthorizationKey, $"Bearer {AuthorizationValue}");

                var response = await client.PostAsync(Endpont, null);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _conversation = JsonConvert.DeserializeObject<Conversation>(content);
                }
            }
            await ReadMessage();

        }

        public async Task SendActivity(string message)
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AuthorizationValue}");

                Activity activity = new Activity
                {
                    From = new ChannelAccount
                    {
                        Id = "user1"
                    },
                    Text = message,
                    Type = "message"
                };

                string jsonContent = JsonConvert.SerializeObject(activity);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                string endpoint = $"/v3/directline/conversations/{_conversation.ConversationId}/activities";
                var response = await client.PostAsync(endpoint, byteContent);
                string json = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(json);
                var id = (string)obj.SelectToken("id");
            }
        }

        public async Task ReadMessage()
        {

            var client = new ClientWebSocket();
            var cts = new CancellationTokenSource();
            await client.ConnectAsync(new Uri(_conversation.StreamUrl), cts.Token);

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    WebSocketReceiveResult result;
                    var message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await client.ReceiveAsync(message, cts.Token);
                        try
                        {
                            if (result.MessageType != WebSocketMessageType.Text)
                                break;

                            var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                            string messageJSON = Encoding.UTF8.GetString(messageBytes);
                            var botsResponse = JsonConvert.DeserializeObject<BotResponse>(messageJSON);

                            var args = new BotResponseEventArgs();
                            args.Activities = botsResponse?.Activities;

                            MessageRecieved?.Invoke(this, args);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    while (!result.EndOfMessage);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
