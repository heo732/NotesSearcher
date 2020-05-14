using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace QAHelper.WPF
{
    public class HighlightableTextBlock : SelectableTextBlock
    {
        public static readonly DependencyProperty HighlightableTextProperty = DependencyProperty.Register(
            "HighlightableText",
            typeof(string),
            typeof(HighlightableTextBlock),
            new FrameworkPropertyMetadata("", OnChanged));

        public static readonly DependencyProperty HighlightedWordsProperty = DependencyProperty.Register(
            "HighlightedWords",
            typeof(string),
            typeof(HighlightableTextBlock),
            new FrameworkPropertyMetadata("", OnChanged));

        public string HighlightableText
        {
            get => (string)GetValue(HighlightableTextProperty);
            set
            {
                SetValue(HighlightableTextProperty, value);
            }   
        }

        public string HighlightedWords
        {
            get => (string)GetValue(HighlightedWordsProperty);
            set
            {
                SetValue(HighlightedWordsProperty, value);
            }
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightableTextBlock textBlock)
            {
                SetTextBlockTextAndHighlightTerm(textBlock, textBlock.HighlightableText, textBlock.HighlightedWords);
            }   
        }

        private static void SetTextBlockTextAndHighlightTerm(TextBlock textBlock, string text, string termToBeHighlighted)
        {
            textBlock.Text = string.Empty;

            if (TextIsEmpty(text))
            {
                return;
            }   

            if (TextIsNotContainingTermToBeHighlighted(text, termToBeHighlighted))
            {
                AddPartToTextBlock(textBlock, text);
                return;
            }

            var textParts = SplitTextIntoTermAndNotTermParts(text, termToBeHighlighted);

            foreach (var textPart in textParts)
            {
                AddPartToTextBlockAndHighlightIfNecessary(textBlock, termToBeHighlighted, textPart);
            }   
        }

        private static bool TextIsEmpty(string text)
        {
            return text.Length == 0;
        }

        private static bool TextIsNotContainingTermToBeHighlighted(string text, string termToBeHighlighted)
        {
            return text.IndexOf(termToBeHighlighted, StringComparison.Ordinal) < 0;
        }

        private static void AddPartToTextBlockAndHighlightIfNecessary(TextBlock textBlock, string termToBeHighlighted, string textPart)
        {
            if (textPart == termToBeHighlighted)
            {
                AddHighlightedPartToTextBlock(textBlock, textPart);
            }   
            else
            {
                AddPartToTextBlock(textBlock, textPart);
            }   
        }

        private static void AddPartToTextBlock(TextBlock textBlock, string part)
        {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.Normal });
        }

        private static void AddHighlightedPartToTextBlock(TextBlock textBlock, string part)
        {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.Bold });
        }

        private static List<string> SplitTextIntoTermAndNotTermParts(string text, string term)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new List<string>() { string.Empty };
            }

            return Regex.Split(text, $@"({Regex.Escape(term)})")
                        .Where(p => p != string.Empty)
                        .ToList();
        }
    }
}