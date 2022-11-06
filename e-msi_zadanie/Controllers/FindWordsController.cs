using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_msi_zadanie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindWordsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FindWordsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/AddProfile")]
        public async Task<IActionResult> AddSearchProfile(IList<string> searchProfiles)
        {
            return await _mediator.Send(new CreateSearchProfileCommand(searchProfiles));
        }

        [HttpPost]
        [Route("/Search")]
        public async Task<IActionResult> PostSearchInTxtFile(IFormFile file, string direction, int maxWordsCount,
            bool ignoreCase, string searchProfileId)
        {
            return await _mediator.Send(new SearchWordsInTxtFileCommand(file, direction, maxWordsCount, ignoreCase, searchProfileId));
        }
    }
}
