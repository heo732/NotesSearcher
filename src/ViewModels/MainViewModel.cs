using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

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
        private readonly SettingsModel _settingsModel = new SettingsModel();

        public MainViewModel()
        {
            LoadQAItemsFromJsonFile("Sample.json", false);
            AboutCommand = new DelegateCommand(AboutAction);
            SaveQAItemsIntoJsonCommand = new DelegateCommand(SaveQAItemsIntoJsonAction);
            LoadQAItemsFromJsonCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromJson(false));
            AppendQAItemsFromJsonCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromJson(true));
            LoadQAItemsFromImagesCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromImages(false));
            AppendQAItemsFromImagesCommand = new DelegateCommand(() => LoadOrAppendQAItemsFromImages(true));
            SettingsCommand = new DelegateCommand(SettingsAction);
        }

        public int QuestionsNumber => QAItemsFiltered.Count;

        private List<string> SearchWordParts
        {
            get
            {
                string searchWordsStr = QuestionSearchWordsString;

                if (string.IsNullOrEmpty(searchWordsStr))
                {
                    return new List<string>();
                }

                foreach (string s in _settingsModel.Punctuation)
                {
                    searchWordsStr = searchWordsStr.Replace(s, " ");
                }
                return searchWordsStr.Split(' ').Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            }
        }

        public List<QuestionViewModel> QAItemsFiltered => TryCatchWrapperMethod(() =>
        {
            List<QuestionViewModel> filteredItems = new List<QuestionViewModel>();

            IList<string> searchWordParts_origin = SearchWordParts;

            if (!searchWordParts_origin.Any())
            {
                return QAItems.Select(i => new QuestionViewModel(new List<Run> { new Run(i.Question) }, i.Answers.Select(a => new AnswerViewModel(new List<Run> { new Run(a) })).ToList())).ToList();
            }

            foreach (QAItem item in QAItems)
            {
                bool passQuestion = false;
                bool passAnyAnswer = false;
                var qestionParts = new List<Run> { new Run(item.Question) };
                List<List<Run>> answersParts = item.Answers.Select(a => new List<Run> { new Run(a) }).ToList();

                // Search in Questions.
                if (_settingsModel.KeyWordsSearchType == Enums.KeyWordsSearchType.Questions || _settingsModel.KeyWordsSearchType == Enums.KeyWordsSearchType.Both)
                {
                    passQuestion = SearchWordPartsInText(item.Question, searchWordParts_origin, out List<Run> qTextParts);
                    qestionParts = qTextParts;
                }

                // Search in Answers.
                if (_settingsModel.KeyWordsSearchType == Enums.KeyWordsSearchType.Answers || (_settingsModel.KeyWordsSearchType == Enums.KeyWordsSearchType.Both && !passQuestion))
                {
                    for (int i = 0; i < item.Answers.Count; i++)
                    {
                        if (SearchWordPartsInText(item.Answers[i], searchWordParts_origin, out List<Run> answerParts))
                        {
                            passAnyAnswer = true;
                            answersParts[i] = answerParts;
                        }
                    }
                }

                bool passed = false;
                switch (_settingsModel.KeyWordsSearchType)
                {
                    case Enums.KeyWordsSearchType.Questions:
                        passed = passQuestion;
                        break;
                    case Enums.KeyWordsSearchType.Answers:
                        passed = passAnyAnswer;
                        break;
                    case Enums.KeyWordsSearchType.Both:
                        passed = passQuestion || passAnyAnswer;
                        break;
                }
                if (passed)
                {
                    filteredItems.Add(new QuestionViewModel(qestionParts, answersParts.Select(a => new AnswerViewModel(a))));
                }
            }

            return filteredItems;
        }, new List<QuestionViewModel>());

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
                RaisePropertyChanged(nameof(QuestionsNumber));
            }
        }

        public DelegateCommand AboutCommand { get; }

        public DelegateCommand SaveQAItemsIntoJsonCommand { get; }

        public DelegateCommand LoadQAItemsFromJsonCommand { get; }

        public DelegateCommand AppendQAItemsFromJsonCommand { get; }

        public DelegateCommand LoadQAItemsFromImagesCommand { get; }

        public DelegateCommand AppendQAItemsFromImagesCommand { get; }

        public DelegateCommand SettingsCommand { get; }

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
                    Answers = i.SelectMany(a => a)
                        .Distinct()
                        .Where(a => !string.IsNullOrWhiteSpace(a))
                        .ToList()
                })
                .Where(i => !string.IsNullOrWhiteSpace(i.Question)));
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
                        switch (_settingsModel.ImageRecognitionLanguage)
                        {
                            case Enums.RecognitionLanguage.EN:
                                translator.Init(Languages.English);
                                break;
                            case Enums.RecognitionLanguage.UA:
                                translator.Init(Languages.Ukrainian);
                                break;
                        }

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

        private void SettingsAction()
        {
            new SettingsWindow(new SettingsViewModel(_settingsModel)).ShowDialog();
            RaisePropertyChanged(nameof(QAItemsFiltered));
        }

        private bool SearchWordPartsInText(string text, IList<string> origin_wordParts_toSearch, out List<Run> highlightableTextParts)
        {
            highlightableTextParts = new List<Run> { new Run(text) };

            foreach (string s in _settingsModel.Punctuation)
            {
                text = text.Replace(s, " ");
            }
            List<string> textWords = text
                .Split(' ')
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            IEnumerable<string> searchParts = origin_wordParts_toSearch;

            while (searchParts.Any() && textWords.Any())
            {
                string sw = searchParts.First();
                int index = textWords.IndexOf(textWords.Where(tw => tw.IndexOf(sw, StringComparison.InvariantCultureIgnoreCase) >= 0).FirstOrDefault());
                if (index >= 0)
                {
                    textWords = new List<string>(textWords.Skip(index + 1));
                    searchParts = searchParts.Skip(1);
                }
                else
                {
                    break;
                }
            }

            if (!searchParts.Any())
            {
                return true;
            }

            return false;
        }
    }
}