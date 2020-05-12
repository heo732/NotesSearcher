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
    }
}