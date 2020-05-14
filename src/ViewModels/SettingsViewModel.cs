using System.Linq;

using QAHelper.Enums;
using QAHelper.Models;
using QAHelper.WPF;

namespace QAHelper.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly SettingsModel _model;

        public SettingsViewModel(SettingsModel model)
        {
            _model = model;
        }

        public RecognitionLanguage SelectedRecognitionLanguage
        {
            get => _model.ImageRecognitionLanguage;
            set
            {
                _model.ImageRecognitionLanguage = value;
                RaisePropertyChanged(nameof(SelectedRecognitionLanguage));
            }
        }

        public string Punctuation
        {
            get => string.Join(" ", _model.Punctuation);
            set
            {
                _model.Punctuation = value
                    .Split(' ')
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct()
                    .SelectMany(s => s.ToCharArray())
                    .ToArray();

                RaisePropertyChanged(nameof(Punctuation));
            }
        }

        public KeyWordsSearchType KeyWordsSearchType
        {
            get => _model.KeyWordsSearchType;
            set
            {
                _model.KeyWordsSearchType = value;
                RaisePropertyChanged(nameof(KeyWordsSearchType));
            }
        }
    }
}