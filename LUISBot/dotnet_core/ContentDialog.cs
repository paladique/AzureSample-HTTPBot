using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace LUISBotSample
{
    public class ContentDialog : ComponentDialog
    {
        private readonly ContentRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

       private readonly string  MS_Root = "https://docs.microsoft.com/";
       private readonly string  MS_SamplesSearchURL = "samples/browse/?terms=";
       private readonly string  MS_DocsSearchURL = "search/?terms=";

        public string DocsUrl => MS_Root + MS_DocsSearchURL;
        public string SamplesUrl => MS_Root + MS_SamplesSearchURL;

        public ContentDialog(ContentRecognizer luisRecognizer, ILogger<ContentDialog> logger)
                  : base(nameof(ContentDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SelectResourceStepAsync,
                SelectTechStepAsync,
                ConfirmChoicesStepAsync,
                SendResults,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> SelectResourceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file."), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Welcome to the Docs Bot! Which learning resources did you want?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Docs", "Samples", "Both" }),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SelectTechStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["resource"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What language did you want?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmChoicesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["lang"] = (string)stepContext.Result;
            var resource = (string)stepContext.Values["resource"];
            var lang = (string)stepContext.Values["lang"];

            var luisResult = await _luisRecognizer.RecognizeAsync<ContentModel>(stepContext.Context, cancellationToken);
            if (luisResult.TopIntent().intent != ContentModel.Intent.Learning)
            {
                MessageFactory.Text($"Your query for {lang} was not recognized by this bot, but will still be added to the search results");
            }
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text($"You want to see {resource} in {lang} Is this correct?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> SendResults(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);

            var resource = (string)stepContext.Values["resource"];
            var lang = (string)stepContext.Values["lang"];

            var cardActions = new List<CardAction>();

            if (resource == "Docs" || resource == "Both")
                cardActions.Add(new CardAction(ActionTypes.OpenUrl, $"MS Docs on {lang}", value: DocsUrl + lang));
            if (resource == "Samples" || resource == "Both")
                cardActions.Add(new CardAction(ActionTypes.OpenUrl, $"MS Samples on {lang}", value: SamplesUrl + lang));

            var adaptiveCardAttachment = new HeroCard()
            {
                Text = "Here's your links!",
                Buttons = cardActions
            };

            reply.Attachments.Add(adaptiveCardAttachment.ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.EndDialogAsync();
        }
    }
}