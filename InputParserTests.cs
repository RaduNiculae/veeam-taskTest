using Microsoft.VisualStudio.TestTools.UnitTesting;
using veeam_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace veeam_task.Tests
{
    [TestClass()]
    public class InputParserTests
    {
        private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.SetMinimumLevel(LogLevel.Debug)
                       .AddConsole()
                       .AddDebug()
            );

        private InputParser parser = new InputParser(loggerFactory.CreateLogger<InputParser>());

        [TestMethod()]
        public void parseInputTest()
        {
            Exception ex;
            string[] validArgs = { "notepad", Int32.MaxValue.ToString() , "1" };
            ManagerInput mInput = new ManagerInput("notepad", Int32.MaxValue, 60000);
            Assert.IsTrue(parser.parseInput(validArgs).Equals(mInput));

            string[] invalidLenArgs = { "notepad", "1", "1", "1" };
            ex = Assert.ThrowsException<Exception>(() => parser.parseInput(invalidLenArgs));
            Assert.AreEqual(ex.Message, "Incorect number of arguments!");

            string[] negativeLifetimeArgs = { "notepad", "-1", "1" };
            ex = Assert.ThrowsException<Exception>(() => parser.parseInput(negativeLifetimeArgs));
            Assert.AreEqual(ex.Message, "Maximum lifetime cannot be lower than 0!");

            string[] invalidFreqZeroArgs = { "notepad", "1", "0" };
            ex = Assert.ThrowsException<Exception>(() => parser.parseInput(invalidFreqZeroArgs));
            Assert.AreEqual(ex.Message, "Monitoring frequency cannot be 0 or lower!");

            string[] invalidFreqNegArgs = { "notepad", "1", "-34" };
            ex = Assert.ThrowsException<Exception>(() => parser.parseInput(invalidFreqNegArgs));
            Assert.AreEqual(ex.Message, "Monitoring frequency cannot be 0 or lower!");

            string[] bothNegArgs = { "notepad", "-1", "-34" };
            ex = Assert.ThrowsException<Exception>(() => parser.parseInput(bothNegArgs));
            Assert.AreEqual(ex.Message, "Maximum lifetime cannot be lower than 0!");

            string[] invalidFormatArgs = { "notepad", "hello", "world" };
            Assert.ThrowsException<FormatException>(() => parser.parseInput(invalidFormatArgs));

            string[] overflowPosArgs = { "notepad", ((long)Int32.MaxValue + 1).ToString(), "1" };
            Assert.ThrowsException<OverflowException>(() => parser.parseInput(overflowPosArgs));

            string[] overflowNegArgs = { "notepad", ((long)Int32.MinValue - 1).ToString(), "1" };
            Assert.ThrowsException<OverflowException>(() => parser.parseInput(overflowNegArgs));

            string[] overflowMillisConversionArgs = { "notepad", "1", ((long)Int32.MaxValue / 60000 + 1).ToString()};
            Assert.ThrowsException<OverflowException>(() => parser.parseInput(overflowMillisConversionArgs));


        }
    }
}