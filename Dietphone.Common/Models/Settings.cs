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
        private string currentUiCulture;
        private string currentProductCulture;
        private string nextUiCulture;
        private string nextProductCulture;
        private readonly object currentUiCultureLock = new object();
        private readonly object currentProductCultureLock = new object();
        private readonly object nextUiCultureLock = new object();
        private readonly object nextProductCultureLock = new object();

        public string CurrentUiCulture
        {
            get
            {
                lock (currentUiCultureLock)
                {
                    if (string.IsNullOrEmpty(currentUiCulture))
                    {
                        currentUiCulture = NextUiCulture;
                    }
                    return currentUiCulture;
                }
            }
        }

        public string CurrentProductCulture
        {
            get
            {
                lock (currentProductCultureLock)
                {
                    if (string.IsNullOrEmpty(currentProductCulture))
                    {
                        currentProductCulture = NextProductCulture;
                    }
                    return currentProductCulture;
                }
            }
        }

        public string NextUiCulture
        {
            get
            {
                lock (nextUiCultureLock)
                {
                    if (string.IsNullOrEmpty(nextUiCulture))
                    {
                        nextUiCulture = GetDefaultCulture();
                    }
                    return nextUiCulture;
                }
            }
            set
            {
                nextUiCulture = value;
            }
        }

        public string NextProductCulture
        {
            get
            {
                lock (nextProductCultureLock)
                {
                    if (string.IsNullOrEmpty(nextProductCulture))
                    {
                        nextProductCulture = GetDefaultCulture();
                    }
                    return nextProductCulture;
                }
            }
            set
            {
                nextProductCulture = value;
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
