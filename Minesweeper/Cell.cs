using System;
using System.Collections.Generic;
using Bridge.Html5;

namespace Minesweeper
{
    public class Theme
    {
        public static string HighlightColor = GetColor(nameof(HighlightColor));
        public static string DefaultFill = GetColor(nameof(DefaultFill));
        public static string CellColor = GetColor(nameof(CellColor));
        public static string BombColor = GetColor(nameof(BombColor));
        public static string BombBackgroundColor = GetColor(nameof(BombBackgroundColor));
        public static string EmptyCellColor = GetColor(nameof(EmptyCellColor));
        public static string FlagColor = GetColor(nameof(FlagColor));
        public static string CellStrokeColor = GetColor(nameof(CellStrokeColor));

        public static string FlagFont = GetColor(nameof(FlagFont),"font");
        public static string ValueFont = GetColor(nameof(ValueFont),"font");
        public static string BombFont = GetColor(nameof(BombFont), "font");

        private static Dictionary<string, string> ClassColor = new Dictionary<string, string>();
        private static string GetColor(string className, string prop = "color")
        {
            if(ClassColor == null)
                ClassColor = new Dictionary<string, string>();
            if(className == null)
                throw new ArgumentNullException(nameof(className));
            if (ClassColor.ContainsKey(className))
                return ClassColor[className];

            var div = new HTMLDivElement();
            div.ClassName = className;
            Document.Body.AppendChild(div);


            var style = Window.GetComputedStyle(div);

            var color = style.GetPropertyValue(prop);

            ClassColor.Add(className, color);

            Document.Body.RemoveChild(div);

            return color;
        }


    }


    public class Cell 
    {
        private readonly BaseCell _cell;
        private readonly HTMLCanvasElement _canvas;

        public void Highlight()
        {
            var context = _canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.FillStyle = Theme.HighlightColor;
            context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);
            context.FillStyle = Theme.DefaultFill;
        }

        public void UnHighLight()
        {
            var context = _canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.FillStyle = Theme.CellColor;
            context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);
            context.FillStyle = Theme.DefaultFill;
        }
        public Cell(BaseCell cell, HTMLCanvasElement canvas)
        {
            _cell = cell;
            _canvas = canvas;
            _cell.VisualChange += (sender, args) => Show();
        }





        public void Show()
        {
            var context = _canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);

            if (_cell.IsHighlighted)
            {
                Highlight();
            }
            else
            {
                UnHighLight();
            }

            // context.ClearRect(X,Y,W,W);
            if (_cell.ShowBomb)
            {
                context.FillStyle = Theme.BombBackgroundColor;
                context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);

                context.FillStyle = Theme.BombColor;
                context.Font = Theme.BombFont;
                context.FillText("X", (int)_cell.X, (int)(_cell.Y + _cell.Width), (int)_cell.Width);
                context.FillStyle = Theme.DefaultFill;
            }
            else if (_cell.ShowValue)
            {
                context.Font = Theme.ValueFont;
                context.FillText(_cell.Value.ToString(), (int)(_cell.X + _cell.Width / 2), (int)(_cell.Y + _cell.Width / 2), (int)_cell.Width);
            }
            else if (_cell.ShowEmpty)
            {
                context.FillStyle = Theme.EmptyCellColor;
                context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);
                context.FillStyle = Theme.DefaultFill;
            }
            if (_cell.ShowFlag)
            {
                context.FillStyle = Theme.FlagColor;
                context.Font = Theme.FlagFont;
                context.FillText("F", (int)_cell.X, (int)(_cell.Y + _cell.Width), (int)_cell.Width);
                context.FillStyle = Theme.DefaultFill;
            }
            else
            {
                context.StrokeStyle = Theme.CellStrokeColor;
                context.StrokeRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);

            }
        }




    }
}