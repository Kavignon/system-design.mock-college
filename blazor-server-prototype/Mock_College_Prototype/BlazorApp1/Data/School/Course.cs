using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data.ValidUsers;

namespace BlazorApp1.Data.School
{
    public class Course
    {
        public string Name { get; }

        public WeeklySchedule Hours { get; }

        public Assessment[] Evaluations { get; }

        public int StudentMaxCapacity { get; }

        public Professor Instructor { get; }

        public string Location { get; }

        public string[] Objectives { get; }

        public string RecommendedReadings { get; }

        public string Code { get; }

        public string? PreRequisites { get; }
    }
}
