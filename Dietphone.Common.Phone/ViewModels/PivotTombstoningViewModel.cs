using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class PivotTombstoningViewModel : ViewModelBase
    {
        public StateProvider StateProvider { protected get; set; }
        private int pivot;
        private const string PIVOT = "PIVOT";

        public virtual int Pivot
        {
            get
            {
                return pivot;
            }
            set
            {
                if (pivot != value)
                {
                    pivot = value;
                    OnPropertyChanged("Pivot");
                }
            }
        }

        public virtual void Tombstone()
        {
            TombstonePivot();
        }

        public virtual void Untombstone()
        {
            UntombstonePivot();
        }

        protected void TombstonePivot()
        {
            var state = StateProvider.State;
            state[PIVOT] = Pivot;
        }

        protected void UntombstonePivot()
        {
            var state = StateProvider.State;
            if (state.ContainsKey(PIVOT))
            {
                Pivot = (int)state[PIVOT];
            }
        }
    }
}
