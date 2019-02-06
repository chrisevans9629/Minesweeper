namespace Minesweeper
{
    public abstract class BaseCell
    {
        private bool _flag;
        private int _value;
        private bool _bomb;
        private bool _visible;
        public abstract void Show();
        public abstract void Highlight();
        public abstract void UnHighLight();
        public bool Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                Show();
            }
        }

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Show();
            }
        }


        public bool Bomb
        {
            get { return _bomb; }
            set
            {
                _bomb = value;
                Show();
            }
        }

        
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Show();
            }
        }
        public int Row { get; set; }
        public int Column { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public bool Hit(int x, int y)
        {
            return (x > X && x < X + Width && y > Y && y < Y + Width);
        }
    }
}