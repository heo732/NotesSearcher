using System.Collections.Generic;
using System.Windows.Documents;

namespace QAHelper.ViewModels
{
    public abstract class HighlightableTextViewModel
    {
        public HighlightableTextViewModel(IList<Run> highlightableTextParts)
        {
            HighlightableTextParts = highlightableTextParts;
        }

        public IList<Run> HighlightableTextParts { get; }
    }
}