using Bridge.Html5;

namespace Minesweeper
{
    public class Cell 
    {
        private readonly BaseCell _cell;

        public void Highlight()
        {
            var context = Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.FillStyle = HTMLColor.LightGray;
            context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);
            context.FillStyle = HTMLColor.Black;
        }

        public void UnHighLight()
        {
            var context = Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.FillStyle = HTMLColor.White;
            context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);

            context.FillStyle = HTMLColor.Black;
        }
        public Cell(BaseCell cell)
        {
            _cell = cell;
            _cell.VisualChange += (sender, args) => Show();
        }





        public void Show()
        {
            var context = Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);

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
                context.FillStyle = HTMLColor.DarkRed;
                context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);

                context.FillStyle = HTMLColor.Red;
                context.Font = "55px arial";
                context.FillText("X", (int)_cell.X, (int)(_cell.Y + _cell.Width), (int)_cell.Width);
                context.FillStyle = HTMLColor.Black;
            }
            else if (_cell.ShowValue)
            {
                context.Font = "18px arial";
                context.FillText(_cell.Value.ToString(), (int)(_cell.X + _cell.Width / 2), (int)(_cell.Y + _cell.Width / 2), (int)_cell.Width);
            }
            else if (_cell.ShowEmpty)
            {
                context.FillStyle = HTMLColor.Gray;
                context.FillRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);
                context.FillStyle = HTMLColor.Black;
            }
            if (_cell.ShowFlag)
            {
                context.FillStyle = HTMLColor.Red;
                context.Font = "55px arial";
                context.FillText("F", (int)_cell.X, (int)(_cell.Y + _cell.Width), (int)_cell.Width);
                context.FillStyle = HTMLColor.Black;
            }
            else
            {
                context.StrokeRect((int)_cell.X, (int)_cell.Y, (int)_cell.Width, (int)_cell.Width);

            }
        }




    }
}