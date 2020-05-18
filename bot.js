// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const fetch = require('node-fetch');
const dotenv = require('dotenv');
const path = require('path');
const { ActivityHandler, MessageFactory } = require('botbuilder');

const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });
const CONVERSATION_FLOW_PROPERTY = 'CONVERSATION_FLOW_PROPERTY';
const USER_PROFILE_PROPERTY = 'USER_PROFILE_PROPERTY';
const HTTP_MESSAGE_ENDPOINT = process.env.MessageEndpoint;

const question = {
    name: 'name',
    message: 'message',
    none: 'none'
};

class HTTPBot extends ActivityHandler {
    constructor(conversationState, userState) {
        super();
        // The state property accessors for conversation flow and user profile.
        this.conversationFlow = conversationState.createProperty(CONVERSATION_FLOW_PROPERTY);
        this.userProfile = userState.createProperty(USER_PROFILE_PROPERTY);

        // The state management objects for the conversation and user.
        this.conversationState = conversationState;
        this.userState = userState;

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            const flow = await this.conversationFlow.get(context, { lastQuestionAsked: question.none });
            const profile = await this.userProfile.get(context, {});
            await HTTPBot.fillOutUserProfile(flow, profile, context);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        // Welcome user when they start the chat
        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            const welcomeText = 'Hello and welcome to this bot!';
            for (let cnt = 0; cnt < membersAdded.length; ++cnt) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(MessageFactory.text(welcomeText, welcomeText));
                }
            }
            const flow = await this.conversationFlow.get(context, { lastQuestionAsked: question.none });
            const profile = await this.userProfile.get(context, {});
            await HTTPBot.fillOutUserProfile(flow, profile, context);
            await next();
        });
    }

    async run(context) {
        await super.run(context);
        // Save any state changes. The load happened during the execution of the Dialog.
        await this.conversationState.saveChanges(context, false);
        await this.userState.saveChanges(context, false);
    }

    static async fillOutUserProfile(flow, profile, turnContext) {
        const input = turnContext.activity.text;
        let result;
        switch (flow.lastQuestionAsked) {
        case question.none:
            await turnContext.sendActivity("Let's get started. What is your name?");
            flow.lastQuestionAsked = question.name;
            break;

        // If we last asked for their name, record their response, confirm that we got it.
        // Ask them for their message and update the conversation flag.
        case question.name:
            result = input !== '';
            if (result) {
                profile.name = input;
                await turnContext.sendActivity(`Hi ${ profile.name }! What is your message?`);
                flow.lastQuestionAsked = question.message;
                break;
            } else {
                // If we couldn't interpret their input, ask them for it again.
                // Don't update the conversation flag, so that we repeat this step.
                await turnContext.sendActivity(result.message || "I'm sorry, I didn't understand that.");
                break;
            }

        case question.message:
            result = input !== '';
            if (result) {
                profile.message = input;
                await sendResponses(profile);
                await turnContext.sendActivity(`Thank you for this message!!: ${ profile.message }.`);
                break;
            } else {
                await turnContext.sendActivity(result.message || 'Sorry! There was a problem sending your message.');
                break;
            }
        }
    }
}

async function sendResponses(userInput) {
    const url = new URL(HTTP_MESSAGE_ENDPOINT, 'https://prod-05.centralus.logic.azure.com:443');

    // Send responses to endpoint
    await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userInput)
    }).then(() => console.log('success!'));
}

module.exports.HTTPBot = HTTPBot;
