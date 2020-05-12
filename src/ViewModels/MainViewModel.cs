using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using Newtonsoft.Json;

using Patagames.Ocr;
using Patagames.Ocr.Enums;

using QAHelper.Models;
using QAHelper.Views;
using QAHelper.WPF;

namespace QAHelper.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private ObservableCollection<QAItem> _qaItems = new ObservableCollection<QAItem>();
        private string _questionSearchWordsString = string.Empty;

        public MainViewModel()
        {
            LoadQAItemsFromJsonFile("Sample.json", false);
            AboutCommand = new DelegateCommand(AboutAction);
            SaveQAItemsIntoJsonCommand = new DelegateCommand(SaveQAItemsIntoJsonAction);
            LoadQAItemsFromJsonCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromJson(false));
            AppendQAItemsFromJsonCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromJson(true));
            LoadQAItemsFromImagesCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromImages(false));
            AppendQAItemsFromImagesCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromImages(true));
        }

        public int QuestionsNumber => QAItems.Count;

        public List<QAItem> QAItemsFiltered => TryCatchWrapperMethod(() =>
        {
            List<QAItem> filteredItems = new List<QAItem>();

            foreach (QAItem item in QAItems)
            {
                IEnumerable<string> searchWords = QuestionSearchWordsString.Split(' ').Where(i => !string.IsNullOrWhiteSpace(i));

                if (!searchWords.Any())
                {
                    filteredItems.Add(item);
                    continue;
                }

                List<string> questionWords = item.Question.Split(' ').ToList();
                while (searchWords.Any() && questionWords.Any())
                {
                    string sw = searchWords.First();
                    int index = questionWords.IndexOf(sw);
                    if (index >= 0)
                    {
                        questionWords = new List<string>(questionWords.Skip(index + 1));
                        searchWords = searchWords.Skip(1);
                    }
                    else
                    {
                        break;
                    }
                }

                if (!searchWords.Any())
                {
                    filteredItems.Add(item);
                }
            }

            return filteredItems;
        }, new List<QAItem>());

        public ObservableCollection<QAItem> QAItems
        {
            get => _qaItems;
            set
            {
                _qaItems = value;
                RaisePropertyChanged(nameof(QAItems));
                RaisePropertyChanged(nameof(QAItemsFiltered));
                RaisePropertyChanged(nameof(QuestionsNumber));
            }
        }

        public string QuestionSearchWordsString
        {
            get => _questionSearchWordsString;
            set
            {
                _questionSearchWordsString = value;
                RaisePropertyChanged(nameof(QuestionSearchWordsString));
                RaisePropertyChanged(nameof(QAItemsFiltered));
            }
        }

        public DelegateCommand AboutCommand { get; }

        public DelegateCommand SaveQAItemsIntoJsonCommand { get; }

        public DelegateCommand LoadQAItemsFromJsonCommand { get; }

        public DelegateCommand AppendQAItemsFromJsonCommand { get; }

        public DelegateCommand LoadQAItemsFromImagesCommand { get; }

        public DelegateCommand AppendQAItemsFromImagesCommand { get; }

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

        private T TryCatchWrapperMethod<T>(Func<T> func, T defaultReturn)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return defaultReturn;
            }
        }

        private void GroupAndAssignQAItems(IEnumerable<QAItem> items, bool append)
        {
            QAItems = new ObservableCollection<QAItem>(items
                .Concat(append ? QAItems : new ObservableCollection<QAItem>())
                .GroupBy(i => i.Question, i => i.Answers)
                .Select(i => new QAItem
                {
                    Question = i.Key,
                    Answers = i.SelectMany(a => a).Distinct().ToList()
                }));
        }

        private void LoadQAItemsFromJsonFile(string filePath, bool append)
        {
            TryCatchWrapperMethod(() =>
            {
                GroupAndAssignQAItems(JsonConvert.DeserializeObject<IEnumerable<QAItem>>(File.ReadAllText(filePath)), append);
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
                var dialog = new SaveFileDialog
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

        private void LoadOrAppendQAItemsFromJson(bool append)
        {
            TryCatchWrapperMethod(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json"
                };

                if (dialog.ShowDialog() ?? false)
                {
                    LoadQAItemsFromJsonFile(dialog.FileName, append);
                }
            });
        }

        private void LoadOrAppendQAItemsFromImages(bool append)
        {
            TryCatchWrapperMethod(() =>
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    dialog.Description = "Please, select folder with questions images.";
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                    string qFolder = dialog.SelectedPath;

                    dialog.Description = "Please, select folder with answers images. They should be named same to questions. Multi answer should starts with name of the question.";
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                    string aFolder = dialog.SelectedPath;

                    var supportedExtensions = new string[] { ".jpg", ".png", ".bmp" };
                    IEnumerable<string> qPaths = Directory
                        .GetFiles(qFolder)
                        .Where(p => supportedExtensions.Contains(Path.GetExtension(p)))
                        .Distinct();
                    IEnumerable<string> aPaths = Directory
                        .GetFiles(aFolder)
                        .Where(p => supportedExtensions.Contains(Path.GetExtension(p)))
                        .Distinct();

                    var qPath_to_qaItem = new Dictionary<string, QAItem>();
                    using (OcrApi translator = OcrApi.Create())
                    {
                        translator.Init(Languages.English);

                        foreach (string qPath in qPaths)
                        {
                            if (!qPath_to_qaItem.TryGetValue(qPath, out QAItem qaItem))
                            {
                                qaItem = new QAItem
                                {
                                    Question = translator.GetTextFromImage(qPath)
                                };
                                qPath_to_qaItem.Add(qPath, qaItem);
                            }

                            string qFileName = Path.GetFileNameWithoutExtension(qPath);

                            foreach (string aPath in aPaths)
                            {
                                string aFileName = Path.GetFileNameWithoutExtension(aPath);
                                if (aFileName.StartsWith(qFileName))
                                {
                                    qaItem.Answers.Add(translator.GetTextFromImage(aPath));
                                }
                            }
                        }
                    }

                    GroupAndAssignQAItems(qPath_to_qaItem.Values, append);
                }
            });
        }
    }
}