using ActivityTracker.Models;
using ActivityTracker.Models.Request_Models;
using ActivityTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ActivityTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrainingController : ControllerBase
    {
        private readonly TrainingService _trainingService;

        public TrainingController(TrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTraining([FromBody] TrainingStartDto training)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var createdTraining = await _trainingService.CreateTrainingAsync(email, training);
            return Ok(createdTraining.Id);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTrainings()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var trainings = await _trainingService.GetMyTrainingsAsync(email);
            return Ok(trainings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainingDetails(int id)
        {
            var trainingDetails = await _trainingService.GetTrainingDetailsAsync(id);
            if (trainingDetails == null)
                return NotFound();

            return Ok(trainingDetails);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetTrainingStats([FromQuery] TrainingType? type, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var stats = await _trainingService.GetTrainingStatsAsync(email, type, startDate, endDate);
            return Ok(stats);
        }

        [HttpPut("{id}/finish")]
        public async Task<IActionResult> FinishTraining(int id)
        {
            await _trainingService.FinishTrainingAsync(id);
            return Ok();
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateTraining(int id, [FromBody] TrainingPointDto newPoint)
        {
            await _trainingService.UpdateTrainingAsync(id, newPoint);
            return Ok();
        }
    }
}
