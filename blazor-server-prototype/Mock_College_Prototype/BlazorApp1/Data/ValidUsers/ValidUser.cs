using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data.Helpers;
using BlazorApp1.Data.School;

namespace BlazorApp1.Data.ValidUsers
{
    public abstract class ValidUser
    {
        protected ValidUser(CommonUserInformation userInformation)
        {
            this.UserInformation = userInformation ?? throw new ArgumentNullException(nameof(userInformation));
        }

        public CommonUserInformation UserInformation { get; }
    }
}
