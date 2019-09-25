using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebChatBot.Models;
using WebChatBot.LuisApiService;
using Microsoft.Bot.Schema;
using System.IO;
using AdaptiveCards;

namespace WebChatBot.Dialogs
{
    public class UserLoginDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserAuthModel> _userProfileAccessor;
        public static bool Log { get; set; }
        public static bool FirstQuery { get; set; }
        public static string UltimaMensagem { get; set; }
        public UserLoginDialog(UserState userState) : base(nameof(UserLoginDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserAuthModel>("UserAuthModel");
            var waterfallSteps = new WaterfallStep[]
            {
                PerguntaStepASync,
                VerifyQnAQuestionStepAsync,
                VerifyLuisIntentionStepAsync,
                LoginConfirmStepAsync,
                EmailStepAsync,
                SenhaStepAsync,
                ConfirmAuthenticationStepAsync
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private static async Task<DialogTurnResult> PerguntaStepASync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (UltimaMensagem != null)
            {
                if (stepContext.Context.Activity.Text == "sair")
                {
                    if (Log)
                        Log = false;
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Você saiu da sessão. Pode continuar me perguntando..."));
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
                return await stepContext.NextAsync(UltimaMensagem, cancellationToken);
            }
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Pergunte algo!") }, cancellationToken);
        }
        private async Task<DialogTurnResult> VerifyQnAQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = "3b26acd3-0c23-4f8f-9936-578b45e35cbd",
                EndpointKey = "7424347c-a52b-40a9-8779-3ae315afe901",
                Host = "https://webchatqna.azurewebsites.net/qnamaker"
            },
                null,
                new HttpClient());

            var response = await qnaMaker.GetAnswersAsync(stepContext.Context);
            if (response != null && response.Length > 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
                UltimaMensagem = stepContext.Context.Activity.Text;
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(stepContext.Result, cancellationToken);
            }
        }
        private async Task<DialogTurnResult> VerifyLuisIntentionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var response = stepContext.Context.Activity.CreateReply();
            if (Log)
            {
                UltimaMensagem = stepContext.Context.Activity.Text;
                var intencao = LuisService.ObterIntencao(UltimaMensagem);
                switch (intencao)
                {
                    case "BancoDeHorasQuery":
                        response.Attachments = new List<Attachment>() { ObterCardAttachment("HorasCard.json") };
                        await stepContext.Context.SendActivityAsync(response);
                        UltimaMensagem = stepContext.Context.Activity.Text;
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    case "ProximasFeriasQuery":
                        response.Attachments = new List<Attachment>() { ObterCardAttachment("FeriasCard.json") };
                        await stepContext.Context.SendActivityAsync(response);
                        UltimaMensagem = stepContext.Context.Activity.Text;
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    default:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Não entendi sua pergunta..."));
                        UltimaMensagem = stepContext.Context.Activity.Text;
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                }
            }
            else
            {
                return await stepContext.NextAsync("", cancellationToken);
            }
        }
        private async Task<DialogTurnResult> LoginConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Para atender a solicitação, você precisa se autenticar. Deseja iniciar a autenticação?") }, cancellationToken);
        }
        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Informe seu email, por favor.") }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Tudo bem, pode continuar me perguntando."), cancellationToken);
                UltimaMensagem = stepContext.Context.Activity.Text;
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
        private async Task<DialogTurnResult> SenhaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["email"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Agora, preciso da sua senha...") }, cancellationToken);
        }
        private async Task<DialogTurnResult> ConfirmAuthenticationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["senha"] = (string)stepContext.Result;
            var userAuth = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserAuthModel(), cancellationToken);
            userAuth.Email = (string)stepContext.Values["email"];
            userAuth.Senha = (string)stepContext.Values["senha"];
            if (AutenticacaoService.AutenticarUsuario(userAuth.Email, userAuth.Senha))
            {
                Log = true;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Você iniciou sua sessão. Para finalizar, digite 'sair'."), cancellationToken);
                UltimaMensagem = stepContext.Context.Activity.Text;
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            Log = false;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Credenciais inválidas."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        private Attachment ObterCardAttachment(string cardPath)
        {
            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(File.ReadAllText("AdaptiveCards/" + cardPath)).Card
            };
        }
    }
}
