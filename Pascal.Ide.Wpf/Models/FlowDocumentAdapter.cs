using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Pascal.Ide.Wpf.Models
{
    public interface IMainWindow
    {
        IFlowDocument Document { get; }
    }
    public class TextAdapter
    {
        public string Text { get; set; }
        public string Color { get; set; }
    }
    public interface IParagraph
    {
        IEnumerable<TextAdapter> Lines { get;set;}
    }

    public interface IFlowDocument
    {
        IParagraph Paragraph { get; }
        double FontSize { get; set; }
        event EventHandler TextChanged;
    }
    public class ParagraphAdapter : IParagraph
    {
        private readonly Paragraph _paragraph;

        public ParagraphAdapter(Paragraph paragraph)
        {
            _paragraph = paragraph;
            
        }

        public IEnumerable<TextAdapter> Lines { get=> GetLines(); set=> SetLines(value); }

        private void SetLines(IEnumerable<TextAdapter> value)
        {
            _paragraph.Inlines.Clear();
            foreach (var textAdapter in value)
            {
                _paragraph.Inlines.Add(new TextBlock()
                {
                    Text = textAdapter.Text,
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(textAdapter.Color))
                });
                
            }
        }

        private IEnumerable<TextAdapter> GetLines()
        {
            return _paragraph.Inlines.OfType<TextBlock>().Select(p => new TextAdapter(){Color = p.Foreground.ToString(), Text = p.Text});
        }
    }

    public class FlowDocumentAdapter : IFlowDocument
    {
        private readonly FlowDocument _flowDocument;

        public FlowDocumentAdapter(FlowDocument flowDocument)
        {
            _flowDocument = flowDocument;
            var range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            range.Changed += RangeOnChanged;
        }

        private void RangeOnChanged(object sender, EventArgs e)
        {
            OnTextChanged();
        }

        public IParagraph Paragraph => new ParagraphAdapter(_flowDocument.Blocks.FirstBlock as Paragraph);

        public double FontSize
        {
            get => _flowDocument.FontSize;
            set => _flowDocument.FontSize = value;
        }

        public event EventHandler TextChanged;

        protected virtual void OnTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}