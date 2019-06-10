namespace bot_flash_cards_blip_sdk_csharp
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Metadata;
    using Lime.Messaging.Contents;
    using Lime.Protocol;

    public class Game
    {
        public string Player { get; set; }

        public int Questions { get; set; }

        public int Result { get; set; }

        public List<Person> People { get; set; }

        public List<Answer> Answers { get; set; }
        
        private Person _lastPerson;

        public Game()
        {
            Answers = new List<Answer>();
        }

        public MediaLink Run()
        {
            Random random = new Random();
            var person = random.Next(0, People.Count-1);

            var document = new MediaLink
            {
                Text = People[person].Text,
                Type = MediaType.Parse("image/jpeg"),
                AspectRatio = People[person].AspectRatio,
                Size = People[person].Size,
                Uri = new Uri(People[person].Uri, UriKind.Absolute),
                PreviewUri = new Uri(People[person].PreviewUri, UriKind.Absolute)
            };

            _lastPerson = People[person];
            People.RemoveAt(person);

            return document;
        }

        public void ProccessAnswer(string answerName)
        {
            var isCorrect = false;

            if (_lastPerson.Name.ToLower().Contains(answerName.ToLower()))
            {
                isCorrect = true;
            }

            Answers.Add(new Answer(isCorrect, _lastPerson, answerName));
        }
    }
}