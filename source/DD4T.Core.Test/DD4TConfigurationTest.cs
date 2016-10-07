using DD4T.ContentModel.Contracts.Configuration;
using DD4T.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DD4T.Core.Test
{
    [TestClass]
    public class DD4TConfigurationTest
    {

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            SetCacheSettings();
        }


        [TestMethod]
        public void GetSetting()
        {
            DD4TConfiguration _configuration = new DD4TConfiguration();
            string jmsTopic = _configuration.JMSTopic;
            Assert.AreEqual(jmsTopic, "TestJMSTopic");
        }

        [TestMethod]
        public void GetExpirationForCacheRegion()
        {
           ThreadStart work = GetExpirationForCacheRegionExecute;
            Thread thread1 = new Thread(work);
            Thread thread2 = new Thread(work);
            thread1.Start();
            thread2.Start();

        }
        private void GetExpirationForCacheRegionExecute()
        {
            DD4TConfiguration _configuration = new DD4TConfiguration();

            int exp4page = _configuration.GetExpirationForCacheRegion("Page");
            int exp4component = _configuration.GetExpirationForCacheRegion("ComponentPresentation");
            int exp4link = _configuration.GetExpirationForCacheRegion("Link");

            Assert.AreEqual(exp4page, 60);
            Assert.AreEqual(exp4component, 61);
            Assert.AreEqual(exp4link, 62);
        }
        private static void SetCacheSettings()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(ConfigurationKeys.JMSTopic);
            config.AppSettings.Settings.Remove(string.Format(ConfigurationKeys.CacheSettingsPerRegion, "Page"));
            config.AppSettings.Settings.Remove(string.Format(ConfigurationKeys.CacheSettingsPerRegion, "ComponentPresentation"));
            config.AppSettings.Settings.Remove(string.Format(ConfigurationKeys.CacheSettingsPerRegion, "Link"));
            config.AppSettings.Settings.Add(ConfigurationKeys.JMSTopic, "TestJMSTopic");
            config.AppSettings.Settings.Add(string.Format(ConfigurationKeys.CacheSettingsPerRegion, "Page"), "60");
            config.AppSettings.Settings.Add(string.Format(ConfigurationKeys.CacheSettingsPerRegion, "ComponentPresentation"), "61");
            config.AppSettings.Settings.Add(string.Format(ConfigurationKeys.CacheSettingsPerRegion, "Link"), "62");
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
