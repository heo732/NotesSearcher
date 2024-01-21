using System.Collections.Generic;

namespace NotesSearcher.Models;
public class QAItem
{
    public string Question { get; set; } = string.Empty;

    public List<string> Answers { get; set; } = new List<string>();
}
