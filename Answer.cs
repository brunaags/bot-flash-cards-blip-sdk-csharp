using System.Runtime.Serialization;

namespace bot_flash_cards_blip_sdk_csharp
{
    public class Answer 
    {
        public bool IsCorrect { get; set; }

        public Person Person { get; set; }

        public string AnswerName { get; set; }

        public Answer(bool isCorrect, Person person, string answerName)
        {
            this.IsCorrect = isCorrect;
            this.Person = person;
            this.AnswerName = answerName;
        }
    }
}