using DataConnectionMonitor;
using System;
using System.IO;
using Xunit;

namespace DataConnectionMonitor.Tests
{
    public class FileWriteServiceTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly FileWriteService _fileWriteService;

        public FileWriteServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _fileWriteService = new FileWriteService();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void WriteFailureToFile_ShouldWriteCorrectFormat()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "failures.csv");
            var failureTime = new DateTime(2023, 10, 15, 14, 30, 45);
            var failureDuration = 125.5;

            // Act
            _fileWriteService.WriteFailureToFile(filePath, failureTime, failureDuration);

            // Assert
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.Equal("2023-10-15 14:30:45,125.5\n", content);
        }

        [Fact]
        public void WriteFailureToFile_ShouldAppendToExistingFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "failures.csv");
            var firstFailureTime = new DateTime(2023, 10, 15, 14, 30, 45);
            var secondFailureTime = new DateTime(2023, 10, 15, 15, 45, 30);

            // Act
            _fileWriteService.WriteFailureToFile(filePath, firstFailureTime, 125.5);
            _fileWriteService.WriteFailureToFile(filePath, secondFailureTime, 230.0);

            // Assert
            var lines = File.ReadAllLines(filePath);
            Assert.Equal(2, lines.Length);
            Assert.Equal("2023-10-15 14:30:45,125.5", lines[0]);
            Assert.Equal("2023-10-15 15:45:30,230", lines[1]);
        }

        [Fact]
        public void WriteSuccessToFile_ShouldWriteCurrentTimestamp()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "success.txt");
            var beforeCall = DateTime.Now;

            // Act
            _fileWriteService.WriteSuccessToFile(filePath);

            // Assert
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath).Trim();
            
            // Parse the written timestamp
            Assert.True(DateTime.TryParseExact(content, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var writtenTime));
            
            // Verify the timestamp is reasonable (within a few seconds of when we called the method)
            var afterCall = DateTime.Now;
            Assert.True(writtenTime >= beforeCall.AddSeconds(-1));
            Assert.True(writtenTime <= afterCall.AddSeconds(1));
        }

        [Fact]
        public void WriteSuccessToFile_ShouldOverwriteExistingFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "success.txt");
            File.WriteAllText(filePath, "old content");

            // Act
            _fileWriteService.WriteSuccessToFile(filePath);

            // Assert
            var lines = File.ReadAllLines(filePath);
            Assert.Single(lines);
            // Verify it contains a timestamp, not "old content"
            Assert.True(DateTime.TryParseExact(lines[0], "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _));
        }

        [Fact]
        public void WriteCurrentStatusToFile_Connected_ShouldWriteConnected()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "status.txt");

            // Act
            _fileWriteService.WriteCurrentStatusToFile(filePath, ConnectionState.Connected);

            // Assert
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath).Trim();
            Assert.Equal("Connected", content);
        }

        [Fact]
        public void WriteCurrentStatusToFile_Retrying_ShouldWriteRetrying()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "status.txt");

            // Act
            _fileWriteService.WriteCurrentStatusToFile(filePath, ConnectionState.Retrying);

            // Assert
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath).Trim();
            Assert.Equal("Retrying", content);
        }

        [Fact]
        public void WriteCurrentStatusToFile_Disconnected_ShouldWriteDisconnected()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "status.txt");

            // Act
            _fileWriteService.WriteCurrentStatusToFile(filePath, ConnectionState.Disconnected);

            // Assert
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath).Trim();
            Assert.Equal("Disconnected", content);
        }

        [Fact]
        public void WriteCurrentStatusToFile_ShouldOverwriteExistingFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "status.txt");
            File.WriteAllText(filePath, "old status");

            // Act
            _fileWriteService.WriteCurrentStatusToFile(filePath, ConnectionState.Connected);

            // Assert
            var lines = File.ReadAllLines(filePath);
            Assert.Single(lines);
            Assert.Equal("Connected", lines[0]);
        }

        [Theory]
        [InlineData("C:\\temp\\test.csv")]
        [InlineData("/tmp/test.csv")]
        [InlineData("./relative/path/test.csv")]
        public void WriteFailureToFile_ShouldHandleDifferentPaths(string filePath)
        {
            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                // Arrange
                var failureTime = DateTime.Now;
                var failureDuration = 100.0;

                // Act & Assert (should not throw)
                _fileWriteService.WriteFailureToFile(filePath, failureTime, failureDuration);
                
                if (File.Exists(filePath))
                {
                    Assert.True(File.ReadAllText(filePath).Length > 0);
                }
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory) && 
                    directory != "C:\\temp" && directory != "/tmp")
                {
                    try { Directory.Delete(directory, true); } catch { }
                }
            }
        }
    }
}