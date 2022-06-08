// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class ChatBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog dialog;
        protected readonly BotState conversationState;
        protected readonly BotState userState;
        protected readonly ILogger logger;

        public ChatBot(T _dialog, ConversationState _conversationState,
            UserState _userState, ILogger<ChatBot<T>> _logger)
        {
            dialog = _dialog;
            conversationState = _conversationState;
            userState = _userState;
            logger = _logger;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        MessageFactory.Text($"Bienvenido te saluda Bot, " +
                        $"tu asistente para agendamiento de citas \n\nEscribe algo para empezar"), 
                        cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await dialog.RunAsync(
                turnContext,
                conversationState.CreateProperty<DialogState>(nameof(DialogState)),
                cancellationToken);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await userState.SaveChangesAsync(turnContext, false, cancellationToken);
            await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
