using Examination.IServices;
using Examination.Model;
using Microsoft.AspNetCore.Mvc;

[Route("api/examination")]
[ApiController]
public class ExaminationController : ControllerBase
{
    private readonly IExaminationService _service;

    public ExaminationController(IExaminationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Question model)
    {
        var result = await _service.CreateAsync(model);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);

        if (!success)
            return NotFound();

        return Ok();
    }
}