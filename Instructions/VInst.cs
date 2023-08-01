namespace rub.Instructions
{
    using Size = Int64;

    public class VInst : Inst
    {

        public VInst(string opcode, string rd, string rs, string imm, LineHolder inst, Size line)
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
            switch (Opcode)
            {
                case "li":
                    CheckValues(Rd, imm: Imm);
                    Text = $"addi {Rd}, $zero, {Imm}";
                    break;
                case "cpy":
                    CheckValues(Rd, Rs);
                    Text = $"add {Rd}, $zero, {Rs}";
                    break;
            }
        }
    }
}