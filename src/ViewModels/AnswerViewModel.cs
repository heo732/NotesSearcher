using System.Collections.Generic;
using System.Windows.Documents;

namespace NotesSearcher.ViewModels;
public class AnswerViewModel : HighlightableTextViewModel
{
    public AnswerViewModel(IList<Run> highlightableTextParts)
        : base(highlightableTextParts)
    { }
}
