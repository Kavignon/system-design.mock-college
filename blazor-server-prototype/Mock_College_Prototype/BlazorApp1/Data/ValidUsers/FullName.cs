using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data.Helpers;

namespace BlazorApp1.Data.ValidUsers
{
    public class FullName
    {
        public FullName(string firstName, string? middleName, string lastName) =>
            this.Value = FullNameCreator.GenerateFullName(firstName, middleName, lastName);

        public string Value { get; }
    }
}
