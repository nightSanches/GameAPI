using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models.Score
{
    public class SubmitScoreRequest
    {
        [Range(0, int.MaxValue)]
        public int Score { get; set; }
    }
}
