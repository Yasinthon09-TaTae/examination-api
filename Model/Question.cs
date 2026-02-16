namespace Examination.Model
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuestionNo { get; set; }
        public string QuestionName { get; set; }
        public List<Choices> Choices { get; set; } = new();
    }
}
