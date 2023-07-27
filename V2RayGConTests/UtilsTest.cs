using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using static V2RayGCon.Misc.Utils;

namespace V2RayGCon.Test
{
    [TestClass]
    public class UtilsTest
    {

        public UtilsTest()
        {

        }

        [DataTestMethod]
        [DataRow("hello, world!")]
        [DataRow("he中llo780, wo文rld!123")]
        [DataRow("😀😀😣👨‍🦰🎗🥙🛴❣")]
        [DataRow("")]
        [DataRow("\uD83C\uDCBA")]
        public void ClumsyWriterTest(string s)
        {
            var f1 = "clumsy-writer-main-test.txt";
            var f2 = "clumsy-writer-bak-test.txt";

            var us = new Models.Datas.UserSettings();
            us.DecodeCache = s;
            ClumsyWriter(us, null, null, f1, f2);

            var us1 = LoadUserSettingsFromFile(f1);
            var us2 = LoadUserSettingsFromFile(f2);

            Assert.AreEqual(us.DecodeCache, us1.DecodeCache);
            Assert.AreEqual(us.DecodeCache, us2.DecodeCache);
        }

        Models.Datas.UserSettings LoadUserSettingsFromFile(string filename)
        {
            var content = File.ReadAllText(filename);
            var result = JsonConvert.DeserializeObject<Models.Datas.UserSettings>(content);
            return result;
        }
    }
}
