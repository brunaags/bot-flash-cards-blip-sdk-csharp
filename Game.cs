namespace bot_flash_cards_blip_sdk_csharp
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Metadata;
    using Lime.Messaging.Contents;
    using Lime.Protocol;

    public class Game
    {
        public string GamerName { get; set; }

        public int Questions { get; set; }

        public int Result { get; set; }
        
        private string _lastAnswer;

        public MediaLink Run(List<People> people)
        {
            Random random = new Random();
            var person = random.Next(0, Questions);

            var document = new MediaLink
            {
                Text = people[person].Text,
                Type = MediaType.Parse("image/jpeg"),
                AspectRatio = people[person].AspectRatio,
                Size = people[person].Size,
                Uri = new Uri(people[person].Uri, UriKind.Absolute),
                PreviewUri = new Uri(people[person].PreviewUri, UriKind.Absolute)
            };

            _lastAnswer = people[person].Name.ToLower();
            people.RemoveAt(person);

            return document;
        }

        public void ProccessAnswer(string answer)
        {
            if (_lastAnswer.Contains(answer.ToLower()))
                Result++;                
        }
    }
}