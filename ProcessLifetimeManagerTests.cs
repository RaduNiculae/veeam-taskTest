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
    public class ProcessLifetimeManagerTests
    {
        private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.SetMinimumLevel(LogLevel.Debug)
                    .AddConsole()
                    .AddDebug()
        );

        private ILogger<ManagerInput> managerInputLogger = loggerFactory.CreateLogger<ManagerInput>();


        [TestMethod()]
        public void ProcessLifetimeManagerTest()
        {
            // Test same reference.
            string[] validArgs1 = { "notepad", "1", "1" };
            ManagerInput mInput = ManagerInput.parseInput(validArgs1, managerInputLogger);
            ProcessLifetimeManager manager1 = new ProcessLifetimeManager(
                mInput,
                loggerFactory.CreateLogger<ProcessLifetimeManager>()
            );
            ProcessLifetimeManager manager2 = new ProcessLifetimeManager(
                mInput,
                loggerFactory.CreateLogger<ProcessLifetimeManager>()
            );
            Assert.IsTrue(manager1.Equals(manager2));

            // Test different reference.
            string[] validArgs3 = { "notepad", "1", "1" };
            ManagerInput mInput3 = ManagerInput.parseInput(validArgs3, managerInputLogger);
            ProcessLifetimeManager manager3 = new ProcessLifetimeManager(
                mInput3,
                loggerFactory.CreateLogger<ProcessLifetimeManager>()
            );
            Assert.IsTrue(manager1.Equals(manager3));

            // Test not equal.
            string[] validArgs4 = { "notepad2", "2", "1" };
            ManagerInput mInput4 = ManagerInput.parseInput(validArgs4, managerInputLogger);
            ProcessLifetimeManager manager4 = new ProcessLifetimeManager(
                mInput4,
                loggerFactory.CreateLogger<ProcessLifetimeManager>()
            );
            Assert.IsFalse(manager1.Equals(manager4));
        }
    }
}