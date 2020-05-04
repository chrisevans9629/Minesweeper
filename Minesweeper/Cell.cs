using Bridge.Html5;

namespace Minesweeper
{
    public class Theme
    {
        public static string HighlightColor = HTMLColor.LightGray;
        public static string DefaultFill = HTMLColor.Black;
        public static string CellColor = HTMLColor.White;
        public static string BombColor = HTMLColor.Red;
        public static string BombBackgroundColor = HTMLColor.DarkRed;
        public static string EmptyCellColor = HTMLColor.Gray;
        public static string FlagColor = HTMLColor.Red;

        public static string FlagFont = "55px arial";
        public static string ValueFont = "18px arial";
        public static string BombFont = "55px arial";
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
                context.StrokeRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);

            }
        }




    }
}