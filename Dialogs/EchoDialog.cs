using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            #region Is Typing Activity

            var activity = context.Activity as Activity;
            Trace.TraceInformation($"Type={activity.Type} Text={activity.Text}");
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                var isTyping = activity.CreateReply("Nerdibot is thinking...");
                isTyping.Type = ActivityTypes.Typing;
                await connector.Conversations.ReplyToActivityAsync(isTyping);

                // DEMO: I've added this for demonstration purposes, so we have time to see the "Is Typing" integration in the UI. Else the bot is too quick for us :)
                // Note: Removed this demo limit, as the bot is used in demos online too. Feel free to jack it back if you want to test it out. 
                // Thread.Sleep(2500);
            }

            #endregion

            #region Handle incoming message

            var message = await argument;

            HttpClient client = new HttpClient();
            var chuckJoke = client.GetStringAsync("https://api.chucknorris.io/jokes/random").Result;

            var deserializedChuck = JsonConvert.DeserializeObject<dynamic>(chuckJoke);
            string chuckSays = ((dynamic) deserializedChuck).value.ToString();
            
            await context.PostAsync(GetRandomGreet(activity.From.Name) + Environment.NewLine + chuckSays);

            #endregion

            context.Wait(MessageReceivedAsync);
        }

        private string GetRandomGreet(string name)
        {
            List<string> greetings = new List<string>
            {
                $"Hey there {name}!",
                $"Okay {name}.",
                $"{name}, your request has been considered, and I approve. This time.",
                $"Okidoki {name}.",
                $"What's up my awesome pal, {name}."

            };
            Random r = new Random();
            int index = r.Next(greetings.Count);
            string randomString = greetings[index];

            return randomString;

        }
    }
}