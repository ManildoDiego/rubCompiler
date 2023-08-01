using rub.RubParser;

namespace rub.Instructions
{
    using Size = Int64;

    public class OInst : Inst
    {
        private readonly string _lastOpcode;

        public OInst(string opcode, string rd, string tag, string lastOpcode, bool condition, LineHolder inst, Size line)
            : base(opcode: opcode,
                   rd: rd,
                   tag: tag,
                   condition: condition,
                   inst: inst,
                   instTracker: line)
        {
            _lastOpcode = lastOpcode;
        }

        public override void Execute()
        {
            switch (Opcode)
            {
                case "print":
                    _errors.Print();
                    CheckValues(Rd);
                    Console.WriteLine(Compiler.registers[Rd]);
                    break;
                case "return":
                    if (!Compiler.returnAddresses.TryPop(out var dirToReturn))
                    {
                        _errors.ReturnWithoutJump();
                    }
                    InstTracker = dirToReturn;
                    break;
                case "leave":
                    _errors.Print();
                    var returnValue = Compiler.registers["$a0"];
                    Console.Read();
                    Environment.Exit((int) returnValue);
                    break;
                case "else":

                    if (!Compiler.opcodes.ContainsKey(_lastOpcode) || Compiler.opcodes[_lastOpcode] != OpKind.JUMP || _lastOpcode == "call")
                    {
                        _errors.ElseWithOutJump();
                        return;
                    }

                    CheckValues(tag: Tag);
                    var dir = JmpInst(Tag, !Condition);
                    if (dir == -1)
                        return;

                    InstTracker = dir;
                    break;
                default:
                    throw new Exception($"Opcode doesn't recognized {Opcode}");
            }
        }
    }
}