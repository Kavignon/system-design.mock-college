using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Data.Helpers
{
    public static class FullNameCreator
    {
        public static string GenerateFullName(string firstName, string? middleName, string lastName)
        {
            return (firstName, middleName, lastName) switch
            {
                var (missing, _, _) when string.IsNullOrEmpty(missing) => throw new ArgumentNullException(nameof(firstName)),
                var (_, _, missing) when string.IsNullOrEmpty(missing) => throw new ArgumentNullException(nameof(lastName)),
                var (f, m, l) when f.Length >= 1 && l.Length >= 1 => $"{f} {m} {l}"
            };
        }

    }
}
