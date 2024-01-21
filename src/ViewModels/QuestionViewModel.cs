using System.Collections.Generic;
using System.Windows.Documents;

namespace NotesSearcher.ViewModels;
public class QuestionViewModel : HighlightableTextViewModel
{
    public QuestionViewModel(IList<Run> highlightableTextParts, IEnumerable<AnswerViewModel> answers)
        : base(highlightableTextParts)
    {
        Answers = answers;
    }

    public IEnumerable<AnswerViewModel> Answers { get; }
}
