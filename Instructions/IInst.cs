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
            ExecuteTypeRI(_rs, _imm: _imm, isTypeI: true);
        }
    }
}