namespace sahla.DTOs.Request
{
    public class CreateChallengeDto
    {
        public int Id { get; set; } 
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int PointsReward { get; set; }

        public bool IsCompleted { get; set; } = false;

    }
}
