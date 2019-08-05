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
using System.Windows.Input;
using System.Windows.Media;
using Minesweeper.Test;
using Pascal.Ide.Wpf.Models;
using Unity;

namespace Pascal.Ide.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow, IDisposable
    {

        CompositeDisposable disposables = new CompositeDisposable();
        private readonly FlowDocument _doc;
        public MainWindow(IUnityContainer container)
        {
            container.RegisterInstance<IMainWindow>(this);
            InitializeComponent();
            _doc = new FlowDocument();
            RichTextBox.Document = _doc;

            var obs = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(action => RichTextBox.TextChanged += action,
                    action => RichTextBox.TextChanged -= action);

            var one = obs.Subscribe(unit => App.Current.Dispatcher.Invoke(OnCodeChanged) );

            var two = obs
                    //todo: can improve by only looking at the code that has changed
                //.Where(p=>p.EventArgs.)
                .Throttle(TimeSpan.FromSeconds(5), new DispatcherScheduler(Dispatcher))
                
                .SkipWhile(p=> _isBusy)
                .Subscribe(unit => HighlightSyntax());
            disposables.Add(one);
            disposables.Add(two);
        }

        private bool _isBusy = false;
         void HighlightSyntax()
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            TextRange text = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            text.ClearAllProperties();

            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            while (current != null)
            {
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(textInRun))
                {
                    foreach (var keyValuePair in Minesweeper.Test.Pascal.Reservations)
                    {
                        var matches = Regex.Matches(textInRun, keyValuePair.Key, RegexOptions.IgnoreCase);
                        foreach (Match match in matches)
                        {


                            var index = match.Index;
                            if (index != -1)
                            {
                                TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                                TextPointer selectionEnd = selectionStart.GetPositionAtOffset(match.Value.Length, LogicalDirection.Forward);
                                TextRange selection = new TextRange(selectionStart, selectionEnd);
                                if (selection.Text != match.Value)
                                {
                                    continue;
                                    // throw new Exception($"expected '{match.Value}' but was '{selection.Text}'");
                                }
                                //selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                                selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.RoyalBlue));
                            }
                        }
                    }

                    //int index = textInRun.IndexOf(keyword);

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

        public Unit OnCodeChanged()
        {
            HighlightSyntax();
            CodeChanged?.Invoke(this, EventArgs.Empty);
            return Unit.Default;
        }


        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}
