using System;

namespace DrxTransfer.IntegrationServicesClient
{
    [EntityName("Персоны")]
    public class IPersons : ICounterparties
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string INILA { get; set; }
        public string ShortName { get; set; }
        public string Sex { get; set; }
    }
}
