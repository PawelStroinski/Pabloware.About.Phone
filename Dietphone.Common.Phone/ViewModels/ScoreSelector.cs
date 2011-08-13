using Dietphone.Models;
using Dietphone.Tools;
using System.Windows;

namespace Dietphone.ViewModels
{
    public enum ScoreKind { First, Second, Third, Fourth }

    public abstract class ScoreSelector : ScoreSelectorBase
    {
        public ScoreSelector(Factories factories)
            : base(factories)
        {
        }

        public string First
        {
            get
            {
                Kind = ScoreKind.First;
                return Result;
            }
        }

        public string Second
        {
            get
            {
                Kind = ScoreKind.Second;
                return Result;
            }
        }

        public string Third
        {
            get
            {
                Kind = ScoreKind.Third;
                return Result;
            }
        }

        public string Fourth
        {
            get
            {
                Kind = ScoreKind.Fourth;
                return Result;
            }
        }

        public bool FirstExists
        {
            get
            {
                Kind = ScoreKind.First;
                return Exists;
            }
        }

        public bool SecondExists
        {
            get
            {
                Kind = ScoreKind.Second;
                return Exists;
            }
        }

        public bool ThirdExists
        {
            get
            {
                Kind = ScoreKind.Third;
                return Exists;
            }
        }

        public bool FourthExists
        {
            get
            {
                Kind = ScoreKind.Fourth;
                return Exists;
            }
        }

        public Visibility FirstVisibility
        {
            get
            {
                return FirstExists.ToVisibility();
            }
        }

        public Visibility SecondVisibility
        {
            get
            {
                return SecondExists.ToVisibility();
            }
        }

        public Visibility ThirdVisibility
        {
            get
            {
                return ThirdExists.ToVisibility();
            }
        }

        public Visibility FourthVisibility
        {
            get
            {
                return FourthExists.ToVisibility();
            }
        }
    }

    public abstract class ScoreSelectorBase
    {
        public ScoreKind Kind { private get; set; }
        protected Settings settingsCopy;
        private ScoreKind currentKind;
        private bool lastForwardHadScore;
        private readonly Factories factories;

        public ScoreSelectorBase(Factories factories)
        {
            this.factories = factories;
        }

        public string Result
        {
            get
            {
                Initialize();
                while (currentKind != Kind)
                {
                    Forward();
                }
                return GetCurrent();
            }
        }

        public bool Exists
        {
            get
            {
                Initialize();
                while (currentKind != Kind)
                {
                    Forward();
                }
                Forward();
                return lastForwardHadScore;
            }
        }

        protected abstract string GetCurrent();

        private void Forward()
        {
            IncreaseCurrentKind();
            lastForwardHadScore = true;
            if (settingsCopy.ScoreEnergy)
            {
                settingsCopy.ScoreEnergy = false;
                return;
            }
            if (settingsCopy.ScoreProtein)
            {
                settingsCopy.ScoreProtein = false;
                return;
            }
            if (settingsCopy.ScoreDigestibleCarbs)
            {
                settingsCopy.ScoreDigestibleCarbs = false;
                return;
            }
            if (settingsCopy.ScoreFat)
            {
                settingsCopy.ScoreFat = false;
                return;
            }
            if (settingsCopy.ScoreCu)
            {
                settingsCopy.ScoreCu = false;
                return;
            }
            if (settingsCopy.ScoreFpu)
            {
                settingsCopy.ScoreFpu = false;
                return;
            }
            lastForwardHadScore = false;
        }

        private void IncreaseCurrentKind()
        {
            var temp = (byte)currentKind;
            temp++;
            currentKind = (ScoreKind)temp;
        }

        private void Initialize()
        {
            var settings = factories.Settings;
            settingsCopy = settings.GetCopy();
            currentKind = ScoreKind.First;
        }
    }
}
