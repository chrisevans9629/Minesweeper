using Bridge.Html5;

namespace Minesweeper
{
    public class Cell : BaseCell
    {

        public override void Highlight()
        {
            var context = Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.FillStyle = HTMLColor.LightGray;
            context.FillRect((int)X, (int)Y, (int)Width, (int)Width);
            context.FillStyle = HTMLColor.Black;
        }

        public override void UnHighLight()
        {
            var context = Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.FillStyle = HTMLColor.White;
            context.FillRect((int)X, (int)Y, (int)Width, (int)Width);

            context.FillStyle = HTMLColor.Black;
        }
        public Cell(int row, int column, int w)
        {
            X = row * w;
            Y = column * w;
            Row = row;
            Column = column;
            Width = w;
            //Bomb = true;
            Visible = false;
        }





        public override void Show()
        {
            var context = Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            // context.ClearRect(X,Y,W,W);
            if (ShowBomb)
            {
                context.FillStyle = HTMLColor.DarkRed;
                context.FillRect((int)X, (int)Y, (int)Width, (int)Width);

                context.FillStyle = HTMLColor.Red;
                context.Font = "55px arial";
                context.FillText("X", (int)X, (int)(Y + Width), (int)Width);
                context.FillStyle = HTMLColor.Black;
            }
            else if (ShowValue)
            {
                context.Font = "18px arial";
                context.FillText(Value.ToString(), (int)(X + Width / 2), (int)(Y + Width / 2), (int)Width);
            }
            else if (ShowEmpty)
            {
                context.FillStyle = HTMLColor.Gray;
                context.FillRect((int)X, (int)Y, (int)Width, (int)Width);
                context.FillStyle = HTMLColor.Black;
            }
            if (ShowFlag)
            {
                context.FillStyle = HTMLColor.Red;
                context.Font = "55px arial";
                context.FillText("F", (int)X, (int)(Y + Width), (int)Width);
                context.FillStyle = HTMLColor.Black;
            }
            else
            {
                context.StrokeRect((int)X, (int)Y, (int)Width, (int)Width);

            }
        }




    }
}