namespace sahla.DTOs.Response
{
    public class GetAllChallengesDto
    {
        public int ChallengeId { get; set; }  

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int PointsReward { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? UserId { get; set; }  

    }
}
