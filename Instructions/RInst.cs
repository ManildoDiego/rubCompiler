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
            ExecuteTypeRI(_rs, _rt);
        }
    }
}