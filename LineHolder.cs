namespace rub
{
	using Size = Int64;

    public class LineHolder
	{
		public LineHolder(Size lineNumber, string line, string filePath)
		{
            LineNumber = lineNumber;
            Line = line;
            FilePath = filePath;
        }

        public Size LineNumber { get; }
        public string Line { get; }
        public string FilePath { get; }
    }
}