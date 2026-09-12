using System.Collections.Generic;

namespace Networking.Dtos
{
    public class ProblemDetailsDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}