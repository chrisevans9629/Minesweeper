using System;
using System.Windows.Controls;
using Pascal.Ide.Wpf.Models;

namespace Pascal.Ide.Wpf.Views
{
    public interface IDocumentService
    {
        string Code { get; set; }
        event EventHandler CodeChanged;
        void Initialize(RichTextBox rtb);

        void HighlightSyntax();
    }
}