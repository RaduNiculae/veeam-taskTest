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
    public class ManagerInputTests
    {
        private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.SetMinimumLevel(LogLevel.Debug)
                    .AddConsole()
                    .AddDebug()
        );

        private ILogger<ManagerInput> managerInputLogger = loggerFactory.CreateLogger<ManagerInput>();

        [TestMethod()]
        public void parseInputTest()
        {
            Exception ex;
            string[] validArgs = { "notepad", Int32.MaxValue.ToString() , "1" };
            ManagerInput mInput = ManagerInput.parseInput(validArgs, managerInputLogger);
            Assert.AreEqual(mInput.ProcessName, "notepad");
            Assert.AreEqual(mInput.MaxLifetime, Int32.MaxValue);
            Assert.AreEqual(mInput.MonitorFreq, 60000);

            string[] invalidLenArgs = { "notepad", "1", "1", "1" };
            ex = Assert.ThrowsException<Exception>(() => ManagerInput.parseInput(invalidLenArgs, managerInputLogger));
            Assert.AreEqual(ex.Message, "Incorect number of arguments!");

            string[] invalidProcessNameArgs = { "/aab", "1", "1" };
            ex = Assert.ThrowsException<Exception>(() => ManagerInput.parseInput(invalidProcessNameArgs, managerInputLogger));
            Assert.AreEqual(ex.Message, "Invalid process name!");

            string[] negativeLifetimeArgs = { "notepad", "-1", "1" };
            ex = Assert.ThrowsException<Exception>(() => ManagerInput.parseInput(negativeLifetimeArgs, managerInputLogger));
            Assert.AreEqual(ex.Message, "Maximum lifetime cannot be lower than 0!");

            string[] invalidFreqZeroArgs = { "notepad", "1", "0" };
            ex = Assert.ThrowsException<Exception>(() => ManagerInput.parseInput(invalidFreqZeroArgs, managerInputLogger));
            Assert.AreEqual(ex.Message, "Monitoring frequency cannot be 0 or lower!");

            string[] invalidFreqNegArgs = { "notepad", "1", "-34" };
            ex = Assert.ThrowsException<Exception>(() => ManagerInput.parseInput(invalidFreqNegArgs, managerInputLogger));
            Assert.AreEqual(ex.Message, "Monitoring frequency cannot be 0 or lower!");

            string[] bothNegArgs = { "notepad", "-1", "-34" };
            ex = Assert.ThrowsException<Exception>(() => ManagerInput.parseInput(bothNegArgs, managerInputLogger));
            Assert.AreEqual(ex.Message, "Maximum lifetime cannot be lower than 0!");

            string[] invalidFormatArgs = { "notepad", "hello", "world" };
            Assert.ThrowsException<FormatException>(() => ManagerInput.parseInput(invalidFormatArgs, managerInputLogger));

            string[] overflowPosArgs = { "notepad", ((long)Int32.MaxValue + 1).ToString(), "1" };
            Assert.ThrowsException<OverflowException>(() => ManagerInput.parseInput(overflowPosArgs, managerInputLogger));

            string[] overflowNegArgs = { "notepad", ((long)Int32.MinValue - 1).ToString(), "1" };
            Assert.ThrowsException<OverflowException>(() => ManagerInput.parseInput(overflowNegArgs, managerInputLogger));

            string[] overflowMillisConversionArgs = { "notepad", "1", ((long)Int32.MaxValue / 60000 + 1).ToString()};
            Assert.ThrowsException<OverflowException>(() => ManagerInput.parseInput(overflowMillisConversionArgs, managerInputLogger));
        }
    }
}