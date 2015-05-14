namespace StateMachine
{
    public class Token
    {
        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
            Text = "";
        }

        public TokenType TokenType { get; set; }
        public string Text { get; set; }
    }
}