using System;
using System.Collections.Generic;
using System.Text;

namespace ConBot.Configuration
{
    public class PromptConfig
    {
        public const string SectionName = "PromptSettings";

        public string OsContext { get; set; } = "Generic Linux/Windows";
        public string Verbosity { get; set; } = "short";
    }
}
