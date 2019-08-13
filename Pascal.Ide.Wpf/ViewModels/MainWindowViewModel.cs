using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using Akavache;
using Microsoft.Win32;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;
using Pascal.Ide.Wpf.Models;
using Pascal.Ide.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace Pascal.Ide.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private List<HighlightParameters> parameters;
        private readonly IDocumentService _mainWindow;
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

        private const string CompilerInterpreter = "Interpreter";
        private string CompilerCSharp = "CSharp";
        private string Compiler68000 = "68000 Assembler";
        private string CompilerPascal = "Pascal";
        public MainWindowViewModel(IDocumentService mainWindow)
        {
            Compilers = new ObservableCollection<string>()
            {
                CompilerInterpreter,
                CompilerCSharp,
                Compiler68000,
                CompilerPascal
            };

            SelectedCompiler = CompilerInterpreter;

            _mainWindow = mainWindow;
            parameters = new List<HighlightParameters>
            {
                new HighlightParameters()
                {
                    Color = Color.RoyalBlue,
                    Filter = item => Minesweeper.Test.PascalTerms.Reservations.ContainsKey(item.Token.Name)
                },
                new HighlightParameters()
                {
                    Color = Color.Yellow,
                    Filter = item => item.Token.Name == PascalTerms.IntegerConst || item.Token.Name == PascalTerms.RealConst
                },
                new HighlightParameters()
                {
                    Color = Color.SaddleBrown,
                    Filter = item => item.Token.Name == PascalTerms.StringConst
                }
            };

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
            set => SetProperty(ref _output, value);
        }

        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value, InputChanged);
        }

        private void InputChanged()
        {
            BlobCache.LocalMachine.InsertObject(InputKey, Input);
        }

        public Node AbstractSyntaxTree
        {
            get => _abstractSyntaxTree;
            set => SetProperty(ref _abstractSyntaxTree, value);
        }

        public DelegateCommand OpenCommand { get; }

        public ObservableCollection<string> Compilers
        {
            get => _compilers;
            set => SetProperty(ref _compilers,value);
        }

        public string SelectedCompiler
        {
            get => _selectedCompiler;
            set => SetProperty(ref _selectedCompiler,value);
        }

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
                if (SelectedCompiler == CompilerInterpreter)
                {
                    var interpreter = new PascalInterpreter(console: console);
                    interpreter.Interpret(AbstractSyntaxTree);
                    Output = console.Output;
                }

                if (SelectedCompiler == CompilerCSharp)
                {
                    var csharp = new PascalToCSharp();
                    var result = csharp.VisitNode(AbstractSyntaxTree);
                    Output = result;
                }
            }
            catch (PascalException e)
            {
                Console.WriteLine(e);
                Error = $"{e.Message}\n{e.StackTrace}";
            }
        }

        private IList<TokenItem> Tokens;
        private ObservableCollection<string> _compilers;
        private string _selectedCompiler;
        public const string CodeKey = "Code";

        private void CodeChanged()
        {
            try
            {
                BlobCache.LocalMachine.InsertObject(CodeKey, Code);
                Tokens = lexer.Tokenize(Code);
                AbstractSyntaxTree = ast.Evaluate(Tokens);
                analyzer.CheckSyntax(AbstractSyntaxTree);
                Error = "";
            }
            catch (PascalException e)
            {
                Console.WriteLine(e);
                Error = $"{e.Message}\n{e.StackTrace}";
            }
            finally
            {
                if (Tokens != null)
                {
                    _mainWindow.HighlightSyntax(parameters);
                }
            }
        }
    }
}