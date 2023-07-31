namespace rub.Lex
{
	using Save = Double;
	using Size = Int64;

    public class Lexer
	{
        private readonly string[] _line;
		private Size _position = 0;
		private readonly DiagnosticBag _errors;
        private readonly Size _lineNumber;

        public Lexer(LineHolder line)
		{
            _lineNumber = line.LineNumber;
            _line = Functions.SeparateStr(line.Line).ToArray();
			_errors = new();
			_errors.SetLine(_lineNumber);
			_errors.SetErrorSufix(line.FilePath);
        }

		private string Peek(Size offset)
		{
			var index = _position + offset;
			if (index >= _line.Length)
				return "\0";
			return _line[index];
		}

		private string Current => Peek(0);
		
		public Token[] Lex()
        {
			List<Token> tokens = new();

            for (int i = 0; i < _line.Length; i++)
            {
                var token = GetToken();
				_position++;

				if (token.Kind == TokenKind.EOF)
					break;

				tokens.Add(token);
			}

			_errors.Print();

			return tokens.ToArray();
        }

        private Token GetToken()
        {
			if (Current == "\0")
                return new Token(TokenKind.EOF, "\0");

            var kind = GetKind(Current);

			if (kind == TokenKind.BADTOKEN)
				_errors.BadInput(Current);

			return new Token(kind, Current);
        }

        private static TokenKind GetKind(string str)
        {
			if (string.IsNullOrEmpty(str))
				return TokenKind.EOF;

            return str[0] switch
            {
                '$' => TokenKind.REGISTER,
                '@' => TokenKind.TAG,
                '-' or
				'0' or
				'1' or
				'2' or
				'3' or
				'4' or
				'5' or
				'6' or
				'7' or
				'8' or
				'9' => TokenKind.NUMBER,
                '.' => TokenKind.DIRECTIVE,
                _   => Compiler.opcodes.ContainsKey(str) ? TokenKind.OPCODE : TokenKind.BADTOKEN,
            };
        }
    }
}