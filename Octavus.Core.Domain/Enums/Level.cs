using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Domain.Enums
{
    public enum Level
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public static class ActivityLevelExtensions
    {
        public static Level FromString(string value)
        {
            if (Enum.TryParse<Level>(value, true, out var result))
                return result;

            throw new ArgumentException($"Invalid value for ActivityLevel: {value}");
        }
    }
}
