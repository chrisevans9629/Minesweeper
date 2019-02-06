using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Minesweeper;

namespace AI.View
{
    public static class Ext
    {
        public static IEnumerable<UIElement> ToList(this UIElementCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                yield return collection[i];
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INeuralNetwork _neuralNetwork;

        public MainWindow(INeuralNetwork neuralNetwork)
        {
            _neuralNetwork = neuralNetwork;
            InitializeComponent();
            this.Loaded += OnLoaded;


        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Update();
        }

        private void Update()
        {
            Canvas.Children.Clear();
            var layers = new List<double[]>();



            var size = _neuralNetwork.HiddenLayers.Length + 2 + _neuralNetwork.Weights.Length;
            bool lastWasWeight = false;
            int weightnum = 0;
            int hiddennum = 0;
            for (int i = 0; i < size; i++)
            {
                if (i == 0)
                {
                    layers.Add(_neuralNetwork.Inputs);
                    continue;
                }
                if (i == size - 1)
                {
                    layers.Add(_neuralNetwork.Outputs);
                    continue;
                }
                if (lastWasWeight)
                {
                    lastWasWeight = false;
                    layers.Add(_neuralNetwork.HiddenLayers[hiddennum]);
                    hiddennum++;
                }
                else
                {
                    lastWasWeight = true;
                    layers.Add(_neuralNetwork.Weights[weightnum]);
                    weightnum++;
                }



            }

            var height = Canvas.ActualHeight;
            var layersize = Canvas.ActualWidth / size;
            var layersarray = layers.ToArray();
            bool wieght = false;
            for (var i = 0; i < layersarray.Length; i++)
            {
                if (wieght)
                {
                    wieght = false;
                }
                else
                {
                    CreateSynapsesLayer(layersarray[i], (int)(i * layersize), 0, (int)layersize, (int)height, i);
                    wieght = true;
                }
            }
            wieght = false;
            for (var i = 0; i < layersarray.Length; i++)
            {
                if (wieght)
                {
                    CreateWieghtLayer(layersarray[i], layersarray[i - 1], layersarray[i + 1], i-1,i+1);
                    wieght = false;
                }
                else
                {
                    wieght = true;
                }
            }

        }

        public void SetPosition(FrameworkElement element, int x, int y, int width, int height)
        {
            element.Margin = new Thickness(x, y, 0, 0);
            element.Width = width;
            element.Height = height;
        }

        public void CreateWieghtLayer(double[] weights, double[] previouslayer, double[] nextlayer, int previousindex, int nextindex)
        {
            var weightindex = 0;
            for (int i = 0; i < previouslayer.Length; i++)
            {
                for (int j = 0; j < nextlayer.Length; j++)
                {
                    var last = Canvas.Children.ToList().Cast<TextBlock>()
                        .FirstOrDefault(p => p.Name == $"Name{previousindex}{i}");
                    var next = Canvas.Children.ToList().Cast<TextBlock>()
                        .FirstOrDefault(p => p.Name == $"Name{nextindex}{j}");
                    
                    var line = new Line
                    {
                        X1 = last.Margin.Left + last.Width,
                        X2 = next.Margin.Left,
                        Y1 = last.Margin.Top + last.Height / 2,
                        Y2 = next.Margin.Top + next.Height / 2,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Red)
                    };
                    var x = line.X2 - line.X1;
                    var y = line.Y2 - line.Y1;
                    //var textblock = new TextBlock
                    //{
                    //    Text = weights[weightindex].ToString("n4")
                    //};
                    //SetPosition(textblock,(int)x,(int)y,100,100);
                    //Canvas.Children.Add(textblock);
                    Canvas.Children.Add(line);


                    weightindex++;
                }
            }
        }
        public void CreateSynapsesLayer(double[] layer, int x, int y, int width, int height, int layernum)
        {
            var synapseHeight = height / layer.Length;
            for (var index = 0; index < layer.Length; index++)
            {
                var d = layer[index];
                var txt = new TextBlock();
                txt.TextAlignment = TextAlignment.Center;
                //txt.Background = new SolidColorBrush(Colors.Beige);
                txt.Text = d.ToString("n4");
                txt.Name = $"Name{layernum}{index}";
                txt.FontSize =  (double)synapseHeight/2;
                if (txt.FontSize > 24)
                {
                    txt.FontSize = 24;
                }
                SetPosition(txt, x, y + synapseHeight * index, width, synapseHeight);

                txt.Padding = new Thickness(0, txt.Height / 2.2, 0,0 );
                Canvas.Children.Add(txt);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            _neuralNetwork.FeedForward(new double[] { random.NextDouble(), random.NextDouble() });
            Update();
        }
    }
}
