using System;
using System.IO;
using Akavache;
using Microsoft.Win32;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;
using Pascal.Ide.Wpf.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Pascal.Ide.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IMainWindow _mainWindow;
        PascalLexer lexer = new PascalLexer();
        PascalAst ast = new PascalAst();
        PascalSemanticAnalyzer analyzer = new PascalSemanticAnalyzer();
        private string _title = "Pascal Studio";
        private string _error;
        private string _output;
        private string _input;
        private Node _abstractSyntaxTree;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Code
        {
            get => _mainWindow.Code;
            set
            {
                _mainWindow.Code = value;
                RaisePropertyChanged();
            }
        }

        public string Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public DelegateCommand StartCommand { get; }


        public MainWindowViewModel(IMainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            StartCommand = new DelegateCommand(Start);
            OpenCommand = new DelegateCommand(Open);
            _mainWindow.CodeChanged += (sender, args) => CodeChanged();
            BlobCache.LocalMachine.GetOrCreateObject(CodeKey, () => "").Subscribe(s => Code = s);
            BlobCache.LocalMachine.GetOrCreateObject(InputKey, () => "").Subscribe(s => Input = s);
        }

        private const string InputKey = "Input";
        private void Open()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var file = dialog.FileName;
                Code = File.ReadAllText(file);
            }
        }

        public string Output
        {
            get => _output;
            set => SetProperty(ref _output,value);
        }

        public string Input
        {
            get => _input;
            set => SetProperty(ref _input,value, InputChanged);
        }

        private void InputChanged()
        {
            BlobCache.LocalMachine.InsertObject(InputKey, Input);
        }

        public Node AbstractSyntaxTree
        {
            get => _abstractSyntaxTree;
            set => SetProperty(ref _abstractSyntaxTree,value);
        }

        public DelegateCommand OpenCommand { get; }

        private void Start()
        {
            try
            {
                var console = new ConsoleModel();
                if (Input != null)
                {
                    console.Input = new Iterator<char>(Input.ToCharArray());
                }

                analyzer.CheckSyntax(AbstractSyntaxTree);
                var interpreter = new PascalInterpreter(console: console);
                interpreter.Interpret(AbstractSyntaxTree);
                Output = console.Output;
            }
            catch (PascalException e)
            {
                Console.WriteLine(e);
                Error = $"{e.Message}\n{e.StackTrace}";
            }

        }

        public const string CodeKey = "Code";
        private void CodeChanged()
        {
            try
            {
                BlobCache.LocalMachine.InsertObject(CodeKey, Code);
                var tokens = lexer.Tokenize(Code);
                AbstractSyntaxTree = ast.Evaluate(tokens);
                var analize = analyzer.CheckSyntax(AbstractSyntaxTree);
                Error = "";
            }
            catch (PascalException e)
            {
                Console.WriteLine(e);
                Error = $"{e.Message}\n{e.StackTrace}";
            }
        }

        
       
    }
}
