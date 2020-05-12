using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

using Newtonsoft.Json;

using QAHelper.Models;
using QAHelper.Views;
using QAHelper.WPF;

namespace QAHelper.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<QAItem> _qaItems = new ObservableCollection<QAItem>();

        public MainViewModel()
        {
            LoadQAItemsFromJsonFile("Sample.json");
            AboutCommand = new DelegateCommand(AboutAction);
            SaveQAItemsIntoJsonCommand = new DelegateCommand(SaveQAItemsIntoJsonAction);
        }

        public int QuestionsNumber => QAItems.Count;

        public ObservableCollection<QAItem> QAItems
        {
            get => _qaItems;
            set
            {
                _qaItems = value;
                RaisePropertyChanged(nameof(QAItems));
                RaisePropertyChanged(nameof(QuestionsNumber));
            }
        }

        public DelegateCommand AboutCommand { get; }

        public DelegateCommand SaveQAItemsIntoJsonCommand { get; }

        private void TryCatchWrapperMethod(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadQAItemsFromJsonFile(string filePath)
        {
            TryCatchWrapperMethod(() =>
            {
                QAItems = new ObservableCollection<QAItem>(JsonConvert.DeserializeObject<IEnumerable<QAItem>>(File.ReadAllText(filePath)));
            });
        }

        private void AboutAction()
        {
            new AboutWindow(new AboutViewModel()).ShowDialog();
        }

        private void SaveQAItemsIntoJsonAction()
        {
            TryCatchWrapperMethod(() =>
            {
                var dialog = new SaveFileDialog()
                {
                    Filter = "JSON files (*.json)|*.json",
                    FileName = "QA_Items"
                };

                if (dialog.ShowDialog() ?? false)
                {
                    File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(QAItems, Formatting.Indented));
                }
            });
        }
    }
}