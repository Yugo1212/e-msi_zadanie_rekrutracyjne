using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Commands
{
    public record CreateSearchProfileCommand(IList<string> userProfiles) : IRequest<IActionResult>;
}
