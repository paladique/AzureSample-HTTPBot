using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;


namespace HTTPBotSample
{
    public class HTTPBot : ActivityHandler
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;

        public HTTPBot(ConversationState conversationState, UserState userState)
        {
            _userState = userState;
            _conversationState = conversationState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // var replyText = $"Echo: {turnContext.Activity.Text}";
            // await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            var conversationStateAccessors = _conversationState.CreateProperty<ConversationFlow>(nameof(ConversationFlow));
            var flow = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationFlow(), cancellationToken);

            var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            var profile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile(), cancellationToken);
            await FillOutUserProfileAsync(flow, profile, turnContext, cancellationToken);

            // Save changes.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome to this bot!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private static async Task FillOutUserProfileAsync(ConversationFlow flow, UserProfile profile, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var input = turnContext.Activity.Text?.Trim();

            switch (flow.LastQuestionAsked)
            {
                case ConversationFlow.Question.None:
                    await turnContext.SendActivityAsync("Let's get started. What is your name?", null, null, cancellationToken);
                    flow.LastQuestionAsked = ConversationFlow.Question.Name;
                    break;
                case ConversationFlow.Question.Name:

                    profile.Name = input;
                    await turnContext.SendActivityAsync($"Hi {profile.Name}.", null, null, cancellationToken);
                    await turnContext.SendActivityAsync("What's your message?", null, null, cancellationToken);
                    flow.LastQuestionAsked = ConversationFlow.Question.Message;
                    break;

                case ConversationFlow.Question.Message:
                    profile.Message = input;
                    await turnContext.SendActivityAsync($"Thank you for your message!", null, null, cancellationToken);
                    flow.LastQuestionAsked = ConversationFlow.Question.None;

                    sendResponses(profile);
                    break;
            }
        }

        static void sendResponses(UserProfile profile)
        {
            var json = JsonConvert.SerializeObject(profile);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var hc = new HttpClient();
            var response = hc.PostAsync("https://prod-23.centralus.logic.azure.com:443/workflows/23e4600d35c24fe08254a35c880401a9/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=xxhAQdFxKrAHDApfDwnFmFB0lmcLNMzEeUpM8OU4kq8", data);

            if (response.Result.StatusCode == HttpStatusCode.OK)
            {

            }

        }      
    }
}

