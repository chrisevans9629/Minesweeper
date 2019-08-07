using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Minesweeper.Test;

namespace Pascal.Ide.Wpf.Views
{
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
                    try
                    {
                        var tokens = lexer.Tokenize(textInRun);
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
                                selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(param.Color));
                            }
                        }
                    }
                    catch (PascalException e)
                    {
                        Console.WriteLine(e);
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

        public void Initialize(RichTextBox richTextBox)
        {
            _doc = new FlowDocument();
            richTextBox.Document = _doc;

            var obs = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(action => richTextBox.TextChanged += action,
                    action => richTextBox.TextChanged -= action);
            var two = obs
                .Throttle(TimeSpan.FromSeconds(1), new DispatcherScheduler(App.Current.Dispatcher))
                .SkipWhile(p => _isBusy)
                .Subscribe(unit => OnCodeChanged());
            _disposables.Add(two);
        }
    }
}