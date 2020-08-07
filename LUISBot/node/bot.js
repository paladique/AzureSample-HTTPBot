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
    constructor(conversationState, userState, dialog) {
        super();
        
        this.conversationState = conversationState;
        this.userState = userState;
        this.dialog = dialog;
        this.dialogState = this.conversationState.createProperty('DialogState');

        // The state property accessors for conversation flow and user profile.
        this.conversationFlow = conversationState.createProperty(CONVERSATION_FLOW_PROPERTY);
        this.userProfile = userState.createProperty(USER_PROFILE_PROPERTY);

        // The state management objects for the conversation and user.
        this.conversationState = conversationState;
        this.userState = userState;

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {    
            await this.dialog.run(context, this.dialogState);
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
            await this.dialog.run(context, this.dialogState);
            await next();
        });
    }

    async run(context) {
        await super.run(context);
        // Save any state changes. The load happened during the execution of the Dialog.
        await this.conversationState.saveChanges(context, false);
        await this.userState.saveChanges(context, false);
    }
}

module.exports.HTTPBot = HTTPBot;
