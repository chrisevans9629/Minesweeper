using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Minesweeper.Forms.ViewModels
{
    public class CellViewModel : BaseCell
    {
        private readonly IMinesweeperBase _minesweeper;
        private string _viewText;
        public DelegateCommand<bool?> TapCommand { get; set; }

        public CellViewModel(IMinesweeperBase minesweeper)
        {
            _minesweeper = minesweeper;
            TapCommand = new DelegateCommand<bool?>(Tap);
        }

        public string ViewText
        {
            get => _viewText;
            set
            {
                _viewText = value;
                OnPropertyChanged();
            }
        }

        private string UpdateViewText()
        {
            return this.Flag ? "F" : (this.Visible ? (this.Bomb ? "X" : this.Value.ToString()) : "NA");
        }

        private void Tap(bool? flag)
        {
            if (flag is bool b)
                _minesweeper.ClickOnCell(this, b);
        }

        public override void Show()
        {
            ViewText = UpdateViewText();
        }

        public override void Highlight()
        {
        }

        public override void UnHighLight()
        {
        }
    }

    public class MainPageViewModel : ViewModelBase
    {
        private MinesweeperBase minesweeper;
        private ObservableCollection<CellViewModel> _cells;

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
            minesweeper = new MinesweeperBase();
            var config = new MinesweeperConfig(p => new CellViewModel(minesweeper) { Row = p.Row, Column = p.Column, Width = p.Width })
            {
                Columns = 10,
                Rows = 10,
                BombCount = 20,
                Seed = 100
            };
            minesweeper.Setup(config);
            Cells = new ObservableCollection<CellViewModel>(minesweeper.Cells.Cast<CellViewModel>());
        }

        public int Rows => minesweeper.Rows;

        public int Columns => minesweeper.Columns;
        public ObservableCollection<CellViewModel> Cells
        {
            get => _cells;
            set => SetProperty(ref _cells, value);
        }
    }
}
