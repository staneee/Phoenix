using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Phoenix.Helpers
{
    public class ApplicationHelper
    {
        #region Version()

        /// <summary>
        ///     The version.
        /// </summary>
        private static string version;

        /// <summary>
        /// Returns the Phoenix.NET version information.
        /// </summary>
        /// <value>
        /// The Phoenix.NET major, minor, revision, and build numbers.
        /// </value>
        /// <remarks>
        /// The current version is determined by extracting the build version of the Phoenix.Core assembly.
        /// </remarks>
        /// <returns>
        /// The version.
        /// </returns>
        public static string Version()
        {
            return version ?? (version = Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        #endregion
    }
}
