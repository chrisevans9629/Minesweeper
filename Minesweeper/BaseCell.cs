﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minesweeper
{
    public abstract class BaseCell : INotifyPropertyChanged
    {
        private bool _flag;
        private int _value;
        private bool _bomb;
        private bool _visible;
        public abstract void Show();
        public abstract void Highlight();
        public abstract void UnHighLight();
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
        public virtual int Row { get; set; }
        public virtual int Column { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public virtual float Width { get; set; }
        public virtual bool Hit(float x, float y)
        {
            return (x > X && x < X + Width && y > Y && y < Y + Width);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}