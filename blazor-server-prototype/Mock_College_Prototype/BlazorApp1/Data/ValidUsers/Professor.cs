using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Data.ValidUsers
{
    public sealed class Professor : ValidUser
    {
        public Professor(CommonUserInformation userInformation, int supervisedStudentCount) 
            : base(userInformation)
        {
            this.SupervisedStudentCountForSemester = supervisedStudentCount;
        }

        public int SupervisedStudentCountForSemester { get; }
    }
}
