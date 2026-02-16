using Examination.IServices;
using Examination.Model;
using StackExchange.Redis;
using System.Text.Json;


public class ExaminationService : IExaminationService
{
    private readonly IDatabase _db;
    private readonly IWebHostEnvironment _env;

    private const string KEY = "Questions";

    private static readonly JsonSerializerOptions _jsonOptions =
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

    public ExaminationService(
        IConnectionMultiplexer redis,
        IWebHostEnvironment env)
    {
        _db = redis.GetDatabase();
        _env = env;
    }

    private async Task<List<Question>> LoadAsync()
    {
        var value = await _db.StringGetAsync(KEY);

        if (!value.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<List<Question>>(value!, _jsonOptions)
                   ?? new List<Question>();
        }

        var filePath = Path.Combine( _env.ContentRootPath,"data", "Examination.json");

        if (!File.Exists(filePath))
            return new List<Question>();

        var jsonFromFile = await File.ReadAllTextAsync(filePath);

        var questions = JsonSerializer.Deserialize<List<Question>>(jsonFromFile, _jsonOptions)
                        ?? new List<Question>();

        await SaveAsync(questions);

        return questions;
    }

    private async Task SaveAsync(List<Question> questions)
    {
        var json = JsonSerializer.Serialize(questions, _jsonOptions);
        await _db.StringSetAsync(KEY, json, TimeSpan.FromMinutes(30));
    }

    public async Task<List<Question>> GetAllAsync()
    {
        var questions = await LoadAsync();
        return questions.OrderBy(q => q.QuestionNo).ToList();
    }

    public async Task<Question> CreateAsync(Question model)
    {
        var questions = await LoadAsync();

        var nextQuestionId = questions.Any()
            ? questions.Max(q => q.QuestionId) + 1
            : 1;

        var nextNo = questions.Any()
            ? questions.Max(q => q.QuestionNo) + 1
            : 1;

        model.QuestionId = nextQuestionId;
        model.QuestionNo = nextNo;

        int nextChoiceId = questions
            .SelectMany(q => q.Choices)
            .Any()
            ? questions.SelectMany(q => q.Choices).Max(c => c.ChoiceId) + 1
            : 1;

        foreach (var choice in model.Choices)
        {
            choice.ChoiceId = nextChoiceId++;
            choice.QuestionId = model.QuestionId;
        }

        questions.Add(model);

        await SaveAsync(questions);

        return model;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var questions = await LoadAsync();

        var question = questions.FirstOrDefault(q => q.QuestionId == id);

        if (question == null)
            return false;

        var deletedNo = question.QuestionNo;

        questions.Remove(question);

        foreach (var q in questions.Where(q => q.QuestionNo > deletedNo))
        {
            q.QuestionNo -= 1;
        }

        await SaveAsync(questions);

        return true;
    }
}