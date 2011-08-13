namespace Dietphone.Models
{
    public class Settings : Entity
    {
        public bool ScoreEnergy { get; set; }
        public bool ScoreProtein { get; set; }
        public bool ScoreDigestibleCarbs { get; set; }
        public bool ScoreFat { get; set; }
        public bool ScoreCu { get; set; }
        public bool ScoreFpu { get; set; }
        public bool FirstRun { get; set; }
    }
}
