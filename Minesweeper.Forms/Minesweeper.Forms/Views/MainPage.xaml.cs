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

           
        }

        protected override void OnAppearing()
        {
            //if (BindingContext is MainPageViewModel vm)
            //{
            //    var t = vm.Cells;

            //    foreach (var cellViewModel in t)
            //    {
            //        var label = Resources["Label"] as View;
            //        label.BindingContext = cellViewModel;

            //        //label.BackgroundColor = Color.AliceBlue;
            //        //Grid.SetColumn(label, cellViewModel.Column);
            //        //Grid.SetRow(label, cellViewModel.Row);

            //        //label.SetBinding(Label.TextProperty,
            //        //    new Binding(nameof(cellViewModel.ViewText),
            //        //        BindingMode.OneWay
            //        //        ));

            //        MineGrid.Children.Add(label);
            //    }
            //}
            base.OnAppearing();
        }
    }
}