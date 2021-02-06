using System.Collections.Generic;
using System.Data;
using Dapper;
using DataProtectionDatabaseKeyStorage.Business.Repositories;
using Moq;
using Moq.Dapper;
using Xunit;

namespace DataProtectionDatabaseKeyStorage.UnitTests.Repositories
{
    public class BaseRepositoryTests
    {
        [Fact]
        public void Execute()
        {
            var mockConnection = new Mock<IDbConnection>();

            const int expectedOutput = 10;

            mockConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null))
                .Returns(expectedOutput);

            var repo = new BaseRepository(mockConnection.Object);

            var result = repo.Execute("test sql");

            Assert.Equal(result, expectedOutput);
        }

        [Fact]
        public void Query()
        {
            var mockConnection = new Mock<IDbConnection>();

            var expectedOutput = new List<int> {10};

            mockConnection.SetupDapper(c => c.Query<int>(It.IsAny<string>(), null, null, true, null, null))
                .Returns(expectedOutput);

            var repo = new BaseRepository(mockConnection.Object);

            var result = repo.Query<int>("test sql");

            Assert.Equal(result, expectedOutput);
        }
    }
}