using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minesweeper
{
    public class BaseCell : INotifyPropertyChanged
    {
        private bool _flag;
        private int _value;
        private bool _bomb;
        private bool _visible;

        public virtual void Show()
        {
           
        }

        public bool ShowBomb => Visible && Bomb;
        public bool ShowValue => Visible && Value > 0;
        public bool ShowEmpty => Visible && Value <= 0 && !Bomb;

        public bool ShowFlag => !Visible && Flag;
        public virtual void Highlight()
        {
            IsHighlighted = true;
        }

        public virtual void UnHighLight()
        {
            IsHighlighted = false;
        }
        public virtual bool Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                OnPropertyChanged();
                Show();
            }
        }

        public virtual int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
                Show();
            }
        }


        public virtual bool Bomb
        {
            get { return _bomb; }
            set
            {
                _bomb = value;
                OnPropertyChanged();
                Show();
            }
        }

        
        public virtual bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                OnPropertyChanged();
                Show();
            }
        }

        public virtual bool IsHighlighted { get; set; }
        public virtual int Row { get; set; }
        public virtual int Column { get; set; }

        public virtual float XOffset { get; set; }
        public virtual float YOffset { get; set; }

        public virtual float X => Column * Width + XOffset;
        public virtual float Y => Row * Width + YOffset;
        public virtual float Width { get; set; }
        public virtual bool Hit(float x, float y)
        {
            return (x > X && x < X + Width && y > Y && y < Y + Width);
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public string DisplayValue()
        {
            if (ShowBomb)
            {
                return "X";
            }
            if (ShowEmpty)
            {
                return "0";
            }
            if (ShowFlag)
            {
                return "F";
            }
            if (ShowValue)
            {
                return Value.ToString();
            }
            return " ";
        }

        public override string ToString()
        {
            return $"({Column},{Row}): {(ShowBomb ? "x" : ShowEmpty ? "_" : ShowFlag ? "F" : ShowValue ? Value.ToString() : "")},Value:{Value},Bomb:{Bomb}";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}