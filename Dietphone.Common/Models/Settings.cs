namespace Dietphone.Models
{
    public class Settings : Entity
    {
        public bool CalculateEnergy { get; set; }
        public bool CalculateProteinInGrams { get; set; }
        public bool CalculateDigestibleCarbsInGrams { get; set; }
        public bool CalculateFatInGrams { get; set; }
        public bool CalculateCu { get; set; }
        public bool CalculateFpu { get; set; }
        public bool FirstRun { get; set; }
    }
}
