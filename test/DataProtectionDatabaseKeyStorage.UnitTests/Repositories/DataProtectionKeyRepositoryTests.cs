using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Dapper;
using DataProtectionDatabaseKeyStorage.Business.Repositories;
using Moq;
using Moq.Dapper;
using Xunit;

namespace DataProtectionDatabaseKeyStorage.UnitTests.Repositories
{
    public class DataProtectionKeyRepositoryTests
    {
        public DataProtectionKeyRepositoryTests()
        {
            MockDbConnection = new Mock<IDbConnection>();
        }

        private Mock<IDbConnection> MockDbConnection { get; }

        [Fact]
        public void GetAllElements()
        {
            var expectedOutput = new List<string> {"<RandomXmlTag>testing</RandomXmlTag>", "<RandomXmlTagTwo>testing</RandomXmlTagTwo>"};

            MockDbConnection.SetupDapper(c => c.Query<string>(It.IsAny<string>(), null, null, true, null, null))
                .Returns(expectedOutput);

            var repo = new DataProtectionKeyRepository(MockDbConnection.Object);

            var result = repo.GetAllElements();

            Assert.Equal(2, result.Count);
            Assert.Equal(expectedOutput[0], result.ToList()[0].ToString());
            Assert.Equal(expectedOutput[1], result.ToList()[1].ToString());
        }

        [Fact]
        public void StoreElement()
        {
            var mockRepo = new Mock<DataProtectionKeyRepository>(MockDbConnection.Object);

            const string friendlyName = "";
            var element = XElement.Parse("<testingTag>testing value</testingTag>");
            var paramsSupplied = new
            {
                friendlyName,
                xml = element.ToString(SaveOptions.DisableFormatting)
            };

            mockRepo.Setup(c => c.Execute(It.IsAny<string>(), It.Is<object>(o => o.GetHashCode() == paramsSupplied.GetHashCode())))
                .Returns(12);

            var repo = mockRepo.Object;
            repo.StoreElement(element, friendlyName);

            mockRepo.Verify(c => c.Execute(It.IsAny<string>(), It.Is<object>(o => o.GetHashCode() == paramsSupplied.GetHashCode())), Times.Once);
        }
    }
}