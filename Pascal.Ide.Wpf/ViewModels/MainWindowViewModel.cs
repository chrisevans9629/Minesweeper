using System;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;
using Prism.Mvvm;

namespace Pascal.Ide.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        private string _code;
        private string _error;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code,value, CodeChanged);
        }
        PascalLexer lexer = new PascalLexer();
        PascalAst ast = new PascalAst();
        PascalSemanticAnalyzer analyzer = new PascalSemanticAnalyzer();
        private void CodeChanged()
        {
            try
            {
                var tokens = lexer.Tokenize(Code);
                var node = ast.Evaluate(tokens);
                var analize = analyzer.CheckSyntax(node);
            }
            catch (PascalException e)
            {
                Console.WriteLine(e);
                Error = $"{e.Message}\n{e.StackTrace}";
            }
        }

        public string Error
        {
            get => _error;
            set => SetProperty(ref _error,value);
        }

        public MainWindowViewModel()
        {

        }
    }
}
