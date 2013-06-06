using System;
using CommandLine;
using CommandLine.Text;

namespace csvvaluecounter
{
    /// <summary>
    /// Internal class used for the command line parsing
    /// </summary>
    internal class Options
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('i', "input", Required = true, DefaultValue = "", HelpText = "Input file")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, DefaultValue = "", HelpText = "Output file")]
        public string Output { get; set; }

        [Option('f', "field", Required = true, HelpText = "CSV field number")]
        public int Field { get; set; }

        [Option('h', "hasheaders", Required = false, DefaultValue = true, HelpText = "CSV file has headers? Defaults to true")]
        public bool HasHeaders { get; set; }

        [Option('d', "delimiter", Required = false, DefaultValue = '\t', HelpText = "The delimiter used for the export. Defaults to \"\t")]
        public char Delimiter { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Copyright = new CopyrightInfo("woanware", 2013),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage: csvvaluecounter -i mft.csv -o counts.txt");
            help.AddOptions(this);

            if (this.LastParserState.Errors.Count > 0)
            {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }
            }

            return help;
        }
    }
}
