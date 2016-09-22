using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DD4T.Core.Test
{
    public class ConfigurationTest : BaseFactoryTest
    {
        static ConfigurationTest()
        {
            Initialize();
        }

        [Fact]
        public void LoadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dd4t-configuration.json");

            var config = configuration.Build();

            var publicationId = config.GetValue<int>("PublicationId");
            Assert.Equal(publicationId, 8);
            //var builder = new ConfigurationBuilder().add
            //    .AddJsonFile("config.json");
        }
    }
}