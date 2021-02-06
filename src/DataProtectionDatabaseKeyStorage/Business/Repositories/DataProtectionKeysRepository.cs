using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using DataProtectionDatabaseKeyStorage.Data.Interfaces;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace DataProtectionDatabaseKeyStorage.Business.Repositories
{
    public class DataProtectionKeyRepository : BaseRepository, IDataProtectionKeyRepository, IXmlRepository
    {
        private const string DataProtectionKeyTable = "DataProtectionKeys";

        public DataProtectionKeyRepository(IDbConnection connection) : base(connection)
        {
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();

            IEnumerable<XElement> GetAllElementsCore()
            {
                //Queries the database for the xml fields
                var xmlList = Query<string>($@"SELECT Xml From {DataProtectionKeyTable}");

                //Loops over the xml fields and cast them into their XElement
                foreach (var xml in xmlList)
                    if (!string.IsNullOrEmpty(xml))
                        yield return XElement.Parse(xml);
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            //sql server query version
            //$@"INSERT INTO {DataProtectionKeyTable} (FriendlyName, Xml)
            //VALUES (@friendlyName, @xml)"

            Execute($@"
                INSERT INTO {DataProtectionKeyTable} (Id, FriendlyName, Xml)
                VALUES (0, @friendlyName, @xml)",
                new
                {
                    friendlyName,
                    xml = element.ToString(SaveOptions.DisableFormatting)
                });
        }
    }
}