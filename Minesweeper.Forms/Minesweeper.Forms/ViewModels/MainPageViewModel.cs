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
    public class MainPageViewModel : ViewModelBase
    {
        private MinesweeperBase minesweeper;
        private ObservableCollection<BaseCell> _cells;

        

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
            minesweeper = new MinesweeperBase();
            var config = new MinesweeperConfig(p => new NoShowCell(p.Row, p.Column, p.Width))
            {
                Columns = 100,
                Rows = 100,
                BombCount = 20,
                Seed = 100
            };
            minesweeper.Setup(config);
            Cells = new ObservableCollection<BaseCell>(minesweeper.Cells);
        }

        public ObservableCollection<BaseCell> Cells
        {
            get => _cells;
            set => SetProperty(ref _cells, value);
        }
    }
}
