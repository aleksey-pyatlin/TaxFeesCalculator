using CommandLine;
using CommandLine.Text;

namespace FeesCalculator.ConsoleApplication
{
    public class RunnerOptions
    {
        [Option('p', "profile", Required = true,
            HelpText = "Your profile that contains payments configuration.")]
        public string Profile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}