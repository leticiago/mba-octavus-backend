using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Domain.Enums
{
    public enum ActivityType
    {
        QuestionAndAnswer,
        DragAndDrop,
        OpenText
    }

    public static class ActivityTypeExtensions
    {
        public static ActivityType FromString(string value)
        {
            if (Enum.TryParse<ActivityType>(value, true, out var result))
                return result;

            throw new ArgumentException($"Invalid value for ActivityType: {value}");
        }
    }
}
