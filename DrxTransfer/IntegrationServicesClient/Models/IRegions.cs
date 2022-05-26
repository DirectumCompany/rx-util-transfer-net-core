namespace DrxTransfer.IntegrationServicesClient
{
    [EntityName("Регионы")]
    public class IRegions
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
