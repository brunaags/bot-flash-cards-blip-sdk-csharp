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
    using System.Collections.Generic;

    public class StateMachine
    {
        private readonly IStateManager _stateManager;
       
        private readonly ISender _sender;

        private readonly Game _game = new Game();

        private List<People> _people;

        public StateMachine(ISender sender, IStateManager stateManager)
        {
            _sender = sender;
            _stateManager = stateManager;
            _people = Reader.Run();
        }

        public async Task<string> VerifyStateAsync(Message message, CancellationToken cancellationToken)
        {
            var currentState = await _stateManager.GetStateAsync(message.From.ToIdentity(), cancellationToken);
            return currentState == "default" ? "state-one" : currentState;
        }

        public async Task RunAsync(Message message, CancellationToken cancellationToken, ChatState chatState)
        {
            switch (await VerifyStateAsync(message, cancellationToken))
            {
                case "state-one":
                    await _sender.SendMessageAsync("Hi! Let's start!", message.From, cancellationToken);
                    await _sender.SendMessageAsync("What's your name?", message.From, cancellationToken);
                    await _stateManager.SetStateAsync(message.From, "state-two", cancellationToken);
                break;

                case "state-two":
                    _game.GamerName = message.Content.ToString();
                    await _sender.SendMessageAsync("How many questions do you want to your game?", message.From, cancellationToken);  
                    await _stateManager.SetStateAsync(message.From, "state-three", cancellationToken);
                break;

                case "state-three":
                    _game.Questions = Convert.ToInt32(message.Content.ToString());
                    await _stateManager.SetStateAsync(message.From, "state-four", cancellationToken);
                    await _sender.SendMessageAsync(_game.Run(_people), message.From, cancellationToken);
                    _game.Questions--;
                break;

                case "state-four":
                    _game.ProccessAnswer(message.Content.ToString());

                    if (_game.Questions > 0)
                    {
                        await _sender.SendMessageAsync(_game.Run(_people), message.From, cancellationToken);
                        _game.Questions--;
                    }
                    else
                    {
                        await _sender.SendMessageAsync($"Yay! Your result is: {_game.Result}.", message.From, cancellationToken);
                        await _stateManager.SetStateAsync(message.From, "state-one", cancellationToken); 
                    }                 
                break;

                default:
                    await _sender.SendMessageAsync("Sorry, I don't understand.", message.From, cancellationToken);
                    await _stateManager.SetStateAsync(message.From, "state-one", cancellationToken);
                break;
            }
        }
    }
}