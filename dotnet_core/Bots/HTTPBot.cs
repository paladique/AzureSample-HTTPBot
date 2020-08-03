using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace HTTPBotSample
{
    public class HTTPBot : ActivityHandler
    {
        private BotState _userState;
        private readonly BotState _conversationState;

        HTTPBot(ConversationState conversationState, UserState userState)
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
                        await turnContext.SendActivityAsync($"Thank you for your message!!.", null, null, cancellationToken);
                        flow.LastQuestionAsked = ConversationFlow.Question.None;
                        break;
            }
        }
    }
}
