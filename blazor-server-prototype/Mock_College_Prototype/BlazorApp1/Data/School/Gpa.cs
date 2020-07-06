using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Data.School
{
    public class Gpa
    {
        public Gpa(float averageValue)
        {
            this.Value = averageValue;
        }

        public float Value { get; }
    }
}
