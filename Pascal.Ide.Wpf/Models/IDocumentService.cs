using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Minesweeper.Test;
using Pascal.Ide.Wpf.Models;

namespace Pascal.Ide.Wpf.Views
{
    public interface IDocumentService
    {
        string Code { get; set; }
        event EventHandler CodeChanged;
        void Initialize(RichTextBox rtb);

        void HighlightSyntax(IList<HighlightParameters> parameters);
    }

    public class HighlightParameters
    {
        public Func<TokenItem, bool> Filter { get; set; }

        public Color Color { get; set; }
    }
}