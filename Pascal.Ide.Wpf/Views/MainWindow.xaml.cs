using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Minesweeper.Test;
using Pascal.Ide.Wpf.Models;
using Unity;

namespace Pascal.Ide.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private readonly FlowDocument _doc;
        public MainWindow(IUnityContainer container)
        {
            container.RegisterInstance<IMainWindow>(this);
            InitializeComponent();
            _doc = new FlowDocument();
            RichTextBox.Document = _doc;
            RichTextBox.KeyDown += DocumentOnTextInput;

        }

        private void DocumentOnTextInput(object sender, KeyEventArgs e)
        {
            SetCode(GetCode());
        }

        public string Code { get => GetCode(); set => SetCode(value); }
        public event EventHandler CodeChanged;
        private void SetCode(string value)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _doc.Blocks.Clear();

                var iterator = new Iterator<char>(value.ToCharArray());


                var words = new List<string>();
                while (iterator.Current != default(char))
                {
                    if (char.IsWhiteSpace(iterator.Current))
                    {
                        words.Add(iterator.Current.ToString());
                        iterator.Advance();
                    }
                    else
                    {
                        var str = "";
                        while (char.IsWhiteSpace(iterator.Current) != true)
                        {
                            str += iterator.Current;
                            iterator.Advance();
                        }
                        words.Add(str);
                    }
                }

                var inlines = new List<Inline>();
                foreach (var word in words)
                {
                    if (Minesweeper.Test.Pascal.Reservations.ContainsKey(word.ToUpper()))
                    {
                        inlines.Add(new Bold(new Run(word)));
                    }
                    else
                    {
                        inlines.Add(new Run(word));
                    }
                }

                var p = new Paragraph();
                foreach (var inline in inlines)
                {
                    p.Inlines.Add(inline);
                }
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
    }
}
