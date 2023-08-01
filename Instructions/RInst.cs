namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;
    public class RInst : Inst
    {
        public RInst(string opcode, string rd, string rs, string rt, LineHolder inst, Size line) 
            : base(opcode: opcode,
                   rd: rd,
                   rs: rs,
                   rt: rt,
                   inst: inst,
                   instTracker: line)
        {
        }

        public override void Execute()
        {
            ExecuteTypeRI();
        }
    }
}