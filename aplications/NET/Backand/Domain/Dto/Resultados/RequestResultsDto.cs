using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.RequestResultsDto
{
    public class RequestResultsDto
    {
        public RequestResultsDto() { }
        public RequestResultsDto(string message, bool success, object? data = null)
        {
            Message = message;
            Success = success;
            Data = data;
        }

        public string? Message { get; set; }
        public bool Success { get; set; }
        public object? Data { get; set; }

    }
}
