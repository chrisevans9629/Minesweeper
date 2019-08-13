using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Minesweeper.Test;
using Pascal.Ide.Wpf.ViewModels;
using Unity;

namespace Pascal.Ide.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;
        public MainWindow(IUnityContainer container)
        {
            var ser = container.Resolve<IDocumentService>();
            InitializeComponent();
            ser.Initialize(RichTextBox);
            var txtBox = FastColoredTextBox;
            if (DataContext is MainWindowViewModel vm)
            {
                
                _vm = vm;
                txtBox.Text = vm.Code;
            }




        }
        private void Tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var inlines = this.Tx1.Inlines;
            inlines.Clear();

            foreach (char ch in this.Tb1.Text)
            {
                if (Char.IsDigit(ch))
                {
                    var run = new Run(ch.ToString());
                    run.Foreground = Brushes.Blue;
                    inlines.Add(run);
                }
                else if (Char.IsLetter(ch))
                {
                    var run = new Run(ch.ToString());
                    run.Foreground = Brushes.Red;
                    inlines.Add(run);
                }
                else
                {
                    var run = new Run(ch.ToString());
                    run.Foreground = Brushes.LimeGreen;
                    inlines.Add(run);
                }
            }
        }

        private void FastColoredTextBox_OnTextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            _vm.Code = FastColoredTextBox.Text;
        }
    }
}
