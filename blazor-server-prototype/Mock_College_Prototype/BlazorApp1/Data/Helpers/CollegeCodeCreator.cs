using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data.ValidUsers;

namespace BlazorApp1.Data.Helpers
{
    public static class CollegeCodeCreator
    {
        private const int LengthOfStudentPrefix = 3;
        private const int LengthOfProfessorPrefix = 4;
        private const int LengthOfUniversalIdentifier = 5;

        private const string StudentPrefix = "STU";
        private const string ProfessorPrefix = "PROF";

        private static bool IsLengthValid(string str, int maxLength) => str.Length == maxLength;

        private static bool IsUserPrefixValid<T>(string userPrefix, T role) where T : ValidUser
        {
            return role switch
            {
                Student _ when DoesUserLookValid(userPrefix, LengthOfStudentPrefix, StudentPrefix) => true,
                Professor _ when DoesUserLookValid(userPrefix, LengthOfProfessorPrefix, ProfessorPrefix) => true,
                _ => false
            };

            static bool DoesUserLookValid(string prefix, int prefixLength, string platformPrefix)
                => IsLengthValid(prefix, prefixLength) && prefix == platformPrefix;
        }

        // Do not forget to validate the universal code. This could easily be check by making the universal ID a column of the user column.
        public static CollegeCode MakeCodeFromInput(string prefix, string universalIdentifier, ValidUser role)
        {
            return (string.IsNullOrEmpty(prefix), string.IsNullOrEmpty(universalIdentifier), IsUserPrefixValid(prefix, role), IsLengthValid(universalIdentifier, LengthOfUniversalIdentifier)) switch
            {
                (false, false, true, true) => new CollegeCode(),
                _ => throw new Exception("An error occured while finding the college code.")
            };
        }
    }
}
