namespace rub.Lex
{
    public class Token
	{
		public Token(TokenKind kind, string text)
		{
            Kind = kind;
            Text = text;
        }

        public TokenKind Kind { get; }
        public string Text { get; }
    }
}