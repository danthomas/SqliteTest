using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
    public class Parser : StateMachine<Parser>
    {
        private string _text;
        private int _index;
        private char _curr;
        private char _prev;
        private char _next;
        
        public Parser()
        {
            //Rules
            var isDelimiter = new Rule(p => p.Curr == '|');
            var stateIsText = new Rule(p => p.TokenType == TokenType.Text);
            var stateIsStatement = new Rule(p => p.TokenType == TokenType.Statement);
            var isLegalStatementChar = new Rule(p => Char.IsLetter(p.Curr));
            var isNotLegalStatementChar = new Rule(p => !Char.IsLetter(p.Curr));

            //Actions
            var appendChar = new Action<Parser>(p => p.CurrToken.Text += p.Curr);
            var newText = new Action<Parser>(p => p.NewToken(TokenType.Text));
            var newStatement = new Action<Parser>(p => p.NewToken(TokenType.Statement));


            When(stateIsText).And(isDelimiter).Then(newStatement);
            When(stateIsStatement).And(isDelimiter).Then(newStatement);

            When(stateIsStatement).And(isLegalStatementChar).Then(appendChar);
            When(stateIsStatement).And(isNotLegalStatementChar).Then(newText).And(appendChar);

            When(stateIsStatement).Then(appendChar);
            When(stateIsText).Then(appendChar);
        }


        public char Curr { get { return _curr; } }
        public char Prev { get { return _prev; } }
        public TokenType TokenType { get { return Tokens.Last().TokenType; } }
        public Token CurrToken { get { return Tokens.Last(); } }

        public List<Token> Tokens { get; set; }

        public override bool Next()
        {
            if (_index < _text.Length)
            {
                _prev = _curr;
                _curr = _text[_index];
                _index++;
                _next = _index < _text.Length ? _text[_index] : (char)0;
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
        }

        internal void Initialise(string text)
        {
            _text = text;
            _index = 0;
            _prev = _curr = _next = (char)0;
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