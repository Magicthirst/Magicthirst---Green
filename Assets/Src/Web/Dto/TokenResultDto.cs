using System;

namespace Web.Dto
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    internal sealed record TokenResultDto(string token);
}