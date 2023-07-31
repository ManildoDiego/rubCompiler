namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;
    public class RInst : Inst
    {
        private readonly string _rs;
        private readonly string _rt;

        public RInst(string opcode, string rd, string rs, string rt, LineHolder inst, Size line) 
            : base(opcode, rd, inst, line)
        {
            _rs = rs;
            _rt = rt;
        }

        public override void Execute()
        {
            CheckValues(Rd, _rs, _rt);
            var isUnsigned = Opcode!.EndsWith("u"); 

            var modOp = isUnsigned ? Opcode[..^1] : Opcode;
            
            var rs = GetRegister(isUnsigned, _rs);
            var rt = GetRegister(isUnsigned, _rt);
            
            Compiler.registers[Rd] = ExecuteLogicalOperation(modOp, rs, rt);
        }
    }
}