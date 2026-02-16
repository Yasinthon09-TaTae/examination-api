namespace Examination.Model
{
    public class Choices
    {
        public int ChoiceId { get; set; }
        public int QuestionId { get; set; }
        public string ChoiceName { get; set; }
        public bool isCorrect { get; set; }
    }
}
