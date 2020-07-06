using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data.School;

namespace BlazorApp1.Data.ValidUsers
{
    public sealed class Student : ValidUser
    {
        public Student(CommonUserInformation userInformation, Gpa gpa, int completedCredits, int enrolledCredits) 
            : base(userInformation)
        {
            this.GPA = gpa;
            this.CompletedCreditCount = completedCredits;
            this.EnrolledCreditCount = enrolledCredits;
        }

        public Gpa GPA { get; }

        public int CompletedCreditCount { get; }

        public int EnrolledCreditCount { get; }
    }
}
