using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class SuperBasicMathInterpreter : IDisposable
    {
        private readonly string _data;
        private readonly IEnumerator<Token> _tokens;
        public SuperBasicMathInterpreter(string data)
        {
            this._data = data;
            _tokens = GetTokens();
        }

        class Token
        {
            public Token()
            {
                Value = "";
            }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        IEnumerator<Token> GetTokens()
        {
            var list = new List<Token>();
            int i = 0;
            while (_data.Length - 1 >= i && _data[i] != default(char))
            {
                if (_data[i] == ' ') i++;
                else if (TokenNumber(ref i, list))
                {

                }
                else if (_data[i] == '+')
                {
                    list.Add(new Token() { Name = SimpleTree.Add });
                    i++;
                }
                else if (_data[i] == '-')
                {
                    list.Add(new Token() { Name = SimpleTree.Sub });
                    i++;
                }
                else if (_data[i] == '*')
                {
                    list.Add(new Token() { Name = SimpleTree.Multi });
                    i++;
                }
                else if (_data[i] == '/')
                {
                    list.Add(new Token() { Name = SimpleTree.Div });
                    i++;
                }
                else if (_data[i] == '(')
                {
                    list.Add(new Token(){Name = SimpleTree.LParinth});
                    i++;
                }
                else if (_data[i] == ')')
                {
                    list.Add(new Token(){Name = SimpleTree.RParinth});
                    i++;
                }
                else
                {
                    throw new Exception($"Char {_data[i]} not expected");
                }

            }


            return list.GetEnumerator();
        }

        private bool TokenNumber(ref int en, IList<Token> list)
        {
            var token = new Token();
            while (_data[en] != default(char) && int.TryParse(_data[en].ToString(), out var t))
            {
                token.Name = SimpleTree.Num;

                token.Value += _data[en].ToString();
                en++;

                if (_data.Length == en) break;
            }


            if (token.Name != null)
            {
                list.Add(token);
                return true;
            }

            return false;
        }

        void Eat(string name)
        {
            if (_tokens.Current?.Name == name)
            {
                _tokens.MoveNext();
            }
            else
            {
                throw new Exception($"expected {name} but was {_tokens.Current?.Name}");
            }
        }

        double ParseNumber()
        {
            var value = double.Parse(_tokens.Current.Value);
            _tokens.MoveNext();
            return value;
        }

        double Para()
        {
            var result = 0.0;
            if (_tokens.Current.Name == SimpleTree.LParinth)
            {
                Eat(SimpleTree.LParinth);
                result = Expression();
                Eat(SimpleTree.RParinth);
                return result;
            }
            return ParseNumber();
        }
        double MultiDiv()
        {
            var result = Para();

            while (_tokens.Current != null && _tokens.Current.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Name == SimpleTree.Multi)
                {
                    Eat(SimpleTree.Multi);
                    result *= Para();
                }

                else if (_tokens.Current.Name == SimpleTree.Div)
                {
                    Eat(SimpleTree.Div);
                    result /= Para();
                }
                else
                {
                    break;
                }
            }


            return result;
        }

        public double Evaluate()
        {
            Eat(null);
            var result = Expression();
            return result;
        }

        private double Expression()
        {
            var result = MultiDiv();
            while (_tokens.Current != null && _tokens.Current.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Name == SimpleTree.Add)
                {
                    Eat(SimpleTree.Add);
                    result += MultiDiv();
                }

                else if (_tokens.Current.Name == SimpleTree.Sub)
                {
                    Eat(SimpleTree.Sub);
                    result -= MultiDiv();
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public void Dispose()
        {
            _tokens?.Dispose();
        }
    }
}