using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Forms.ViewModels;
using Xamarin.Forms;

namespace Minesweeper.Forms.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //if (BindingContext is MainPageViewModel vm)
            //{
            //    var t = vm.Cells;

            //    foreach (var cellViewModel in t)
            //    {
            //        var label = new Label();

            //        Grid.SetColumn(label, cellViewModel.Column);
            //        Grid.SetRow(label, cellViewModel.Row);

            //        label.SetBinding(Label.TextProperty, new Binding(nameof(cellViewModel.ViewText), BindingMode.OneWay, null, null,null, source:cellViewModel));

            //        MineGrid.Children.Add(label);
            //    }
            //}
        }
    }
}