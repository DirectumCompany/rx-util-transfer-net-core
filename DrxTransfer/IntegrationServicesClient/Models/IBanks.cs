namespace DrxTransfer.IntegrationServicesClient
{
    [EntityName("Банк")]
    public  class IBanks : ICounterparties
    {
        
        public string TRRC { get; set; }
        public string IsCardReadOnly { get; set; }
        public string LegalName { get; set; }
        public string BIC { get; set; }
        public string CorrespondentAccount { get; set; }
        public bool IsSystem { get; set; }
    }
}
