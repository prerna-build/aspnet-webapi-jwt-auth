namespace ASP.NET_Core.API.Models.DTO
{
    public class RegionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public String Name { get; set; }
        public String? RegionImageUrl { get; set; }
    }
}
