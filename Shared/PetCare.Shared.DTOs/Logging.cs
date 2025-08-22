using System.Text.Json;

using Microsoft.Extensions.Logging;
using PetCare.Shared.DTOs.Utils;

namespace Common.Logging
{
    public class ErrorLogger
    {
        private readonly ILogger<ErrorLogger> _logger;

        public ErrorLogger(ILogger<ErrorLogger> logger)
        {
            _logger = logger;
        }

        public void Log(Exception ex)
        {
            var dto = ExceptionMapper.Map(ex);
            var json = JsonSerializer.Serialize(dto);
            _logger.LogError(json);
        }
    }
}
