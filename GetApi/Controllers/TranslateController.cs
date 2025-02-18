using Microsoft.AspNetCore.Mvc;
using Service;

namespace GetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {

        private readonly IGeminiService _geminiService;

        public TranslateController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost]
        public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                return BadRequest("Input text cannot be empty.");
            }

            try
            {
                string sqlQuery = await _geminiService.GetChatResponse(request.Text);
                return Ok(new { translatedQuery = sqlQuery });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }

    // Define a model for the request body
    public class TranslationRequest
    {
        public string Text { get; set; }
    }
}
