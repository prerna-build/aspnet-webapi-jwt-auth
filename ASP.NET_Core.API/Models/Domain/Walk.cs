namespace ASP.NET_Core.API.Models.Domain
{
    public class Walk
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public String Description { get; set; }
        public double LengthInKm { get; set; }
        public String? WalkImageUrl { get; set; }
        public Guid DifficultyId { get; set; }
        public Guid RegionId { get; set; }

        //navigation property
        public Difficulty Difficulty { get; set; }     //relation between walk and difficulty
        public Region Region { get; set; }    // relation between walk and region
    }
}
