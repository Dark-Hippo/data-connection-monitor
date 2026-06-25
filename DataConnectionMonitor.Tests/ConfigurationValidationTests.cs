using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace DataConnectionMonitor.Tests
{
    public class ConfigurationValidationTests : IDisposable
    {
        private readonly string _testDirectory;

        public ConfigurationValidationTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void Configuration_ValidMaxRetries_ShouldParse()
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string>
            {
                ["MaxRetries"] = "5"
            });

            // Act
            bool success = int.TryParse(config["MaxRetries"], out int maxRetries);

            // Assert
            Assert.True(success);
            Assert.Equal(5, maxRetries);
        }

        [Fact]
        public void Configuration_InvalidMaxRetries_ShouldFailParsing()
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string>
            {
                ["MaxRetries"] = "invalid"
            });

            // Act
            bool success = int.TryParse(config["MaxRetries"], out int maxRetries);

            // Assert
            Assert.False(success);
            Assert.Equal(0, maxRetries);
        }

        [Fact]
        public void Configuration_ValidPingInterval_ShouldParse()
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string>
            {
                ["PingInterval"] = "3000"
            });

            // Act
            bool success = int.TryParse(config["PingInterval"], out int pingInterval);

            // Assert
            Assert.True(success);
            Assert.Equal(3000, pingInterval);
        }

        [Fact]
        public void Configuration_InvalidPingInterval_ShouldFailParsing()
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string>
            {
                ["PingInterval"] = "not-a-number"
            });

            // Act
            bool success = int.TryParse(config["PingInterval"], out int pingInterval);

            // Assert
            Assert.False(success);
            Assert.Equal(0, pingInterval);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Configuration_EmptyOrNullDisconnectionsFile_ShouldBeInvalid(string? value)
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string?>
            {
                ["DisconnectionsFile"] = value
            });

            // Act
            var disconnectionsFile = config["DisconnectionsFile"];

            // Assert
            Assert.True(string.IsNullOrWhiteSpace(disconnectionsFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Configuration_EmptyOrNullLastSuccessfulConnectionFile_ShouldBeInvalid(string? value)
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string?>
            {
                ["LastSuccessfulConnectionFile"] = value
            });

            // Act
            var lastSuccessfulConnectionsFile = config["LastSuccessfulConnectionFile"];

            // Assert
            Assert.True(string.IsNullOrWhiteSpace(lastSuccessfulConnectionsFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Configuration_EmptyOrNullCurrentStatusFile_ShouldBeInvalid(string? value)
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string?>
            {
                ["CurrentStatusFile"] = value
            });

            // Act
            var currentStatusFile = config["CurrentStatusFile"];

            // Assert
            Assert.True(string.IsNullOrWhiteSpace(currentStatusFile));
        }

        [Fact]
        public void Configuration_ValidFilePaths_ShouldBeValid()
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string>
            {
                ["DisconnectionsFile"] = "output/failures.csv",
                ["LastSuccessfulConnectionFile"] = "output/success.txt",
                ["CurrentStatusFile"] = "output/status.txt"
            });

            // Act & Assert
            Assert.False(string.IsNullOrEmpty(config["DisconnectionsFile"]));
            Assert.False(string.IsNullOrEmpty(config["LastSuccessfulConnectionFile"]));
            Assert.False(string.IsNullOrEmpty(config["CurrentStatusFile"]));
        }

        [Fact]
        public void Configuration_IPAddresses_ValidList_ShouldParse()
        {
            // Arrange
            var jsonConfig = @"{
                ""IPAddresses"": [
                    { ""Address"": ""8.8.8.8"", ""Reference"": ""Google DNS"" },
                    { ""Address"": ""1.1.1.1"", ""Reference"": ""Cloudflare DNS"" }
                ]
            }";
            var config = CreateConfigurationFromJson(jsonConfig);

            // Act
            var ipAddresses = config.GetSection("IPAddresses").Get<List<IPAddress>>();

            // Assert
            Assert.NotNull(ipAddresses);
            Assert.Equal(2, ipAddresses.Count);
            Assert.Equal("8.8.8.8", ipAddresses[0].Address);
            Assert.Equal("Google DNS", ipAddresses[0].Reference);
            Assert.Equal("1.1.1.1", ipAddresses[1].Address);
            Assert.Equal("Cloudflare DNS", ipAddresses[1].Reference);
        }

        [Fact]
        public void Configuration_IPAddresses_EmptyList_ShouldBeInvalid()
        {
            // Arrange
            var jsonConfig = @"{ ""IPAddresses"": [] }";
            var config = CreateConfigurationFromJson(jsonConfig);

            // Act
            var ipAddresses = config.GetSection("IPAddresses").Get<List<IPAddress>>();

            // Assert
            Assert.True(ipAddresses == null || ipAddresses.Count == 0);
        }

        [Fact]
        public void Configuration_IPAddresses_Missing_ShouldBeNull()
        {
            // Arrange
            var config = CreateConfiguration(new Dictionary<string, string>());

            // Act
            var ipAddresses = config.GetSection("IPAddresses").Get<List<IPAddress>>();

            // Assert
            Assert.Null(ipAddresses);
        }

        private IConfiguration CreateConfiguration(Dictionary<string, string?> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values!)
                .Build();
        }

        private IConfiguration CreateConfigurationFromJson(string json)
        {
            var tempFile = Path.Combine(_testDirectory, "test-appsettings.json");
            File.WriteAllText(tempFile, json);

            return new ConfigurationBuilder()
                .AddJsonFile(tempFile)
                .Build();
        }
    }
}