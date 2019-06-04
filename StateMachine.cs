namespace bot_flash_cards_blip_sdk_csharp
{
    using Lime.Protocol;
    using System.Threading;
    using System.Threading.Tasks;
    using Take.Blip.Client;
    using Take.Blip.Client.Session;
    using Take.Blip.Client.Extensions.Bucket;
    using System;
    using Lime.Messaging.Contents;

    public class StateMachine
    {
        private readonly IStateManager _stateManager;
       
        private readonly ISender _sender;

        private readonly Game _game = new Game();

        public StateMachine(ISender sender, IStateManager stateManager) {
            _sender = sender;
            _stateManager = stateManager;
        }

        public async Task<string> VerifyStateAsync(Message message, CancellationToken cancellationToken)
        {
            var currentState = await _stateManager.GetStateAsync(message.From.ToIdentity(), cancellationToken);
            return currentState == "default" ? "step-one" : currentState;
        }

        public async Task RunAsync(Message message, CancellationToken cancellationToken)
        {
            switch (await VerifyStateAsync(message, cancellationToken))
            {
                case "step-one":
                    await _sender.SendMessageAsync("Hi! Let's start!", message.From, cancellationToken);
                    await _sender.SendMessageAsync("What's your name?", message.From, cancellationToken);
                    await _stateManager.SetStateAsync(message.From, "step-two", cancellationToken);
                break;

                case "step-two":
                    _game.GamerName = message.Content.ToString();
                    await _sender.SendMessageAsync("How many questions do you want to your game?", message.From, cancellationToken);  
                    await _stateManager.SetStateAsync(message.From, "step-three", cancellationToken);
                break;

                case "step-three":
                    _game.Questions = Convert.ToInt32(message.Content.ToString());
                    await _sender.SendMessageAsync("So, there is the game.", message.From, cancellationToken);
                    _game.Run();
                    await _stateManager.SetStateAsync(message.From, "step-four", cancellationToken);
                break;

                case "step-four":
                    await _sender.SendMessageAsync($"Yay! Your result is: {_game.Result}.", message.From, cancellationToken);  
                    await _stateManager.SetStateAsync(message.From, "step-one", cancellationToken);
                break;

                default:
                    await _sender.SendMessageAsync("Sorry, I don't understand.", message.From, cancellationToken);
                    await _stateManager.SetStateAsync(message.From, "step-one", cancellationToken);
                break;
            }
        }
    }
}