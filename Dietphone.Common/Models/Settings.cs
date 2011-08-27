using System.Threading;
using System.Linq;

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
        public string NextUiCulture { get; set; }
        public string NextProductCulture { get; set; }
        private string lastUiCulture;
        private string lastProductCulture;
        private readonly object uiCultureLock = new object();
        private readonly object productCultureLock = new object();

        public string UiCulture
        {
            get
            {
                lock (uiCultureLock)
                {
                    if (string.IsNullOrEmpty(lastUiCulture))
                    {
                        MakeSureNextUiCultureExists();
                        lastUiCulture = NextUiCulture;
                    }
                    return lastUiCulture;
                }
            }
        }

        public string ProductCulture
        {
            get
            {
                lock (productCultureLock)
                {
                    if (string.IsNullOrEmpty(lastProductCulture))
                    {
                        MakeSureNextProductCultureExists();
                        lastProductCulture = NextProductCulture;
                    }
                    return lastProductCulture;
                }
            }
        }

        private void MakeSureNextUiCultureExists()
        {
            if (string.IsNullOrEmpty(NextUiCulture))
            {
                NextUiCulture = GetDefaultCulture();
            }
        }

        private void MakeSureNextProductCultureExists()
        {
            if (string.IsNullOrEmpty(NextProductCulture))
            {
                NextProductCulture = GetDefaultCulture();
            }
        }

        private string GetDefaultCulture()
        {
            var cultures = new Cultures();
            return cultures.DefaultCulture;
        }
    }

    public class Cultures
    {
        public string[] SupportedCultures
        {
            get
            {
                return new string[] { "en-US", "pl-PL" };
            }
        }

        public string DefaultCulture
        {
            get
            {
                var thread = Thread.CurrentThread;
                var culture = thread.CurrentCulture;
                var systemCulture = culture.Name;
                if (SupportedCultures.Contains(systemCulture))
                {
                    return systemCulture;
                }
                else
                {
                    return SupportedCultures.FirstOrDefault();
                }
            }
        }
    }
}
