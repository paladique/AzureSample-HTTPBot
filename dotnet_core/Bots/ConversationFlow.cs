// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.9.1

namespace HTTPBotSample
{
    public class ConversationFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            Name,
            Message,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}