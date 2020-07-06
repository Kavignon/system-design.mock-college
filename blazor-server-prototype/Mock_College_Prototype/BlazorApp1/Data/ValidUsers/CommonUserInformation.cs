using System;
using BlazorApp1.Data.Helpers;
using BlazorApp1.Data.School;

namespace BlazorApp1.Data.ValidUsers
{
    public sealed class CommonUserInformation
    {
       public CommonUserInformation(CollegeCode code, FullName fullName, Course[] semesterCourses, DateTime startDate)
        {
            this.Code = code ?? throw new ArgumentNullException(nameof(code));

            this.Name = fullName ?? throw new ArgumentNullException(nameof(fullName));

            this.SemesterCourses = semesterCourses ?? throw new ArgumentNullException(nameof(semesterCourses));

            this.StartDate = ValidDateRangeChecker.IsStartDateValid(startDate)
                ? startDate
                : throw new Exception($"Not valid entry time ({startDate}) in the database.");
        }

        public CollegeCode Code { get; }

        public FullName Name { get; }

        public Course[] SemesterCourses { get; }

        public DateTime StartDate { get; }
}
}
