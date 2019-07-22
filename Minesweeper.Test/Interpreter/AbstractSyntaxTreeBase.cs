﻿using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public abstract class AbstractSyntaxTreeBase : IDisposable
    {
        protected Node Expression()
        {
            var result = MultiDiv();
            while (_tokens.Current != null && _tokens.Current.Token.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Token.Name == SimpleTree.Add)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Add);
                    result = new BinaryOperator(result, MultiDiv(), token);
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Sub)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Sub);
                    result = new BinaryOperator(result, MultiDiv(), token);
                }
                else
                {
                    break;
                }
            }

            return result;
        }
        protected virtual NumberLeaf ParseNumber()
        {
            var value = new NumberLeaf(_tokens.Current);
            _tokens.MoveNext();
            return value;
        }

        protected virtual Node ParaAddSub()
        {
            var current = _tokens.Current;
            var par = Parenthese();
            if (par != null) return par;

            if (current.Token.Name == SimpleTree.Add)
            {
                Eat(SimpleTree.Add);
                return new UnaryOperator(ParaAddSub(), current);
            }
            if (current.Token.Name == SimpleTree.Sub)
            {
                Eat(SimpleTree.Sub);
                return new UnaryOperator(ParaAddSub(), current);
            }
            return ParseNumber();
        }

        protected virtual Node Parenthese()
        {
            if (_tokens.Current.Token.Name == SimpleTree.LParinth)
            {
                Eat(SimpleTree.LParinth);
                var result = Expression();
                Eat(SimpleTree.RParinth);
                var para = result;
                return para;
            }

            return null;
        }

        Node MultiDiv()
        {
            var result = ParaAddSub();

            while (_tokens.Current != null && _tokens.Current.Token.Name != SimpleTree.Num)
            {
                if (_tokens.Current.Token.Name == SimpleTree.Multi)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Multi);
                    result = new BinaryOperator(result, ParaAddSub(), token);
                }

                else if (_tokens.Current.Token.Name == SimpleTree.Div)
                {
                    var token = _tokens.Current;
                    Eat(SimpleTree.Div);
                    result = new BinaryOperator(result, ParaAddSub(), token);
                }
                else
                {
                    break;
                }
            }


            return result;
        }
        protected IEnumerator<TokenItem> _tokens;
        public void Dispose()
        {
            _tokens?.Dispose();
        }
        public void Eat(string name)
        {
            if (_tokens.Current?.Token.Name == name)
            {
                _tokens.MoveNext();
            }
            else
            {
                throw new Exception($"expected {name} but was {_tokens.Current?.Token.Name}");
            }
        }

        public abstract Node Evaluate();
    }
}