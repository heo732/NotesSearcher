using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using QAHelper.Models;
using QAHelper.WPF;

namespace QAHelper.ViewModels
{
    public class QAItemViewModel : BindableBase
    {
        private string _question;
        private ObservableCollection<string> _answers = new ObservableCollection<string>();

        public QAItemViewModel(QAItem model, string multiAnswerDelimeter)
        {
            Question = model.Question;
            Answers = new ObservableCollection<string>(Regex.Split(model.Answer, multiAnswerDelimeter + "{1}"));
        }

        public string Question
        {
            get => _question;
            private set
            {
                _question = value;
                RaisePropertyChanged(nameof(Question));
            }
        }

        public ObservableCollection<string> Answers
        {
            get => _answers;
            private set
            {
                _answers = value;
                RaisePropertyChanged(nameof(_answers));
            }
        }
    }
}