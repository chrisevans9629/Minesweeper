
using System;
using Bridge.Html5;

namespace Minesweeper
{
    public class Game : MinesweeperBase
    {


        Random random = new Random();
        private HTMLElement Result => Document.GetElementById("result");
        public Game()
        {
            GameEnded += (sender, b) =>
            {
                var result = Result;
                if (b)
                {
                    result.TextContent = "Congrats, you won!";
                }
                else
                {
                    result.TextContent = "Uh oh, you hit a bomb!";
                }
            };
            GameSetup();
            SetupHtml();
            Show();
            //Window.RequestAnimationFrame(RenderLoop);
        }

        HTMLCanvasElement Canvas;
        public float GetY(float y) => y + Document.DocumentElement.ScrollTop - (float) Canvas.GetBoundingClientRect().Top;

        public float GetX(float x) =>
            x + Document.DocumentElement.ScrollLeft - (float) Canvas.GetBoundingClientRect().Left;

        private void SetupCanvas(HTMLCanvasElement canvas, HTMLElement score)
        {
            var context =
                canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            context.Canvas.OnMouseMove = mevent =>
            {
                //context.ClearRect(0,0,width,height);
                foreach (var cell in Grid.Cells)
                {
                    if (cell.Hit(GetX(mevent.ClientX), GetY(mevent.ClientY)))
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

            

            context.Canvas.OnMouseDown = mevent =>
            {
                if (mevent.Button == 0)
                {
                    ClickAction(score, context, mevent.ClientX, mevent.ClientY, _flag);
                }
                if (mevent.Button == 2)
                {
                    ClickAction(score, context, mevent.ClientX, mevent.ClientY, true);
                }
            };

            context.Canvas.OnContextMenu = eventt =>
            {
                eventt.PreventDefault();
            };

        }

        private void ClickAction(HTMLElement score, CanvasRenderingContext2D context, float x, float y, bool flag)
        {
            context.ClearRect(0, 0, (int) Width, (int) Height);
            foreach (var item in Grid.Cells)
            {
                if (item.Hit(GetX(x), GetY(y)))
                {
                    ClickOnCell(item, flag);
                    score.InnerHTML = "Score: " + Score;
                }
            }

            Show();
        }


        private bool _flag;
        //HTMLHeadingElement score = new HTMLHeadingElement(HeadingType.H1);

        void GameSetup()
        {
            Setup(new MinesweeperConfig() { Seed = random.Next() });
            Grid.Cells.ForEach(p => new Cell(p, Document.GetElementById<HTMLCanvasElement>(boardId)));
        }
        string boardId = "board";

        private void SetupHtml()
        {



            string gameId = "game";
            var scoreId = "score";

            HTMLCanvasElement canvas = (HTMLCanvasElement) Document.GetElementById(boardId);
            Canvas = canvas;
            var score = Document.GetElementById(scoreId);
            score.InnerHTML = "Score: ";

            SetupCanvas(canvas, score);


            var reset = Document.GetElementById("reset");
            var flag = Document.GetElementById("flag");
            var text = Document.GetElementById("flagtext");
            flag.OnClick = click =>
            {
                _flag = !_flag;
                text.InnerHTML = "Flag: " + _flag;
            };
            reset.OnClick = click =>
            {
                GameSetup();
                Show();
                Result.TextContent = "";
            };
            canvas.Width = (int)Width;
            canvas.Height = (int)Height;

        }


    }
}