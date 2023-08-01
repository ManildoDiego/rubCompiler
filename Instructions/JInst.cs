namespace rub.Instructions
{
    using Save = Int64;
    using Size = Int64;
    public class JInst : Inst
    {
        public JInst(string opcode, string rs, string rt, string imm, bool condition, string tag, LineHolder inst, Size line)
            : base(opcode: opcode,
                   rs: rs,
                   rt: rt,
                   imm: imm,
                   condition: condition,
                   tag: tag,
                   inst: inst,
                   instTracker: line)
        {
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
                var c = Rt == "";
                var toSee1 = c ? null : Rt;
                var toSee2 = !c ? null : Imm;
                
                CheckValues(rs: Rs, rt: toSee1, imm: toSee2);

                var op1 = Compiler.registers[Rs];
                var op2 = c ? GetImmValue(Imm) : Compiler.registers[Rs];

                Condition = GetJmpCondition(op1, op2);
            }

            CheckValues(tag: Tag);

            var dir = JmpInst(Tag, Condition);

            if (dir != -1)
            {
                Compiler.returnAddresses.Push(InstTracker);
                InstTracker = dir;
            }
        }
    }
}