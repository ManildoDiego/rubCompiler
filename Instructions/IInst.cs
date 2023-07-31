namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;

    public class IInst : Inst
    {
        private readonly string _rs;
        private readonly string _imm;

        public IInst(string opcode, string rd, string rs, string imm, LineHolder inst, Size line)
            : base(opcode, rd, inst, line)
        {
            _rs = rs;
            _imm = imm;
            _errors.SetLine(inst.LineNumber);
            _errors.SetErrorSufix(inst.FilePath);
        }
        
        public override void Execute()
        {
            CheckValues(Rd, _rs, imm: _imm);
            var isUnsigned = Opcode!.EndsWith("u"); 

            var modOp = isUnsigned ? Opcode[..^2] : Opcode[..^1];

            var rs = GetRegister(isUnsigned, _rs);
            var imm = isUnsigned ? Math.Abs(GetImmValue(_imm)) : GetImmValue(_imm);
            
            Compiler.registers[Rd] = ExecuteLogicalOperation(modOp, rs, imm);
        }
    }
}