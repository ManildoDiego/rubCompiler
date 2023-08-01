namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;

    public abstract class Inst 
    {
        protected readonly DiagnosticBag _errors = new();

        protected string Opcode { get; }
        protected string Rd { get; }
        protected string Rs { get; }
        protected string Rt { get; }
        protected string Imm { get; }
        protected string Tag { get; }
        public bool Condition { get; protected set; }
        public string Text { get; protected set; } 

        public Size InstTracker { get; protected set; }
        public DiagnosticBag Errors => _errors;

        protected Inst(string opcode, LineHolder inst, Size instTracker, string rd = "", string rs = "", string rt = "", string imm = "", string tag = "", bool condition = false)
        {
            Opcode = opcode;
            Rd = rd;
            Rs = rs;
            Rt = rt;
            Imm = imm;
            Tag = tag;
            Condition = condition;
            Text = "";

            InstTracker = instTracker;

            _errors.SetLine(inst.LineNumber);
            _errors.SetErrorSufix("rub " + inst.FilePath);
        }
        
        public abstract void Execute();

        private static Save GetRegister(bool isUnsigned, string reg)
        {
            var rValue = Compiler.registers[reg];
            return isUnsigned ? Math.Abs(rValue) : rValue;
        }

        protected void ExecuteTypeRI(bool isTypeI = false)
        {
            var secondOp = !isTypeI ? Rt : Imm;

            if (!isTypeI)
            {
                CheckValues(Rd, Rs, secondOp);
            }
            else
            {
                CheckValues(Rd, Rs, imm: secondOp);
            }

            var isUnsigned = Opcode.EndsWith("u");

            var modOp = 
                !isTypeI 
                    ? isUnsigned ? Opcode[..^1] : Opcode 
                    : isUnsigned ? Opcode[..^2] : Opcode[..^1];

            var op1 = GetRegister(isUnsigned, Rs);
            var op2 = 
                !isTypeI 
                    ? GetRegister(isUnsigned, secondOp) 
                    : isUnsigned ? Math.Abs(GetImmValue(secondOp)) : GetImmValue(secondOp);

            Compiler.registers[Rd] = ExecuteLogicalOperation(modOp, op1, op2);
        }

        private static Save ExecuteLogicalOperation(string op, Save rs, Save rt) => op switch
        {
            "add" => rs + rt,
            "sub" => rs - rt,
            "mul" => rs * rt,
            "div" => rs / rt,
            "rem" => rs % rt,

            "sll" => (int)rs << (int)rt,
            "slr" => (int)rs >> (int)rt,

            "or" => rs | rt,
            "and" => rs & rt,
            "xor" => rs ^ rt,
            "nor" => ~(rs | rt),
            "nand" => ~(rs & rt),
            "xnor" => ~(rs ^ rt),
            _ => throw new Exception($"Invalid operation: {op}"),
        };

        protected Save GetImmValue(string number)
        {
            var immBase = GetBase(ref number);

            var baseStr = immBase switch
            {
                16 => "hexadecimal",
                10 => "decimal",
                8  => "octal",
                2  => "binary",
                _  => throw new InvalidOperationException($"Invalid base {immBase}")
            };

            if (!Functions.TryParse(number, immBase, out var value))
            {
                _errors.InvalidBase(baseStr, number);
            }

            return value;
        }

        private static int GetBase(ref string num)
        {
            int immBase = 10;

            if (num.Length >= 2 && (num[0] == '0' || (num[0] == '-' && num[1] == '0')))
            {
                var toProbe = (num[0] == '-' && num[1] == '0') ? num[2] : num[1];

                immBase = toProbe switch
                {
                    'x' => 16,
                    'o' => 8,
                    'b' => 2,
                    _   => 10,
                };

                switch (toProbe)
                {
                    case 'x': immBase = 16; break;
                    case 'o': immBase = 8; break;
                    case 'b': immBase = 2; break;
                }

                if (immBase != 10)
                {
                    var firstChar = num[0];
                    var isNegative = firstChar == '-';
                    var indexToRemove = isNegative ? 3 : 2;

                    num = num[indexToRemove..];

                    if (isNegative)
                    {
                        num = '-' + num;
                    }
                }
            }

            return immBase;
        }

        protected void CheckValues(string? rd = null, string? rs = null, string? rt = null, string? imm = null, string? tag = null)
        {
            if (rd == "")
            {
                _errors.InvalidRegister("destiny", rd);
            }

            if (rs == "")
            {
                _errors.InvalidRegister("source", rs);
            }

            if (rt == "")
            {
                _errors.InvalidRegister("target", rt);
            }

            if (imm == "")
            {
                _errors.InvalidImmediate(imm);
            }

            if (tag == "")
            {
                _errors.InvalidTag(tag);
            }

            _errors.Print();
        }

        protected Size JmpInst(string tag, bool condition)
        {
            if (!condition)
                return -1;
            
            if (!Compiler.tags.ContainsKey(tag))
            {
                _errors.TagNotFound(tag);
                return -1;
            }

            return Compiler.tags[tag];
        }
    } 
}