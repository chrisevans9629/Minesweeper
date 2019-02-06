using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NeuralNetwork = Minesweeper.NeuralNetwork;

namespace AI.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var main = new MainWindow(new NeuralNetwork(new Random(100), 4, new []{8,16,8},4, 0.3));
            main.Show();
        }
    }
}
