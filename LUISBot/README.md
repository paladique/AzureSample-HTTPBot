# LUIS Chatbot Sample

Bot using the [Microsoft Bot Framework](https://dev.botframework.com?WT.mc_id=ca-github-jasmineg) and Language Understanding (LUIS) that returns search queries on Microsoft Docs and Samples.

## How it Works

The bot greets the user and prompts for their choices of docs or samples and returns a search query based on their choices. The LUIS model is trained to understand technology interests, including the user's.

## Prerequisites

- Clone/Download this repo to your local machine.
- [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator)
- [Node.js](https://nodejs.org) version 10.14.1 or higher
  OR
- [.NET Core SDK](https://dotnet.microsoft.com/download?WT.mc_id=ca-github-jasmineg)

- [Azure Account](https://azure.microsoft.com/en-us/free/?WT.mc_id=ca-github-jasmineg)
- [Student? 🎓 Sign up for an Azure Student account!](https://azure.microsoft.com/en-us/free/students/?WT.mc_id=ca-github-jasmineg)

## Setup

### Language Understanding (LUIS)

- Sign in to [LUIS](https://www.luis.ai/)
- Import `ContentModel.json` as new app. **+ New App for conversation** > **Import as JSON**
- Click `Train` and try some input with `Test`
- Click `Publish` to create api keys for the model.
- Copy the primary or secondary key, endpoint url, app id from `Manage` > `Azure Resources` and `Settings`

### Edit Code

#### Edit Code: Node

- Paste copied values from last step in the provided `.env` file

    ```node
    LuisAppId=
    LuisAPIKey=
    LuisAPIHostName=https://copied-url
    ```

#### Edit Code: .NET Core

- In your favorite command line tool, navigate to the project directory and enter the following values from last step in the commands

    ```bash
    dotnet user-secrets init
    dotnet user-secrets set "LuisAppId" "copied-app-id"
    dotnet user-secrets set "LuisAPIKey" "api-key"
    dotnet user-secrets set "LuisAPIHostName" "copied-url"
    ```

- Confirm that the .csproj file contains a UserSecretsId entry: `<UserSecretsId>unique-guid</UserSecretsId>`
- You can remove the secrets with

    ```bash
    dotnet user-secrets clear
    ```

### Run the bot

#### Run the bot: Node

- Install modules and start bot

    ```bash
    npm install
    npm start
    ```

#### Run the bot: .NET Core

- Start from command line (in project directory)
  
    ```bash
    dotnet run
    ```

### Testing the bot locally using Bot Framework Emulator

The [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Optional: Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Learn More

### Concepts Used in This Demo

- [Gathering Input Using Prompts](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?WT.mc_id=ca-github-jasmineg)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?WT.mc_id=ca-github-jasmineg)
- [Receive and respond to inbound HTTPS requests in Azure Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/connectors-native-reqres#prerequisites?WT.mc_id=ca-github-jasmineg)

### General Resources

- [Bot Framework Documentation](https://docs.botframework.com?WT.mc_id=ca-github-jasmineg)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?WT.mc_id=ca-github-jasmineg)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?WT.mc_id=ca-github-jasmineg)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/?WT.mc_id=ca-github-jasmineg)
- [Overview - What is Azure Logic Apps?](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-overview?WT.mc_id=ca-github-jasmineg)
