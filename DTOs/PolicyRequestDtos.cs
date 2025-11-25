namespace ULIP_proj.DTOs
{
    public class PolicyDetailsRequest
    {
        public int PolicyId { get; set; }
    }

    public class AcceptProposalRequest
    {
        public int PolicyId { get; set; }
        public bool Accepted { get; set; }
        public bool? RequireDocuments { get; set; } = false;
    }

    public class SurrenderRequest
    {
        public int PolicyId { get; set; }
        public string Reason { get; set; }
        public int? ManagerId { get; set; }
    }
}