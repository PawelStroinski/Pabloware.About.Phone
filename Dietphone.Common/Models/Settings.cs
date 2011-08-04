namespace Dietphone.Models
{
    public class Settings : Entity
    {
        public bool ShowEnergy { get; set; }
        public bool ShowProteinInGrams { get; set; }
        public bool ShowDigestibleCarbsInGrams { get; set; }
        public bool ShowFatInGrams { get; set; }
        public bool ShowCu { get; set; }
        public bool ShowFpu { get; set; }
    }
}
