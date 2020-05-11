using System.Collections.ObjectModel;

using QAHelper.WPF;

namespace QAHelper
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<QAItem> qaItems = new ObservableCollection<QAItem>();

        public int QuestionsNumber => QAItems.Count;

        public ObservableCollection<QAItem> QAItems
        {
            get => qaItems;
            set
            {
                qaItems = value;
                RaisePropertyChanged(nameof(QAItems));
                RaisePropertyChanged(nameof(QuestionsNumber));
            }
        }
    }
}