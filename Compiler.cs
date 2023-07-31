using rub.RubParser;

namespace rub
{
	using Save = Int64;
	using Size = Int64;
	
    public class Compiler
	{
		public static readonly Dictionary<string, OpKind> opcodes = new()
		{
			{"add",  OpKind.REGISTER},
			{"sub",  OpKind.REGISTER},
			{"mul",  OpKind.REGISTER},
			{"div",  OpKind.REGISTER},
			{"rem",  OpKind.REGISTER},
			{"sll",  OpKind.REGISTER},
			{"slr",  OpKind.REGISTER},
			{"or",   OpKind.REGISTER},
			{"nor",  OpKind.REGISTER},
			{"and",  OpKind.REGISTER},
			{"nand", OpKind.REGISTER},
			{"xor",  OpKind.REGISTER},

			{"addu",  OpKind.REGISTER},
			{"subu",  OpKind.REGISTER},
			{"mulu",  OpKind.REGISTER},
			{"divu",  OpKind.REGISTER},
			{"remu",  OpKind.REGISTER},
			{"sllu",  OpKind.REGISTER},
			{"slru",  OpKind.REGISTER},
			{"oru",   OpKind.REGISTER},
			{"noru",  OpKind.REGISTER},
			{"andu",  OpKind.REGISTER},
			{"nandu", OpKind.REGISTER},
			{"xoru",  OpKind.REGISTER},

			{"addi",  OpKind.IMMEDIATE},
			{"subi",  OpKind.IMMEDIATE},
			{"muli",  OpKind.IMMEDIATE},
			{"divi",  OpKind.IMMEDIATE},
			{"remi",  OpKind.IMMEDIATE},
			{"slli",  OpKind.IMMEDIATE},
			{"slri",  OpKind.IMMEDIATE},
			{"ori",   OpKind.IMMEDIATE},
			{"andi",  OpKind.IMMEDIATE},
			{"nori",  OpKind.IMMEDIATE},
			{"nandi", OpKind.IMMEDIATE},
			{"xori",  OpKind.IMMEDIATE},
			
			{"addiu",  OpKind.IMMEDIATE},
			{"subiu",  OpKind.IMMEDIATE},
			{"muliu",  OpKind.IMMEDIATE},
			{"diviu",  OpKind.IMMEDIATE},
			{"remiu",  OpKind.IMMEDIATE},
			{"slliu",  OpKind.IMMEDIATE},
			{"slriu",  OpKind.IMMEDIATE},
			{"oriu",   OpKind.IMMEDIATE},
			{"andiu",  OpKind.IMMEDIATE},
			{"noriu",  OpKind.IMMEDIATE},
			{"nandiu", OpKind.IMMEDIATE},
			{"xoriu",  OpKind.IMMEDIATE},

			{"print",  OpKind.OTHER},
			{"return", OpKind.OTHER},
			{"leave",  OpKind.OTHER},
			{"else",   OpKind.OTHER},

			{"cpy",    OpKind.VARIANTS},
			{"li",     OpKind.VARIANTS},
			
			{"call", OpKind.JUMP},

			{"jeq", OpKind.JUMP},
			{"jne", OpKind.JUMP},

			{"jg",  OpKind.JUMP},
			{"jl",  OpKind.JUMP},

			{"jge", OpKind.JUMP},
			{"jle", OpKind.JUMP}
		};

		public static readonly Dictionary<string, Save> registers = new()
		{
			{"$zero", 0L},

			{"$v0", 0L},
			{"$v1", 0L},
			
			{"$a0", 0L},
			{"$a1", 0L},
			{"$a2", 0L},
			{"$a3", 0L},
			{"$a4", 0L},
			{"$a5", 0L},
			{"$a6", 0L},
			{"$a7", 0L},
			
			{"$t0", 0L},
			{"$t1", 0L},
			{"$t2", 0L},
			{"$t3", 0L},
			{"$t4", 0L},
			{"$t5", 0L},
			{"$t6", 0L},
			{"$t7", 0L},
			{"$t8", 0L},
			{"$t9", 0L},

			{"$s0", 0L},
			{"$s1", 0L},
			{"$s2", 0L},
			{"$s3", 0L},
			{"$s4", 0L},
			{"$s5", 0L},
			{"$s6", 0L},
			{"$s7", 0L},

			{"$k0", 0L},
			{"$k1", 0L},

			{"$sp", 0x07fffe10L},

			{"%lo", 0L},
			{"%hi", 0L},
		};

		public static readonly Dictionary<string, Size> tags = new();

		public static readonly Stack<Size> returnAddresses = new();
	}
}