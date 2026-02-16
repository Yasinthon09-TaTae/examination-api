using Examination.Model;

namespace Examination.IServices
{
    public interface IExaminationService
    {
        Task<List<Question>> GetAllAsync();
        Task<Question> CreateAsync(Question model);
        Task<bool> DeleteAsync(int id);
    }
}
