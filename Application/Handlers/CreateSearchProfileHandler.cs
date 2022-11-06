using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Handlers
{
    public class CreateSearchProfileHandler : IRequestHandler<CreateSearchProfileCommand, IActionResult>
    {
        private readonly IMemoryCache _memoryCache;

        public CreateSearchProfileHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> Handle(CreateSearchProfileCommand request, CancellationToken cancellationToken)
        {
            Guid guid = Guid.NewGuid();

            try
            {
                _memoryCache.Set($"{guid}", request.userProfiles, TimeSpan.FromMinutes(10));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return new OkObjectResult(guid);
        }
    }
}
