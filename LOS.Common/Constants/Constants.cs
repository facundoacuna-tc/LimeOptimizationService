using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Common.Constants
{
    public static class Constants
    {
        public const string AUTHORIZATION = "Authorization";
        public const string TOKEN_PREFIX = "Bearer";
        public const string VERSION_INFO = "VersionInfo";
        public const int ONE_DAY_IN_SECONDS = 24 * 60 * 60;
        public const int REFRESH_VALID_DAYS = 90;
    }
}
