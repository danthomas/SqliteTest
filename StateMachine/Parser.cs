using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
    public class Parser : StateMachine<Parser>
    {
        private string _text;
        private int _index;
        private char _currChar;
        private char _prevChar;
        private char _nextChar;
        
        public Parser()
        {
            //Rules
            var currIsDelimiter = new Rule(p => p.CurrChar == '|');
            var currIsLParen = new Rule(p => p.CurrChar == '(');
            var currIsRParen = new Rule(p => p.CurrChar == ')');
            var currIsLCurly = new Rule(p => p.CurrChar == '{');
            var currIsRCurly = new Rule(p => p.CurrChar == '}');
            var currIsDot = new Rule(p => p.CurrChar == '.');
            var currIsLetter = new Rule(p => Char.IsLetter(p.CurrChar));
            var currIsNotLetter = new Rule(p => !Char.IsLetter(p.CurrChar));

            var nextIsLCurly = new Rule(p => p.NextCharChar == '{');
            var nextIsRCurly = new Rule(p => p.NextCharChar == '}');


            var stateIsText = new Rule(p => p.TokenType == TokenType.Text);
            var stateIsStatement = new Rule(p => p.TokenType == TokenType.Statement);
            var stateIsBlock = new Rule(p => p.TokenType == TokenType.Block);


            var parenCountIsOne = new Rule(p => p.ParenCount == 1);
            var curlyCountIsOne = new Rule(p => p.CurlyCount == 1);
            var parenCountIsNonZero = new Rule(p => p.ParenCount != 0);
            var curlyCountIsNonZero = new Rule(p => p.CurlyCount != 0);

            //Actions
            var appendChar = new Action<Parser>(p => p.CurrToken.Text += p.CurrChar);
            var newText = new Action<Parser>(p => p.NewToken(TokenType.Text));
            var newStatement = new Action<Parser>(p => p.NewToken(TokenType.Statement));
            var newBlock = new Action<Parser>(p => p.NewToken(TokenType.Block));
            var incrementParenCount = new Action<Parser>(p => p.ParenCount++);
            var decrementParenCount = new Action<Parser>(p => p.ParenCount--);
            var incrementCurlyCount = new Action<Parser>(p => p.CurlyCount++);
            var decrementCurlyCount = new Action<Parser>(p => p.CurlyCount--);
            var resetParenCount = new Action<Parser>(p => p.ParenCount = 0);
            var resetCurlyCount = new Action<Parser>(p => p.CurlyCount = 0);


            When(stateIsText).And(currIsDelimiter).And(nextIsLCurly).Then(newBlock).And(resetCurlyCount);
            When(stateIsText).And(currIsDelimiter).Then(newStatement).And(resetParenCount);
            When(stateIsText).Then(appendChar);


            When(stateIsStatement).And(currIsRParen).And(parenCountIsOne).Then(appendChar).And(newText);
            When(stateIsStatement).And(currIsLParen).Then(appendChar).And(incrementParenCount);
            When(stateIsStatement).And(currIsRParen).And(parenCountIsNonZero).Then(appendChar).And(decrementParenCount);
            When(stateIsStatement).And(currIsDot).Then(appendChar);
            When(stateIsStatement).And(currIsLetter).Then(appendChar);
            When(stateIsStatement).And(parenCountIsNonZero).Then(appendChar);
            When(stateIsStatement).And(currIsDelimiter).Then(newStatement).And(resetParenCount);
            When(stateIsStatement).And(currIsNotLetter).Then(newText).And(appendChar);

            When(stateIsBlock).And(currIsRCurly).And(curlyCountIsOne).Then(appendChar).And(newText);
            When(stateIsStatement).And(currIsLCurly).Then(appendChar).And(incrementCurlyCount);
            When(stateIsBlock).And(currIsRCurly).And(curlyCountIsNonZero).Then(appendChar).And(decrementCurlyCount);
            When(stateIsBlock).And(curlyCountIsNonZero).Then(appendChar);
            When(stateIsBlock).And(currIsDelimiter).Then(newStatement).And(resetParenCount);
            When(stateIsBlock).Then(appendChar);
            
        }

        public int CurlyCount { get; set; }

        public int ParenCount { get; set; }
        public char CurrChar { get { return _currChar; } }
        public char PrevChar { get { return _prevChar; } }
        public char NextCharChar { get { return _nextChar; } }
        public TokenType TokenType { get { return Tokens.Last().TokenType; } }
        public Token CurrToken { get { return Tokens.Last(); } }
        public List<Token> Tokens { get; set; }

        public override bool Next()
        {
            if (_index < _text.Length)
            {
                _prevChar = _currChar;
                _currChar = _text[_index];
                _index++;
                _nextChar = _index < _text.Length ? _text[_index] : (char)0;
                return true;
            }

            return false;
        }

        public void NewToken(TokenType tokenType)
        {
            Token token = new Token(tokenType);
            Tokens.Add(token);
        }

        public void Parse(string text)
        {
            Initialise(text);
            Run(this);
            Tokens = Tokens.Where(x => x.Text != "").ToList();
        }

        internal void Initialise(string text)
        {
            _text = text;
            _index = 0;
            _prevChar = _currChar = _nextChar = (char)0;
            Tokens = new List<Token> { new Token(TokenType.Text) };
        }

        internal void Step()
        {
            if (Next())
            {
                Whens.ForEach(x => x.Eval(this));

                When_ when = Whens.FirstOrDefault(x => x.IsTrue);

                if (when != null)
                {
                    when.Actions.ForEach(x => x(this));
                }
            }
        }
    }
}