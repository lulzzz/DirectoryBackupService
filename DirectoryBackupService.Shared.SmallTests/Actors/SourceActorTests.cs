using Akka.Actor;
using Akka.TestKit.Xunit2;
using ConnelHooley.AkkaTestingHelpers;
using DirectoryBackupService.Shared.Actors;
using System;
using System.IO;
using Xunit;
using DirectoryBackupService.Shared.Models;
using System.Text;
using FluentAssertions;
using ConnelHooley.TestHelpers;
using System.Threading;

namespace DirectoryBackupService.Shared.SmallTests.Actors
{
    public class SourceActorTests : TestKit
    {
        private readonly string _directory;
        private readonly string _filePath1;
        private readonly string _filePath2;

        public SourceActorTests()
        {
            const string fileName1 = "File1.txt";
            const string fileName2 = "File2.txt";
            _directory = $@"C:\Tests\{Guid.NewGuid()}";
            _filePath1 = Path.Combine(_directory, fileName1);
            _filePath2 = Path.Combine(_directory, fileName2);

            Directory.CreateDirectory(_directory);
            DeleteFile(_filePath1);
            DeleteFile(_filePath2);
        }

        private UnitTestFramework<SourceActor> ArrangeSourceActor()
        {
            //Arrange
            var framework = UnitTestFrameworkSettings
                .Empty
                .CreateFramework<SourceActor>(this, Props.Create(() => new SourceActor(new SourceSettings(_directory, "2"))), 1);
            framework.Supervisor.ExpectMsg<SourceActor.SetUpCompleteMessage>();
            return framework;
        }

        //[Fact]
        //public void SourceActor_FileIsCreatedAndUpdatedMultipleTimesInQuickSuccession_SendsOneUpsertMessageWithTheLatestFileContentsToChild()
        //{
        //    //Arrange
        //    var framework = ArrangeSourceActor();

        //    // Act
        //    File.WriteAllText(_filePath1, Guid.NewGuid().ToString());
        //    File.WriteAllText(_filePath1, Guid.NewGuid().ToString());
        //    File.WriteAllText(_filePath1, Guid.NewGuid().ToString());
        //    File.WriteAllText(_filePath1, Guid.NewGuid().ToString());
        //    var expectedFileContentText = Guid.NewGuid().ToString();
        //    var expectedFileContentBytes = Encoding.ASCII.GetBytes(expectedFileContentText);
        //    File.WriteAllText(_filePath1, expectedFileContentText);

        //    // Assert
        //    var expected = new DestinationActor.UpsertFileMessage(
        //        Path.GetFileName(_filePath1));
        //    framework
        //        .ResolvedTestProbe("destination")
        //        .ExpectMsg<DestinationActor.UpsertFileMessage>()
        //        .Should().BeEquivalentTo(expected);
        //}
        
        //[Fact]
        //public void SourceActor_FileIsCreatedAndDeletedInQuickSuccession_NoMessageIsSentToChild()
        //{
        //    //Arrange
        //    var framework = ArrangeSourceActor();

        //    // Act
        //    File.WriteAllText(_filePath1, Guid.NewGuid().ToString());
        //    File.Delete(_filePath1);
            
        //    // Assert
        //    framework
        //        .ResolvedTestProbe("destination")
        //        .ExpectNoMsg();
        //}

        [Fact]
        public void SourceActor_FileIsCreatedAndUpdatedAndDeletedAndCreatedAndUpdatedInQuickSuccession_SendsOneUpsertMessageToChild()
        {
            //Arrange
            var framework = ArrangeSourceActor();

            // Act
            WriteFile(_filePath1);
            WriteFile(_filePath1);
            DeleteFile(_filePath1);
            WriteFile(_filePath1);
            WriteFile(_filePath1);
            var fileContents = WriteFile(_filePath1);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.UpsertFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.UpsertFileMessage(_filePath1));
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }
        
        [Fact]
        public void SourceActor_FileIsCreatedAndDeletedInQuickSuccession_DoesNotAnyMessagesToChild()
        {
            //Arrange
            var framework = ArrangeSourceActor();

            // Act
            WriteFile(_filePath1);
            DeleteFile(_filePath1);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }

        [Fact]
        public void SourceActor_FileIsCreatedAndUpdatedAndRenamed_SendsAnUpsertMessageThenARenameMessageToChild()
        {
            //Arrange
            var framework = ArrangeSourceActor();

            // Act
            WriteFile(_filePath1);
            var fileContents = WriteFile(_filePath1);
            RenameFile(_filePath1, _filePath2);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.UpsertFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.UpsertFileMessage(_filePath1));
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.RenameFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.RenameFileMessage(
                    _filePath1,
                    _filePath2));
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }
        
        [Fact]
        public void SourceActor_FileIsCreatedAndUpdatedAndDeletedInQuickSuccession_DoesNotSendAnyMessagesToChild()
        {
            //Arrange
            var framework = ArrangeSourceActor();

            // Act
            WriteFile(_filePath1);
            WriteFile(_filePath1);
            WriteFile(_filePath1);
            DeleteFile(_filePath1);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }
        
        [Fact]
        public void SourceActor_FileIsCreated_SendsOneUpsertMessageToChild()
        {
            // Arrange
            var framework = ArrangeSourceActor();

            // Act
            var fileContents = WriteFile(_filePath1);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.UpsertFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.UpsertFileMessage(_filePath1));
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }

        [Fact]
        public void SourceActor_ExistingFileIsDeleted_SendsOneDeleteMessageToChild()
        {
            // Arrange
            WriteFile(_filePath1);
            var framework = ArrangeSourceActor();

            // Act
            DeleteFile(_filePath1);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.DeleteFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.DeleteFileMessage(_filePath1));
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }

        [Fact]
        public void SourceActor_ExistingFileIsUpdated_SendsOneUpsertMessageToChild()
        {
            // Arrange
            WriteFile(_filePath1);
            var framework = ArrangeSourceActor();

            // Act
            var fileContent = WriteFile(_filePath1);

            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.UpsertFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.UpsertFileMessage(
                    _filePath1));
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }

        [Fact]
        public void SourceActor_ExistingFileIsRenamed_SendsOneRenameMessageToChild()
        {
            // Arrange
            WriteFile(_filePath1);
            var framework = ArrangeSourceActor();

            // Act
            RenameFile(_filePath1, _filePath2);
            
            // Assert
            framework
                .ResolvedTestProbe("destination")
                .ExpectMsg<DestinationActor.RenameFileMessage>()
                .Should().BeEquivalentTo(new DestinationActor.RenameFileMessage(
                    _filePath1,
                    _filePath2));
            framework
                .ResolvedTestProbe("destination")
                .ExpectNoMsg();
        }

        private static byte[] WriteFile(string filePath)
        {
            var fileContent = TestHelper.GenerateStringFrom(TestHelper.AlphaNumeric);
            var fileContentBytes = Encoding.ASCII.GetBytes(fileContent);
            File.WriteAllBytes(filePath, fileContentBytes);
            return fileContentBytes;
        }

        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static void RenameFile(string existingFilePath, string newFilePath)
        {
            if (File.Exists(existingFilePath))
            {
                File.Move(existingFilePath, newFilePath);
            }
        }

        public void Dispose()
        {
            DeleteFile(_filePath1);
            DeleteFile(_filePath2);
            Directory.Delete(_directory, true);
            base.Dispose();
        }
    }
}
