
using PetCare.Shared.DTOs.DTOs;
using System;

namespace PetCare.Shared.DTOs.Utils
{
    public static class ExceptionMapper
    {
        public static ExceptionDto Map(Exception ex)
        {
            return new ExceptionDto
            {
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                ExceptionType = ex.GetType().FullName
            };
        }
    }
}
