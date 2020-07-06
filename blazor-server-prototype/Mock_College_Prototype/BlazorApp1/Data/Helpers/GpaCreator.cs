using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data.School;

namespace BlazorApp1.Data.Helpers
{
    public class GpaCreator
    {
        public static Gpa MakeAverageGpaFromPastResult(IReadOnlyCollection<float> studentPastGrades)
        {
            return new Gpa(studentPastGrades.Average());
        }
    }
}
