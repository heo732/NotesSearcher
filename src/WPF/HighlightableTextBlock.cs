using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace QAHelper.WPF
{
    public class HighlightableTextBlock : SelectableTextBlock
    {
        public static readonly DependencyProperty HighlightableTextPartsProperty = DependencyProperty.Register(
            "HighlightableTextParts",
            typeof(IList<Run>),
            typeof(HighlightableTextBlock),
            new FrameworkPropertyMetadata { PropertyChangedCallback = OnChanged });

        public IList<Run> HighlightableTextParts
        {
            get => (IList<Run>)GetValue(HighlightableTextPartsProperty);
            set
            {
                SetValue(HighlightableTextPartsProperty, value);
            }   
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightableTextBlock textBlock)
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.AddRange(textBlock.HighlightableTextParts);
            }   
        }
    }
}