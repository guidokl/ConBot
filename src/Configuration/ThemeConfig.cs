using System;
using System.Collections.Generic;
using System.Text;

namespace ConBot.Configuration
{
    public class ThemeConfig
    {
        // https://spectreconsole.net/console/reference/color-reference

        public const string SectionName = "ThemeSettings";

        public string PanelBorder { get; set; } = "teal";
        public string BlockCode { get; set; } = "lightsalmon1";
        public string InlineCode { get; set; } = "blue";
        public string Highlight { get; set; } = "DeepPink1";
        public string Header { get; set; } = "yellow";
    }
}
