const { LuisRecognizer } = require("botbuilder-ai");
const { CardFactory, MessageFactory } = require("botbuilder");

const {
  ChoiceFactory,
  ChoicePrompt,
  ComponentDialog,
  ConfirmPrompt,
  DialogSet,
  DialogTurnStatus,
  TextPrompt,
  WaterfallDialog,
} = require("botbuilder-dialogs");

const CHOICE_PROMPT = "CHOICE_PROMPT";
const CONFIRM_PROMPT = "CONFIRM_PROMPT";
const LANG_PROMPT = "LANG_PROMPT";
const WATERFALL_DIALOG = "WATERFALL_DIALOG";

const MS_Root = "https://docs.microsoft.com/";
const MS_SamplesSearchURL = "samples/browse/?terms=";
const MS_DocsSearchURL = "search/?terms=";

class ContentDialog extends ComponentDialog {
  constructor(luis) {
    super("contentDialog");
    this.luis = luis;

    this.addDialog(new ChoicePrompt(CHOICE_PROMPT));
    this.addDialog(new TextPrompt(LANG_PROMPT));
    this.addDialog(new ConfirmPrompt(CONFIRM_PROMPT));

    this.addDialog(
      new WaterfallDialog(WATERFALL_DIALOG, [
        this.selectResourceStep.bind(this),
        this.selectTechStep.bind(this),
        this.confirmChoices.bind(this),
        this.sendResults.bind(this),
      ])
    );

    this.initialDialogId = WATERFALL_DIALOG;
  }

  async selectResourceStep(step) {
    if (!this.luis.isConfigured) {
      const messageText =
        "NOTE: LUIS is not configured. To enable all capabilities, add `LuisAppId`, `LuisAPIKey` and `LuisAPIHostName` to the .env file.";
      step.context.sendActivity(messageText);
      return await step.endDialog();
    } else {
      return await step.prompt(CHOICE_PROMPT, {
        prompt: "Which learning resources did you want?",
        choices: ChoiceFactory.toChoices(["Docs", "Samples", "Both"]),
      });
    }
  }

  async selectTechStep(step) {
    step.values.resources = step.result.value;
    return await step.prompt(LANG_PROMPT, "What language did you want?");
  }

  async confirmChoices(step) {
    step.values.lang = step.result;
    const luisResult = await this.luis.executeLuisQuery(step.context);
    const message = `You want to see ${step.values.resources} in ${step.values.lang} Is this correct?`;

    if (LuisRecognizer.topIntent(luisResult) != "Learning") {
      step.context.sendActivity(
        `Your query for ${step.values.lang} was not recognized by this bot, but will still be added to the search results`
      );
    }

    return await step.prompt(CONFIRM_PROMPT, message);
  }

  async sendResults(step) {
    let url = MS_Root;
    let resource = step.values.resources;
    let language = step.values.lang;

    let sample = {
      type: "Action.OpenUrl",
      title: "MS Samples on " + language,
      url: `${url + MS_SamplesSearchURL + language}`,
    };

    let docs = {
      type: "Action.OpenUrl",
      title: "MS Docs on " + language,
      url: `${url + MS_DocsSearchURL + language}`,
    };

    let cardSchema = {
      $schema: "http://adaptivecards.io/schemas/adaptive-card.json",
      type: "AdaptiveCard",
      version: "1.0",
      body: [
        {
          type: "TextBlock",
          text: "Here's your links!",
        },
      ],
      actions: [],
    };

    if (resource == "Docs") cardSchema.actions.push(docs);
    else if (resource == "Samples") cardSchema.actions.push(sample);
    else {
      cardSchema.actions.push(docs);
      cardSchema.actions.push(sample);
    }

    const card = CardFactory.adaptiveCard(cardSchema);
    const message = MessageFactory.attachment(card);
    await step.context.sendActivity(message);

    return await step.endDialog();
  }

  async run(turnContext, accessor) {
    const dialogSet = new DialogSet(accessor);
    dialogSet.add(this);
    const dialogContext = await dialogSet.createContext(turnContext);
    const results = await dialogContext.continueDialog();
    if (results.status === DialogTurnStatus.empty) {
      await dialogContext.beginDialog(this.id);
    }
  }
}

module.exports.ContentDialog = ContentDialog;
