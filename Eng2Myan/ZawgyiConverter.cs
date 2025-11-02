using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Eng2Myan
{
    /// <summary>
    /// A C# port of the Zawgyi-to-Unicode conversion logic
    /// based on the provided Python script.
    /// This is a non-static class, so you can initialize it.
    /// </summary>
    public class ZawgyiConverter
    {
        // Conversion rules ported from the Python script
        // Note: BOTH strings are now regular strings to allow
        // C# to process \uXXXX Unicode escapes in patterns and replacements.
        private readonly List<string[]> _rules = new List<string[]>()
        {
            // Regex patterns (rule[0]) are now regular strings, not verbatim (@"")
            new[] { "(\u103d|\u1087)", "\u103e" },
            new[] { "\u103c", "\u103d" },
            new[] { "(\u103b|\u107e|\u107f|\u1080|\u1081|\u1082|\u1083|\u1084)", "\u103c" },
            new[] { "(\u103a|\u107d)", "\u103b" },
            new[] { "\u1039", "\u103a" },
            new[] { "\u106a", "\u1009" },
            new[] { "\u106b", "\u100a" },
            new[] { "\u106c", "\u1039\u100b" },
            new[] { "\u106d", "\u1039\u100c" },
            new[] { "\u106e", "\u100d\u1039\u100d" },
            new[] { "\u106f", "\u100d\u1039\u100e" },
            new[] { "\u1070", "\u1039\u100f" },
            new[] { "(\u1071|\u1072)", "\u1039\u1010" },
            new[] { "\u1060", "\u1039\u1000" },
            new[] { "\u1061", "\u1039\u1001" },
            new[] { "\u1062", "\u1039\u1002" },
            new[] { "\u1063", "\u1039\u1003" },
            new[] { "\u1065", "\u1039\u1005" },
            new[] { "\u1068", "\u1039\u1007" },
            new[] { "\u1069", "\u1039\u1008" },
            new[] { "(\u1073|\u1074)", "\u1039\u1011" },
            new[] { "\u1075", "\u1039\u1012" },
            new[] { "\u1076", "\u1039\u1013" },
            new[] { "\u1077", "\u1039\u1014" },
            new[] { "\u1078", "\u1039\u1015" },
            new[] { "\u1079", "\u1039\u1016" },
            new[] { "\u107a", "\u1039\u1017" },
            new[] { "\u107c", "\u1039\u1019" },
            new[] { "\u1085", "\u1039\u101c" },
            new[] { "\u1033", "\u102f" },
            new[] { "\u1034", "\u1030" },
            new[] { "\u103f", "\u1030" },
            new[] { "\u1086", "\u103f" },
            new[] { "\u1036\u1088", "\u1088\u1036" },
            new[] { "\u1088", "\u103e\u102f" },
            new[] { "\u1089", "\u103e\u1030" },
            new[] { "\u108a", "\u103d\u103e" },
            new[] { "([\u1000-\u1021])\u1064", "\u1004\u103a\u1039\\1" },
            new[] { "([\u1000-\u1021])\u108b", "\u1004\u103a\u1039\\1\u102d" },
            new[] { "([\u1000-\u1021])\u108c", "\u1004\u103a\u1039\\1\u102e" },
            new[] { "([\u1000-\u1021])\u108d", "\u1004\u103a\u1039\\1\u1036" },
            new[] { "\u108e", "\u102d\u1036" },
            new[] { "\u108f", "\u1014" },
            new[] { "\u1090", "\u101b" },
            new[] { "\u1091", "\u100f\u1039\u1091" },
            new[] { "\u1019\u102c(\u107b|\u1093)", "\u1019\u1039\u1018\u102c" },
            new[] { "(\u107b|\u1093)", "\u103a\u1018" },
            new[] { "(\u1094|\u1095)", "\u1037" },
            new[] { "\u1096", "\u1039\u1010\u103d" },
            new[] { "\u1097", "\u100b\u1039\u100b" },
            new[] { "\u103c([\u1000-\u1021])([\u1000-\u1021])?", "\\1\u103c\\2" },
            new[] { "([\u1000-\u1021])\u103c\u103a", "\u103c\\1\u103a" },
            new[] { "\u1031([\u1000-\u1021])(\u103e)?(\u103b)?", "\\1\\2\\3\u1031" },
            new[] { "([\u1000-\u1021])\u1031([\u103b\u103c\u103d\u103e]+)", "\\1\\2\u1031" },
            new[] { "\u1032\u103d", "\u103d\u1032" },
            new[] { "\u103d\u103b", "\u103b\u103d" },
            new[] { "\u103a\u1037", "\u1037\u103a" },
            new[] { "\u102f(\u102d|\u102e|\u1036|\u1037)\u102f", "\u102f\\1" },
            new[] { "\u102f\u102f", "\u102f" },
            new[] { "(\u102f|\u1030)(\u102d|\u102e)", "\\2\\1" },
            new[] { "(\u103e)(\u103b|\u1037)", "\\2\\1" },
            new[] { "\u1025(\u103a|\u102c)", "\u1009\\1" },
            new[] { "\u1025\u102e", "\u1026" },
            new[] { "\u1005\u103b", "\u1008" },
            new[] { "\u1036(\u102f|\u1030)", "\\1\u1036" },
            new[] { "\u1031\u1037\u103e", "\u103e\u1031\u1037" },
            new[] { "\u1031\u103e\u102c", "\u103e\u1031\u102c" },
            new[] { "\u105a", "\u102b\u103a" },
            new[] { "\u1031\u103b\u103e", "\u103b\u103e\u1031" },
            new[] { "(\u102d|\u102e)(\u103d|\u103e)", "\\2\\1" },
            new[] { "\u102c\u1039([\u1000-\u1021])", "\u1039\\1\u102c" },
            new[] { "\u103c\u1004\u103a\u1039([\u1000-\u1021])", "\u1004\u103a\u1039\\1\u103c" },
    // This was line 90.
    new[] { "\u1039\u103c\u103a\u1039([\u1000-\u1021])", "\u103a\u1039\\1\u103c" },
            new[] { "\u103c\u1039([\u1000-\u1021])", "\u1039\\1\u103c" },
            new[] { "\u1036\u1039([\u1000-\u1021])", "\u1039\\1\u1036" },
            new[] { "\u1092", "\u100b\u1039\u100c" },
            new[] { "\u104e", "\u104e\u1004\u103a\u1038" },
            new[] { "\u1040(\u102b|\u102c|\u1036)", "\u101d\\1" },
            new[] { "\u1025\u1039", "\u1009\u1039" },
            new[] { "([\u1000-\u1021])\u103c\u1031\u103d", "\\1\u103c\u103d\u1031" },
            new[] { "([\u1000-\u1021])\u103d\u1031\u103b", "\\1\u103b\u103d\u1031" }
        };

        /// <summary>
        /// Converts a Zawgyi-encoded string to a Unicode-encoded string.
        /// </summary>
        /// <param name="zawgyiText">The Zawgyi text to convert.</param>
        /// <returns>The Unicode equivalent string.</returns>
        public string ToUnicode(string zawgyiText)
        {
            if (string.IsNullOrEmpty(zawgyiText))
            {
                return zawgyiText;
            }

            string unicodeText = zawgyiText;
            foreach (var rule in _rules)
            {
                // C# Regex uses $1, $2 etc. for backreferences.
                // This replaces the Python/JS backslash \1 with C#'s $1
                string replacement = rule[1].Replace(@"\", @"$");
                unicodeText = Regex.Replace(unicodeText, rule[0], replacement);
            }
            return unicodeText;
        }
    }
}



