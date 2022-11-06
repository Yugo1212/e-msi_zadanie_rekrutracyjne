using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Queries
{
    public record SearchWordsInTxtFileCommand(IFormFile file, string direction, int maxWordsCount, bool ignoreCase, string searchProfileId) : IRequest<IActionResult>;
}
