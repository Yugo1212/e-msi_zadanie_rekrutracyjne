using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Handlers
{
    public class SearchWordsInTxtFileHandler : IRequestHandler<SearchWordsInTxtFileCommand, IActionResult>
    {
        private readonly IMemoryCache _memoryCache;

        public SearchWordsInTxtFileHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> Handle(SearchWordsInTxtFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(request.file.OpenReadStream()))
                {
                    string content = await streamReader.ReadToEndAsync();

                    IList<string> searchProfile = _memoryCache.Get<List<string>>(request.searchProfileId);

                    char[] separators = new char[] { ' ', '.', ',', ':', ';', '\n', '\r' };

                    IList<string> result = new List<string>();

                    switch (request.direction)
                    {
                        case "left":
                            foreach (var words in searchProfile)
                            {
                                IList<int> indexes = new List<int>();
                                for (var index = 0; index < content.Length;)
                                {
                                    
                                    if (request.ignoreCase)
                                        index = content.IndexOf(words, index, StringComparison.OrdinalIgnoreCase);
                                    else
                                        index = content.IndexOf(words, index);
                                    if (index == -1)
                                        break;
                                indexes.Add(index);
                                    index = index + words.Length;
                                }

                                foreach (var id in indexes)
                                {
                                    if (indexes[0] == 0)
                                        return new OkObjectResult(result);

                                IList<string> items = content.Substring(0, id - 1)
                                    .Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

                                    if (items.Count < request.maxWordsCount)
                                    {
                                        result = items;
                                    }
                                    else
                                    {
                                        for (var i = 0; i < request.maxWordsCount;)
                                        {
                                            result.Add(items[items.Count - 1]);
                                            items.Remove(items[items.Count - 1]);
                                            i++;
                                        }
                                    }
                                }
                            }
                            break;
                        case "right":
                            foreach (var words in searchProfile)
                            {
                                IList<int> indexes = new List<int>();
                                for (var index = 0; index < content.Length;)
                                {
                                    
                                    if (request.ignoreCase)
                                        index = content.IndexOf(words, index, StringComparison.OrdinalIgnoreCase) + words.Length;
                                    else
                                        index = content.IndexOf(words, index + 1) + words.Length;
                                    if (index == -1 + words.Length)
                                        break;
                                    indexes.Add(index);
                                }

                                foreach (var id in indexes)
                                {
                                    if (indexes[0] == content.Length)
                                        return new OkObjectResult(result);

                                    IList<string> items = content.Substring(id)
                                        .Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

                                    if (items.Count < request.maxWordsCount)
                                    {
                                        result = items;
                                    }
                                    else
                                    {
                                        for (var i = 0; i < request.maxWordsCount;)
                                        {
                                            result.Add(items[i]);
                                            i++;
                                        }
                                    }
                                }
                            }
                            break;
                        case "top":
                        case "bottom":
                            foreach (var words in searchProfile)
                            {
                                IList<string> lines = content.Split('\n');
                                IList<string> linesWithWordsFromSearchProfile = new List<string>();

                                if (request.ignoreCase)
                                    linesWithWordsFromSearchProfile =
                                        lines.Where(w => w.ToLower().Contains(words.ToLower())).ToList();
                                else
                                    lines.Where(w => w.Contains(words)).ToList();

                                IList<int> indexesOfLinesToSearchIn = new List<int>();
                                int startOfSearchProfilesInLine = 0;
                                int endOfSearchProfilesInLine = 0;

                                foreach (var line in linesWithWordsFromSearchProfile)
                                {
                                    if (request.direction == "top" && lines.IndexOf(line) > 0)
                                        indexesOfLinesToSearchIn.Add(lines.IndexOf(line) - 1);
                                    else if (request.direction == "bottom" && lines.IndexOf(line) < lines.Count - 1)
                                        indexesOfLinesToSearchIn.Add(lines.IndexOf(line) + 1);

                                    startOfSearchProfilesInLine = line.IndexOf(words, StringComparison.OrdinalIgnoreCase);
                                    endOfSearchProfilesInLine = line.IndexOf(words, StringComparison.OrdinalIgnoreCase) + words.Length;

                                    if (indexesOfLinesToSearchIn.Count > 0)
                                    {
                                        foreach (var id in indexesOfLinesToSearchIn)
                                        {
                                            IList<string> wordsList = lines[id].Split(" ");
                                            int lenght = 0;
                                            for (int i = 0; i < request.maxWordsCount;)
                                            {

                                                foreach (var word in wordsList)
                                                {
                                                    int indexOfCharacter = 0;
                                                    foreach (var character in word)
                                                    {
                                                        indexOfCharacter++;
                                                        lenght++;
                                                        if (lenght >= startOfSearchProfilesInLine &&
                                                            lenght <= endOfSearchProfilesInLine)
                                                        {
                                                            if (!result.Contains(word))
                                                                result.Add(word);
                                                        }

                                                        if (indexOfCharacter == word.Length)
                                                            lenght++;
                                                    }
                                                }
                                                i++;
                                            }
                                        }
                                    }
                                }

                        }
                            break;
                        default:
                            return new OkObjectResult("Wrong direction name");
                            break;
                    }
                    return new OkObjectResult(result);
                }
        }
         catch (Exception ex)
            {
                Console.WriteLine("Exception message:");
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
