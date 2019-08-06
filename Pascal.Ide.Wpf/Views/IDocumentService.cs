using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Minesweeper.Test;
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