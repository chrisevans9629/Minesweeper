using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using FastColoredTextBoxNS;
using Minesweeper.Test;
using Pascal.Ide.Wpf.Views;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Style = FastColoredTextBoxNS.Style;
using TextChangedEventArgs = System.Windows.Controls.TextChangedEventArgs;

namespace Pascal.Ide.Wpf.Models
{
    public class FastCodeDocumentService : IDocumentService
    {
        private FastColoredTextBox fastColoredTextBox;
        public string Code
        {
            get => fastColoredTextBox?.Text;
            set => CodeHasChanged(value);
        }

        private void CodeHasChanged(string value)
        {
            if (fastColoredTextBox != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    fastColoredTextBox.Text = value;

                });

            }
        }

        public event EventHandler CodeChanged;
        public void Initialize(object rtb)
        {
            fastColoredTextBox = rtb as FastColoredTextBox;
            fastColoredTextBox.BackColor = Color.FromArgb(61, 61, 61);
            fastColoredTextBox.ForeColor = Color.White;
            
            fastColoredTextBox.Font = new Font("Times New Roman", 18);
            fastColoredTextBox.TextChangedDelayed += FastColoredTextBoxOnTextChangedDelayed;
        }

        private void FastColoredTextBoxOnTextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            
            OnCodeChanged();
        }

         public class HighlightStyles
        {
            public HighlightParameters HighlightParameters { get; set; }
            public TextStyle TextStyle { get; set; }
        }

         IList<HighlightStyles> HighlightStyleList { get; set; }
        public void HighlightSyntax(IList<HighlightParameters> parameters)
        {
            try
            {
                
                var lexer = new PascalLexer();
                var tokens = lexer.Tokenize(Code);
                if (HighlightStyleList == null)
                {
                    HighlightStyleList = parameters.Select(p => new HighlightStyles()
                    {
                        HighlightParameters = p,
                        TextStyle = new TextStyle(new SolidBrush(p.Color), new SolidBrush(Color.Transparent), FontStyle.Regular)
                    }).ToList();
                }
                fastColoredTextBox.VisibleRange.ClearStyle(HighlightStyleList.Select(p=>p.TextStyle as Style).ToArray());

                foreach (var tokenItem in tokens)
                {
                    foreach (var highlightParameterse in HighlightStyleList)
                    {
                        if (highlightParameterse.HighlightParameters.Filter(tokenItem))
                        {
                            var range = fastColoredTextBox.GetRange(tokenItem.Index, tokenItem.Index + tokenItem.Value.Length);
                            range.SetStyle(highlightParameterse.TextStyle);
                            
                        }
                    }
                }
            }
            catch (PascalException e)
            {
                Console.WriteLine(e);
            }
        }



        protected virtual void OnCodeChanged()
        {
            CodeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DocumentService : IDocumentService, IDisposable
    {

        private bool _isBusy = false;
        public void HighlightSyntax(IList<HighlightParameters> parameters)
        {

            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            TextRange text = new TextRange(_doc.ContentStart, _doc.ContentEnd);
            text.ClearAllProperties();
            var lexer = new PascalLexer();

            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            while (current != null)
            {
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(textInRun))
                {
                    var tokens = new List<TokenItem>();
                    try
                    {
                        tokens = lexer.Tokenize(textInRun).ToList();
                    }
                    catch (PascalException e)
                    {
                        Console.WriteLine(e);
                    }

                    foreach (var match in tokens)
                    {
                        var param = parameters.FirstOrDefault(p => p.Filter(match));
                        if (param == null)
                        {
                            continue;
                        }
                        var index = match.Index;
                        if (index != -1)
                        {
                            TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                            TextPointer selectionEnd = selectionStart.GetPositionAtOffset(match.Value.Length, LogicalDirection.Forward);
                            TextRange selection = new TextRange(selectionStart, selectionEnd);
                            if (selection.Text != match.Value)
                            {
                                continue;
                            }
                           // selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(param.Color));
                        }
                    }



                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }

            _isBusy = false;
        }

        public string Code
        {
            get => GetCode();
            set => SetCode(value);
        }
        public event EventHandler CodeChanged;
        private void SetCode(string value)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _doc.Blocks.Clear();
                var p = new Paragraph(new Run(value));
                p.Margin = new Thickness(0);
                _doc.Blocks.Add(p);
                OnCodeChanged();
            });

        }


        private string GetCode()
        {
            var run = new TextRange(_doc.ContentStart, _doc.ContentEnd);
            return run.Text;
        }

        private void OnCodeChanged()
        {
            CodeChanged?.Invoke(this, EventArgs.Empty);
        }
        readonly CompositeDisposable _disposables = new CompositeDisposable();


        public void Dispose()
        {
            _disposables?.Dispose();
        }
        private FlowDocument _doc;

        public void Initialize(object richTextBox)
        {
            var rtb = richTextBox as RichTextBox;
            _doc = new FlowDocument();
            rtb.Document = _doc;

            var obs = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(action => rtb.TextChanged += action,
                    action => rtb.TextChanged -= action);
            var two = obs
                .Throttle(TimeSpan.FromSeconds(1), new DispatcherScheduler(App.Current.Dispatcher))
                .SkipWhile(p => _isBusy)
                .Subscribe(unit => OnCodeChanged());
            _disposables.Add(two);
        }
    }
}