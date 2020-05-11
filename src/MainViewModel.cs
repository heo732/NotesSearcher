using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

using Newtonsoft.Json;

using QAHelper.WPF;

namespace QAHelper
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<QAItem> qaItems = new ObservableCollection<QAItem>();

        public MainViewModel()
        {
            LoadQAItemsFromJsonFile("Sample.json");
        }

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

        private void LoadQAItemsFromJsonFile(string filePath)
        {
            try
            {
                QAItems = new ObservableCollection<QAItem>(JsonConvert.DeserializeObject<IEnumerable<QAItem>>(File.ReadAllText(filePath)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}