namespace sahla.DTOs.Request
{
    public class UpdateChallengeDto
    {
        public int ChallengeId { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? PointsReward { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? UserId { get; set; } 
    }
}
