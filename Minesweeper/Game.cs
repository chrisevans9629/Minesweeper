
using System;
using Bridge.Html5;

namespace Minesweeper
{
    public class Game : MinesweeperBase
    {
        //internal double lastRender = 0;
        //void RenderLoop(double timestamp)
        //{
        //    var progess = timestamp - lastRender;
        //    Update(progess);
        //    Draw();
        //    lastRender = timestamp;
        //    Window.RequestAnimationFrame(RenderLoop);
        //}
        //void Update(double time)
        //{
            
        //}
        //void Draw()
        //{
        //    //context.ClearRect(0,0, width, height);
        //   // Show();
        //}
        private CanvasRenderingContext2D context =
            Program.Canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
        Random random = new Random();

        public Game()
        {
            GameEnded += (sender, b) =>
            {
                if (b)
                {
                    Window.Alert("Win!");
                }
                else
                {
                    Window.Alert("Lost!");
                }
            };
            Setup(p => new Cell(p.Row,p.Column,(int)p.Width),numOfBombs:50,seed:random.Next(int.MaxValue));
            SetupHtml();
            Show();
            //Window.RequestAnimationFrame(RenderLoop);
            context.Canvas.OnMouseMove = mevent =>
            {
                //context.ClearRect(0,0,width,height);
                foreach (var cell in Grid.Cells)
                {
                    if (cell.Hit(mevent.ClientX, mevent.ClientY + Document.DocumentElement.ScrollTop))
                    {
                        cell.Highlight();
                    }
                    else
                    {
                        cell.UnHighLight();
                    }
                }
                Show();
            };
            context.Canvas.OnClick = mevent =>
            {
                context.ClearRect(0, 0,(int) Width, (int)Height);
                foreach (var item in Grid.Cells)
                {
                    if (item.Hit(mevent.ClientX, mevent.ClientY + Document.DocumentElement.ScrollTop))
                    {
                        ClickOnCell(item, _flag);
                        score.InnerHTML = "Score: " + Score;
                    }
                    // item.Show();
                }
                Show();
            };
        }

       

        private bool _flag;
        HTMLHeadingElement score = new HTMLHeadingElement(HeadingType.H1);

        private void SetupHtml()
        {
            Document.Body.AppendChild(new HTMLDivElement() {Id = "game"});
            var game = Document.GetElementById("game");
            var canvas = Program.Canvas;
            var tools = new HTMLDivElement();
            score.InnerHTML = "Score: ";
            var reset = new HTMLButtonElement();
            reset.Style.Width = "100px";
            reset.Style.Height = "100px";
            var flag = new HTMLButtonElement();
            flag.Style.Width = "100px";
            flag.Style.Height = "100px";
            var text = new HTMLParagraphElement();
            text.InnerHTML = "Flag: " + _flag;
            flag.InnerHTML = "Flag";
            flag.OnClick = click =>
            {
                _flag = !_flag;
                text.InnerHTML = "Flag: " + _flag;
            };
            reset.InnerHTML = "Reset";
            reset.OnClick = click =>
            {
                //Reset();
                Setup(p => new Cell(p.Row,p.Column,(int)p.Width),numOfBombs:50,seed:random.Next(int.MaxValue) );
                Show();
               // Setup(p => new Cell(p.Row,p.Column,p.Width)); 
               // Show();
            };
            tools.AppendChild(flag);
            tools.AppendChild(reset);
            tools.AppendChild(text);
            tools.AppendChild(score);
            canvas.Width = (int) Width;
            canvas.Height = (int)Height;
            game.AppendChild(canvas);
            game.AppendChild(tools);
            //Document.Body.AppendChild(canvas);
            // Document.Body.AppendChild(tools);

        }

        
    }
}