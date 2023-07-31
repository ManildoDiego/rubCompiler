using rub.RubParser;

namespace rub.Instructions
{
    using Size = Int64;

    public class OInst : Inst
    {
        private readonly string _tag;
        private readonly string _lastOpcode;
        private readonly bool _condition;

        public OInst(string opcode, string rd, string tag, string lastOpcode, bool condition, LineHolder inst, Size line)
            : base(opcode, rd, inst, line)            
        {
            _tag = tag;
            _lastOpcode = lastOpcode;
            _condition = condition;
            _errors.SetLine(InstTracker);
            _errors.SetLine(inst.LineNumber);
            _errors.SetErrorSufix(inst.FilePath);
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

                    CheckValues(tag: _tag);
                    var dir = JmpInst(_tag, !_condition);
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