namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;
    public class JInst : Inst
    {
        private readonly string _rs;
        private readonly string _imm;
        private readonly string _tag;

        public bool Condition { get; private set; }

        public JInst(string opcode, string rd, string rs, string imm, bool condition, string tag, LineHolder inst, Size line)
            : base(opcode, rd, inst, line)
        {
            _rs = rs;
            _imm = imm;
            Condition = condition;
            _tag = tag;
            _errors.SetLine(inst.LineNumber);
            _errors.SetErrorSufix(inst.FilePath);
        }

        private bool GetJmpCondition(Save op1, Save op2)
        {
            return Opcode switch
            {
                "jeq" => op1 == op2,
                "jne" => op1 != op2,
                "jg"  => op1 > op2,
                "jl"  => op1 < op2,
                "jge" => op1 >= op2,
                "jle" => op1 <= op2,
                _ => throw new Exception($"Opcode didn't recognized in jump instruction {Opcode}"),
            };
        }
        
        public override void Execute()
        {
            if (Opcode != "call")
            {
                var c = _rs == "";
                var rs = c ? null : Rd;
                var imm = c ? null : _imm;

                CheckValues(rs: rs, rt: _rs, imm: imm);

                var op1 = Compiler.registers[Rd];
                var op2 = c ? GetImmValue(_imm) : Compiler.registers[_rs];

                Condition = GetJmpCondition(op1, op2);
            }

            CheckValues(tag: _tag);

            var dir = JmpInst(_tag, Condition);

            if (dir != -1)
            {
                Compiler.returnAddresses.Push(InstTracker);
                InstTracker = dir;
            }
        }
    }
}