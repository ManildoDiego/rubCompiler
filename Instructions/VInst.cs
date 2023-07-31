using System.Drawing;

namespace rub.Instructions
{
    using Size = Int64;

    public class VInst : Inst
    {
        private readonly string _rs;
        private readonly string _imm;

        public string Text { get; private set; } 

        public VInst(string opcode, string rd, string rs, string imm, LineHolder inst, Size line)
            : base(opcode, rd, inst, line)            
        {
            _rs = rs;
            _imm = imm;
            Text = "";
            _errors.SetLine(inst.LineNumber);
            _errors.SetErrorSufix(inst.FilePath);
        }
        
        public override void Execute()
        {
            switch (Opcode)
            {
                case "li":
                    CheckValues(Rd, imm: _imm);
                    Text = $"addi {Rd}, $zero, {_imm}";
                    break;
                case "cpy":
                    CheckValues(Rd, _rs);
                    Text = $"add {Rd}, $zero, {_rs}";
                    break;
            }
        }
    }
}