﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix.Data.Models
{
    /// <summary>
    /// Lookups
    /// </summary>
    public class Lookups
    {
        /// <summary>
        /// Cultures supported by BE
        /// </summary>
        public IEnumerable<SelectOption> Cultures { get; set; }
        /// <summary>
        /// Roles for self registration
        /// </summary>
        public IEnumerable<SelectOption> SelfRegisterRoles { get; set; }
        /// <summary>
        /// Page list
        /// </summary>
        public IEnumerable<SelectOption> PageList { get; set; }
        /// <summary>
        /// List of blog authors
        /// </summary>
        public IEnumerable<SelectOption> AuthorList { get; set; }
        /// <summary>
        /// Category list
        /// </summary>
        public IEnumerable<SelectOption> CategoryList { get; set; }
        /// <summary>
        /// List of installed themes
        /// </summary>
        public IEnumerable<SelectOption> InstalledThemes { get; set; }

        public EditorOptions PostOptions { get; set; }
        public EditorOptions PageOptions { get; set; }
    }
}
