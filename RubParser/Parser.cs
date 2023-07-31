using rub.Instructions;
using rub.Lex;

namespace rub.RubParser
{
    using Save = Int64;
    using Size = Int64;

    public class Parser
	{
        private static Size _mainIndex = -1;

        private readonly LineHolder[] _lines;
        private LineHolder? _line;
        private readonly DiagnosticBag _errors;
		private List<Token> _tokens = new();
        private readonly List<Inst> _instructions = new();

        private bool _condition = false;
		private string _opcode = "";
		private OpKind _kind = OpKind.NULL;

		private string _rd = "";
		private string _rs = "";
		private string _rt = "";

		private string _imm = "";

		private string _tag = "";
		
		private int _regLimit = 0;
		private int _registerCounter = 0;

		private Size _instTracker = 0;

        public Parser(LineHolder[] lines)
		{
            _lines = lines;
            _errors = new();
            ExecuteMainEvents();
        }

        private void ExecuteMainEvents()
        {
            for (_instTracker = 0; _instTracker < _lines.LongLength; _instTracker++)
            {
                _line = _lines[_instTracker];
                Lexer lexer = new(_line);
                var tokens = lexer.Lex().ToList();

                for (var j = 0; j < tokens.Count; j++)
                {
                    var token = tokens[j];
                    if (token.Kind == TokenKind.TAG)
                    {
                        ReadTag(token);
                    }

                    if (token.Text == "@main:")
                    {
                        _mainIndex = _instTracker+1;
                        break;
                    }
                }
            }

            if (_mainIndex == -1)
            {
                _errors.TagNotFound("@main");
            }

            _errors.Print();
        }

        public void Parse()
		{
            for (_instTracker = _mainIndex; _instTracker < _lines.LongLength; _instTracker++)
            {
                var line = _lines[_instTracker];
                if (string.IsNullOrEmpty(line.Line))
                    continue;

                _errors.SetLine(line.LineNumber);
                _errors.SetErrorSufix("rub " + line.FilePath);
                Execute(line);
            }

			_errors.Print();
        }

        private void Execute(LineHolder line)
        {
            Lexer lexer = new(line);
            _tokens = lexer.Lex().ToList();

            foreach (var token in _tokens)
			{
                if (token.Kind == TokenKind.EOF)
                {
                    break;
                }

                switch (token.Kind)
                {
                    case TokenKind.OPCODE:   ReadOpcode(token);   break;
                    case TokenKind.REGISTER: ReadRegister(token); break;
                    case TokenKind.TAG:      ReadTag(token);      break;
                    case TokenKind.NUMBER:   ReadNumber(token);   break;
                    case TokenKind.BADTOKEN:
					default:
						throw new Exception("" + token.Kind);
                }
            }

            switch (_kind)
            {
                case OpKind.REGISTER:  TypeR(); break;
                case OpKind.IMMEDIATE: TypeI(); break;
                case OpKind.JUMP:      TypeJ(); break;
                case OpKind.OTHER:     TypeO(); break;
                case OpKind.VARIANTS:  TypeV(); break;
                case OpKind.TAG:                break;
                case OpKind.BADOP:              break;
                default:
                    throw new Exception($"Invalid type {_kind}");
            }

            ResetValues();
        }
        
        private void TypeR()
        {
            RInst inst = new(_opcode, _rd, _rs, _rt, _line!, _instTracker);
            inst.Execute();

            _errors.AddRange(inst.Errors);
        }

        private void TypeI()
        {
            IInst inst = new(_opcode, _rd, _rs, _imm, _line!, _instTracker);
            inst.Execute();

            _errors.AddRange(inst.Errors);
        }

        private void TypeJ()
        {
            _condition = true;
            
            JInst inst = new(_opcode, _rd, _rs, _imm, _condition, _tag, _line!, _instTracker);
            inst.Execute();
            
            _errors.AddRange(inst.Errors);
            _condition = inst.Condition;
            _instTracker = inst.InstTracker;
        }

        private void TypeO()
        {
            var _lastOpcode = _lines[_instTracker-1].Line.Split(' ')[0];
            OInst inst = new(_opcode, _rd, _tag, _lastOpcode, _condition, _line!, _instTracker);
            inst.Execute();
            
            _errors.AddRange(inst.Errors);
            _instTracker = inst.InstTracker;
        }

        private void TypeV()
        {
            var lineNumber = _line!.LineNumber;
            var filePath = _line!.FilePath;

            VInst inst = new(_opcode, _rd, _rs, _imm, _line!, _instTracker);
            inst.Execute();

            _errors.AddRange(inst.Errors);
            
            var line = inst.Text;

            ResetValues();
            Execute(new LineHolder(lineNumber, line, filePath));
        }

        private void ResetValues()
        {
			_opcode = "";
			_kind = OpKind.NULL;

			_rd = "";
			_rs = "";
			_rt = "";

			_imm = "";

			_tag = "";
			
			_regLimit = 0;
			_registerCounter = 0;
        }

        private void ReadNumber(Token token)
        {
			_imm = token.Text;
        }

        private void ReadTag(Token token)
        {
			var tag = token.Text;

			if (tag[^1] == ':')
			{
                if (!string.IsNullOrEmpty(_opcode))
                {
                    _errors.MayBeTagCall(tag);
                    return;
                }

                tag = tag[..^1];

                if (Compiler.tags.ContainsKey(tag))
                {
                    _errors.TagAlreadyExists(tag);
                    return;
                }

                Compiler.tags.Add(tag, _instTracker);
                _opcode = "";
                _kind = OpKind.TAG;
            }
			else if (_kind == OpKind.JUMP || _opcode == "else")
			{
                if (Compiler.tags.ContainsKey(tag))
                {
                    _tag = tag;
                }
                else
                {
                    _errors.TagNotFound(tag);
                }
            }
        }

        private void ReadRegister(Token token)
        {
			var reg = token.Text[^1] == ',' ? token.Text[..^1] : token.Text;

            if (!Compiler.registers.ContainsKey(reg))
			{
				_errors.RegisterNotExists(reg);
			}

			if (string.IsNullOrEmpty(_rd))
			{
				_rd = reg;
				_registerCounter++;
			}
			else if (string.IsNullOrEmpty(_rs))
			{
				_rs = reg;
				_registerCounter++;
			}
			else if (string.IsNullOrEmpty(_rt))
			{
				_rt = reg;
				_registerCounter++;
			}
			
			if (_registerCounter > _regLimit)
			{
				_errors.LimitRegistersExceded(_regLimit, _opcode, reg);
			}
			
        }

        private void ReadOpcode(Token token)
        {
            if (!string.IsNullOrEmpty(_opcode))
            {
                _errors.DuplicatedOpcode(_opcode, token.Text);
                _kind = OpKind.BADOP;
                return;
            }

            if (!Compiler.opcodes.ContainsKey(token.Text))
            {
                _errors.UnrecognizedOpcode(_opcode);
                _kind = OpKind.BADOP;
                return;
            }

            _opcode = token.Text;
			_kind = Compiler.opcodes[_opcode];
			_regLimit = GetRegLimit(_kind);
        }

        private static int GetRegLimit(OpKind kind)
        {
            return kind switch
            {
                OpKind.REGISTER  => 3,
                OpKind.IMMEDIATE => 2,
                OpKind.JUMP      => 2,
                OpKind.VARIANTS  => 2,
                OpKind.OTHER     => 1,
                _ => throw new ArgumentException($"Null OpKind = {kind}"),
            };
        }
    }
}