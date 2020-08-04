using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace HTTPBotSample
{
    public class HTTPBot : ActivityHandler
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;
        private readonly IOptionsMonitor<Config> _optionsMonitor;

        public HTTPBot(ConversationState conversationState, UserState userState, IOptionsMonitor<Config> optionsMonitor)
        {
            _userState = userState;
            _conversationState = conversationState;
            _optionsMonitor = optionsMonitor;

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
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

        private async Task FillOutUserProfileAsync(ConversationFlow flow, UserProfile profile, ITurnContext turnContext, CancellationToken cancellationToken)
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

                    var messageRecieved = sendResponses(profile);
                    if (messageRecieved)
                    {
                        await turnContext.SendActivityAsync($"Thank you for your message!", null, null, cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"There was a problem sending your message. Try again!", null, null, cancellationToken);
                    }
                    flow.LastQuestionAsked = ConversationFlow.Question.None;
                    break;
            }
        }

        public bool sendResponses(UserProfile profile)
        {
            var endpoint = _optionsMonitor.CurrentValue.MessageEndpoint;

            var json = JsonConvert.SerializeObject(profile);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var hc = new HttpClient();
            var response = hc.PostAsync(endpoint, data);
            var success = response.Result.StatusCode == HttpStatusCode.Accepted ? true : false;
            return success;
        }
    }
}

