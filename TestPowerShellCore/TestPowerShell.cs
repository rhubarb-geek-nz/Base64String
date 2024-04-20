// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

#if NETCOREAPP
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace RhubarbGeekNz.Base64String
{
    [TestClass]
    public class UnitTests
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
        public UnitTests()
        {
            foreach (Type t in new Type[] {
                typeof(ConvertToBase64String),
                typeof(ConvertFromBase64String)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                if (ca == null) throw new NullReferenceException();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop action"));
        }

        [TestMethod]
        public void TestConvertToBase64String()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("New-Object Byte[] -ArgumentList @(,256) | ConvertTo-Base64String");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual(344, outputPipeline[0].BaseObject.ToString().Length);
            }
        }

        [TestMethod]
        public void TestConvertToAndFromBase64String()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("@(,(New-Object Byte[] -ArgumentList @(,10000))) | ConvertTo-Base64String | ConvertFrom-Base64String");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                Assert.AreEqual(10000, ((byte[])outputPipeline[0].BaseObject).Length);
            }
        }

        [TestMethod]
        public void TestHelloWorld()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("@(,[System.Text.Encoding]::ASCII.GetBytes('Hello World')) | ConvertTo-Base64String | ConvertFrom-Base64String");

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                string result = System.Text.Encoding.ASCII.GetString((byte[])outputPipeline[0].BaseObject);

                Assert.AreEqual("Hello World", result);
            }
        }

        [TestMethod]
        public void TestRandomData()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {

                byte[] bytes = new byte[10000];

                new Random().NextBytes(bytes);

                powerShell.AddScript(
                        "Param([byte[]]$bytes)" + Environment.NewLine +
                        "@(,$bytes) | ConvertTo-Base64String | ConvertFrom-Base64String").AddArgument(bytes);

                var outputPipeline = powerShell.Invoke();

                Assert.AreEqual(1, outputPipeline.Count);

                byte[] result = (byte[])outputPipeline[0].BaseObject;

                Assert.AreEqual(bytes.Length, result.Length);

                for (int i = 0; i < bytes.Length; i++)
                {
                    Assert.AreEqual(bytes[i], result[i]);
                }
            }
        }

        [TestMethod]
        public void TestBadData()
        {
            bool caught = false;
            string exName = null;

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                try
                {
                    powerShell.AddScript("'!$%^&*()-_=+:;<>,.?/#~@][{}' | ConvertFrom-Base64String");
                    powerShell.Invoke();
                }
                catch (ActionPreferenceStopException ex)
                {
                    exName = ex.ErrorRecord.Exception.GetType().Name;
                    caught = ex.ErrorRecord.Exception is FormatException;
                }
            }

            Assert.IsTrue(caught, exName);
        }

        [TestMethod]
        public void TestConvertToWithNull()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$null | ConvertTo-Base64String");
                var outputPipeline = powerShell.Invoke();
                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsNull(outputPipeline[0]);
            }
        }

        [TestMethod]
        public void TestConvertFromWithNull()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("$null | ConvertFrom-Base64String");
                var outputPipeline = powerShell.Invoke();
                Assert.AreEqual(1, outputPipeline.Count);
                Assert.IsNull(outputPipeline[0]);
            }
        }

        [TestMethod]
        public void TestConvertToWithEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("New-Object -TypeName byte[] -ArgumentList 0 | ConvertTo-Base64String");
                var outputPipeline = powerShell.Invoke();
                Assert.AreEqual(1, outputPipeline.Count);
                Assert.AreEqual(0, outputPipeline[0].BaseObject.ToString().Length);
            }
        }

        [TestMethod]
        public void TestConvertFromWithEmpty()
        {
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddScript("'' | ConvertFrom-Base64String");
                var outputPipeline = powerShell.Invoke();
                Assert.AreEqual(1, outputPipeline.Count);
                byte[] result = (byte[])outputPipeline[0].BaseObject;
                Assert.AreEqual(0, result.Length);
            }
        }
    }
}
