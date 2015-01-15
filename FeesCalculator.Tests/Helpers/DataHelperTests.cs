using System;
using System.IO;
using FeesCalculator.ConsoleApplication.Adapters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FeesCalculator.Tests.Helpers
{
    [TestFixture]
    public class DataHelperTests
    {
        [TestCase("4_Quarter_2014.json")]
        //[Ignore]
        public void FixDataTest(String fileName)
        {
            const string root = @".\..\..\..\FeesCalculator.ConsoleApplication\Data\SimpleAdapter";
            JsonPaymentAdapter jsonPaymentAdapter = new JsonPaymentAdapter();
            var container  = jsonPaymentAdapter.GetContainer(
                root, fileName + ".bak");

            var correlation = Int32.Parse(File.ReadAllLines(Path.Combine(root, "correlation.bak"))[0]);
            foreach (var message in container.GetMessages())
            {
                var real = message.Amount;
                message.Amount = message.Amount - correlation;
                Console.WriteLine("{0} => {1}", real, message.Amount);
            }

            var setttings = jsonPaymentAdapter.GetJsonSettigns();
            setttings.Formatting = Formatting.Indented;

            File.WriteAllText(Path.Combine(root, fileName), 
                JsonConvert.SerializeObject(container, setttings)); 
        }
    }
}