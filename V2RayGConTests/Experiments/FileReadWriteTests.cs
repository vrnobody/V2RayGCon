using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace V2RayGCon.Test.Experiments
{
    [TestClass]
    public class FileReadWriteTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var us = new Models.Datas.UserSettings();
            var content = JsonConvert.SerializeObject(us);
            string mainUsFilename = "mainUserSettingsTest.json";
            string bakUsFilename = "bakUserSettingsTest.bak";
            Stopwatch sw = new Stopwatch();
            string readBak = null;
            sw.Restart();
            File.WriteAllText(mainUsFilename, content);
            var readMain = File.ReadAllText(mainUsFilename);
            if (readMain == content)
            {
                File.WriteAllText(bakUsFilename, content);
                readBak = File.ReadAllText(bakUsFilename);
            }
            sw.Stop();
            Console.WriteLine("Time span: " + sw.ElapsedMilliseconds);
            Assert.AreEqual(readBak, content);
        }
    }
}
