using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

namespace rub
{
	using Size = Int64;

    public class DiagnosticBag
    {
        public DiagnosticBag(string prefix = "", bool isWarning = false)
        {
            _color = isWarning ? ConsoleColor.DarkYellow : ConsoleColor.Red;
            _sufixBag = isWarning ? "WARNING" : "ERROR";
            _prefix = prefix;
        }

        public void SetLine(Size line) => _line = line;

        public void SetErrorSufix(string prefix) => _prefix = prefix;

        public bool Empty() => _printings.ToArray().Length == 0;

        public void Clear() => _printings.Clear();

        private readonly List<(Size? Line, string Error)> _printings = new();

        private void Add(string error, bool showLine = true)
        {
            var line = !showLine ? null : _line;
            _printings.Add((line, error));
        }

        public void AddRange(DiagnosticBag errors)
        {
            _printings.AddRange(errors._printings);
        }

        public void Print()
        {
            if (Empty()) 
                return;

            foreach (var (Line, Error) in _printings)
            {
                var err = (Line == null || Line == 0)
                    ? $"[{_sufixBag} {_prefix}] {Error}" 
                    : $"[{_sufixBag} {_prefix} {Line}] {Error}";

                Console.ForegroundColor = _color;
                Console.WriteLine(err);
                Console.ResetColor();
            }

            Console.ReadLine();
            Environment.Exit(-1);
        }

        public void MayBeString(string str)
        {
            Add($"Bad input \"{str}\". Maybe this is a string? (String format: {stringFormat})");
        }

        public void MayBeTagDefinition(string tag)
        {
            Add($"Bad input \"{tag}\". Maybe this is a tag definition? (Tag definition format: {tagDefinitionFormat})");
        }

        public void MayBeTagCall(string tag)
        {
            Add($"Bad input \"{tag}\". Maybe this is a tag call? (Tag call format: {tagCallFormat})");
        }

        public void UnrecognizedOpcode(string opcode)
        {
            Add($"Opcode \"{opcode}\" doesn't exists");
        }

        public void UnrecognizedKeyWord(string keyWord)
        {
            Add($"Unrecognized KeyWord \"{keyWord}\"");
        }

        public void InvalidRegister(string register, string rName)
        {
            Add($"Invalid {register} register \"{rName}\"");
        }

        public void TagAlreadyExists(string tag)
        {
            Add($"Tag \"{tag}\" already exists");
        }

        public void DirectiveAlreadyExists(string directive)
        {
            Add($"Directive \"{directive}\" already exists");
        }

        public void InvalidBase(string baseStr, string number)
        {
            Add($"Invalid {baseStr} number {number}");
        }

        public void InvalidBase(int _base)
        {
            Add($"Invalid numeric expression base {_base}");
        }

        public void LimitRegistersExceded(int limit, string op, string lastReg)
        {
            string r = (limit == 1) ? "" : "s";
            Add($"There are more than {limit} register{r} in \"{op}\" instruction. \"{lastReg}\" invalid");
        }

        public void RegisterNotExists(string r)
        {
            Add($"Register {r} doesn't exists");
        }

        public void TagNotFound(string tag)
        {
            Add($"{tag} not found", false);
        }

        public void ReturnWithoutJump()
        {
            Add("return without jump instruction");
        }

        public void DirectiveNotFound(string directive)
        {
            Add($"{directive} directive not found", false);
        }

        public void DuplicatedTag(string tag)
        {
            Add($"Duplicate tag {tag}");
        }

        public void NoFile()
        {
            Add("You did not enter the file to compile");
        }

        public void NameFileAlreadyEntered(string name)
        {
            Add($"You have already entered a name file -> {name}");
        }

        public void FileNotExists(string file)
        {
            Add($"No file named \"{file}\"", false);
        }

        public void InvalidImmediate(string imm)
        {
            Add($"Invalid immediate value {imm}");
        }

        public void InfiniteLoop(string tag)
        {
            Add($"Infinite loop calling {tag} tag");
        }

        public void MissingArgument(string arg)
        {
            Add($"Missing argument in {arg}");
        }

        public void InvalidTag(string tag)
        {
            Add($"Invalid tag {tag}");
        }

        public void BadInput(string token)
        {
            Add($"Bad input found: {token}");
        }

        public void DuplicatedOpcode(string actualOp, string duplicatedOp)
        {
            Add($"Duplicated opcode, actual: \"{actualOp}\", duplicated: {duplicatedOp}");
        }

        public void ElseWithOutJump()
        {
            Add($"\"else\" wihtout a jump instruction");
        }

        public void CompilerError(string error)
        {
            Add($"A comipler error has happend! {error}");
        }

        private readonly string _sufixBag;
        private readonly ConsoleColor _color;
        
        private static readonly string stringFormat = "\"<string>\"";
        private static readonly string tagDefinitionFormat = "@<tag name>:";
        private static readonly string tagCallFormat = tagDefinitionFormat[..^1];


        private Size? _line;
        private string _prefix = "";
    }
}