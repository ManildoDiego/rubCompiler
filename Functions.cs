namespace rub
{
	using Save = Int64;
	using Size = Int64;

    public class Functions
	{
		
		private static readonly List<string> filePaths = new();

		private static Save ToInt(string value, int numBase)
		{
			return (typeof(Save) == typeof(Int32))
				? (Save) Convert.ToInt32(value, numBase)
				: (Save) Convert.ToInt64(value, numBase);
		}

		public static bool TryParse(string s, int baseNumber, out Save result)
		{
			result = 0;

			if (string.IsNullOrEmpty(s))
			{
				return true;
			}

			if (s.EndsWith(","))
			{
				s = s[..^1];
			}

			try
	        {
	            if (s.StartsWith("-"))
	            {
					s = s[1..];
	                var positiveResult = ToInt(s, baseNumber);
	                result = -positiveResult;
	            }
	            else
	            {
	                result = ToInt(s, baseNumber);
	            }
	            return true;
	        }
	        catch (FormatException)
	        {
	            return false;
	        }
	        catch (OverflowException)
	        {
	            return false;
	        }
		}

		public static LineHolder[] ReadFile(string filePath)
		{
			var fullFilePath = Path.GetFullPath(filePath);
			var tempLines = File.ReadAllLines(filePath);
			List<LineHolder> lines = new();

            for (Size i = 0; i < tempLines.LongLength; i++)
			{
                string l = tempLines[i];
                var line = l.Replace("\t", "");

				lines.Add(new(i+1, line, fullFilePath));
			}

			return lines.ToArray();
		}

		public static List<string> SeparateStr(string str, char delimiter = ' ')
		{
		    List<string> parts = new();
		    string[] tokens = str.Split(delimiter);

		    bool insideString = false;
		    string part = "";

		    foreach (string token in tokens)
		    {
		        if (insideString)
		        {
		            if (!string.IsNullOrEmpty(token) && token.EndsWith("\""))
		            {
		                insideString = false;
		            }
		            part += delimiter + token;
		            continue;
		        }

		        if (!string.IsNullOrEmpty(token) && token.StartsWith("\""))
		        {
		            insideString = true;
		            if (token.Length > 1 && token.EndsWith("\""))
		            {
		                insideString = false;
		            }
		        }

		        parts.Add(part);
		        part = token;
		    }

		    parts.Add(part);
			parts.Remove("");

			if (parts.Count == 0)
			{
				parts.Add("");
			}

		    return parts;
		}

        public static LineHolder[] GetLinesFrom(string filePath)
        {
			var fullFilePath = Path.GetFullPath(filePath);

			if (filePaths.Contains(fullFilePath))
			{
				Program.errors.NameFileAlreadyEntered(filePath);
				return Array.Empty<LineHolder>();
			}
			
			var lines = ReadFile(filePath).ToList();

            for (int i = 0; i < lines.Count; i++)
			{
                var line = lines[i];
                var l = line.Line.Split(' ');
                for (int j = 0; j < l.Length; j++)
                {
                    var part = l[j];
                    var lookahead = (j + 1 < l.Length) ? l[j + 1] : "";

                    if (!lookahead.StartsWith("\"") || !lookahead.EndsWith("\"") || part != ".link" || lookahead == "")
                    {
						continue;
                    }

                    lookahead = lookahead[1..^1];
						
                    if (lookahead == filePath)
                    {
                        Program.errors.NameFileAlreadyEntered(filePath);
                        return Array.Empty<LineHolder>();
                    }

                    lines.Remove(line);
                    var newLines = GetLinesFrom(lookahead);

                    lines.InsertRange(0, newLines);
                }
            }

			return lines.ToArray();
        }
    }
}