using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Data.Helpers
{
    public static class ValidDateRangeChecker
    {
        private static readonly DateTime SchoolFoundedDate = new DateTime(1977, 5, 23);

        public static bool IsStartDateValid(DateTime entryTime)
        {
            return entryTime switch
            {
                var valid when valid >= SchoolFoundedDate && valid <= DateTime.Today => true,
                var extreme when extreme == default ||  extreme == DateTime.MinValue || extreme == DateTime.MaxValue =>
                    throw new Exception("Time cannot be in an extreme value."),
                var priorToBeingFunded when priorToBeingFunded < SchoolFoundedDate => false,
                var valueBiggerThanToday when valueBiggerThanToday > DateTime.Today => false,
                _ => false
            };
        }
    }
}
