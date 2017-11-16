using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            var message = await argument;

            HttpClient client = new HttpClient();
            var chuckJoke = client.GetStringAsync("https://api.chucknorris.io/jokes/random").Result;

            var deserializedChuck = JsonConvert.DeserializeObject<dynamic>(chuckJoke);
            string chuckSays = ((dynamic) deserializedChuck).value.ToString();
            
            await context.PostAsync(GetRandomGreet() + Environment.NewLine + chuckSays);
            context.Wait(MessageReceivedAsync);
        }

        private string GetRandomGreet()
        {
            List<string> greetings = new List<string>
            {
                "Captain, I'm sorry but I don't understand. I will tell you a joke instead: ",
                "Wait, what? Nevermind, Chuck wants to relay this info: ",
                "I don't know what you're on about, but here's an entirely different topic to think about: "
            };
            Random r = new Random();
            int index = r.Next(greetings.Count);
            string randomString = greetings[index];

            return randomString;

        }

    }
}