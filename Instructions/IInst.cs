namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;

    public class IInst : Inst
    {
        public IInst(string opcode, string rd, string rs, string imm, LineHolder inst, Size line)
            : base(opcode: opcode,
                   rd: rd,
                   rs: rs,
                   imm: imm,
                   inst: inst,
                   instTracker: line)
        {
        }
        
        public override void Execute()
        {
            ExecuteTypeRI(isTypeI: true);
        }
    }
}