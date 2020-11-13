# Bot Framework Chatbot Samples

Collection of samples using the [Microsoft Bot Framework](https://dev.botframework.com?WT.mc_id=ca-github-jasmineg).

## Samples

### Http Bot

Bot that send a user's responses to an HTTP endpoint.

[Node](HttpBot/node)

[.NET Core](HttpBot/dotnet_core)

### LUIS Bot

Bot that returns search queries on Microsoft Docs and Samples.

[Node](LUISBot/node)

[.NET Core](LUISBot/dotnet_core)

## Prerequisites

- Clone/Download this repo to your local machine.
- [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator)
- [Node.js](https://nodejs.org) version 10.14.1 or higher
  OR
- [.NET Core SDK](https://dotnet.microsoft.com/download?WT.mc_id=academic-0000-jasmineg)

- [Azure Account](https://azure.microsoft.com/free/?WT.mc_id=academic-0000-jasmineg)
- [Student? 🎓 Sign up for an Azure Student account!](https://azure.microsoft.com/free/students/?WT.mc_id=academic-0000-jasmineg)

### Testing the bot locally using Bot Framework Emulator

The [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter Bot URL of `http://localhost:3978/api/messages`

## Optional: Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Learn More

### Concepts Used in This Demo

- [Gathering Input Using Prompts](https://docs.microsoft.com/azure/bot-service/bot-builder-prompts?WT.mc_id=academic-0000-jasmineg)
- [Activity processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing?WT.mc_id=academic-0000-jasmineg)
- [Receive and respond to inbound HTTPS requests in Azure Logic Apps](https://docs.microsoft.com/azure/connectors/connectors-native-reqres?WT.mc_id=academic-0000-jasmineg#prerequisites?WT.mc_id=ca-github-jasmineg)

### General Resources

- [Bot Framework Documentation](https://docs.botframework.com?WT.mc_id=ca-github-jasmineg)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?WT.mc_id=academic-0000-jasmineg)
- [Channels and Bot Connector Service](https://docs.microsoft.com/azure/bot-service/bot-concepts?WT.mc_id=academic-0000-jasmineg)
- [Language Understanding using LUIS](https://docs.microsoft.com/azure/cognitive-services/luis/?WT.mc_id=academic-0000-jasmineg)
- [Overview - What is Azure Logic Apps?](https://docs.microsoft.com/azure/logic-apps/logic-apps-overview?WT.mc_id=academic-0000-jasmineg)
