using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using Newtonsoft.Json;

using QAHelper.Models;
using QAHelper.WPF;

namespace QAHelper.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<QAItemViewModel> _qaItems = new ObservableCollection<QAItemViewModel>();
        private string _multiAnswerDelimeter = "===";

        public MainViewModel()
        {
            LoadQAItemsFromJsonFile("Sample.json");
            ChangeMultiAnswerDelimeterCommand = new DelegateCommand(ChangeMultiAnswerDelimeterAction);
        }

        public int QuestionsNumber => QAItems.Count;

        public ObservableCollection<QAItemViewModel> QAItems
        {
            get => _qaItems;
            set
            {
                _qaItems = value;
                RaisePropertyChanged(nameof(QAItems));
                RaisePropertyChanged(nameof(QuestionsNumber));
            }
        }

        public DelegateCommand ChangeMultiAnswerDelimeterCommand { get; }

        private void LoadQAItemsFromJsonFile(string filePath)
        {
            try
            {
                QAItems = new ObservableCollection<QAItemViewModel>(JsonConvert.DeserializeObject<IEnumerable<QAItem>>(File.ReadAllText(filePath)).Select(i => new QAItemViewModel(i, _multiAnswerDelimeter)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeMultiAnswerDelimeterAction()
        {

        }
    }
}