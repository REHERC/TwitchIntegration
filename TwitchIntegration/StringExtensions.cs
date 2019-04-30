using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchIntegration
{
    public static class StringExtensions
    {
        const string RegexPattern = "(?<tag>[<][/]?[a-zA-Z0-9=\\\"#]+[>])";

        //TODO: Modify to recognise tags with regex dark magic
        public static string WordWrap(this string text, int lineLength)
        {
            int position;
            int next;
            var sb = new StringBuilder();

            if (lineLength < 1)
                return text;

            for (position = 0; position < text.Length; position = next)
            {
                int lineEnd = text.IndexOf('\n', position);
                if (lineEnd == -1)
                    next = lineEnd = text.Length;
                else
                    next = lineEnd + 1;

                if (lineEnd > position)
                {
                    do
                    {
                        int length = lineEnd - position;

                        if (length > lineLength)
                            length = LineBreak(text, position, lineLength);

                        sb.Append(text, position, length);
                        sb.Append('\n');

                        position += length;
                        while (position < lineEnd && char.IsWhiteSpace(text[position]))
                            position++;
                    } while (lineEnd > position);
                }
                else sb.Append('\n');
            }
            return sb.ToString();
        }

        // returns a length, not a position
        private static int LineBreak(string text, int where, int max)
        {
            Regex regex = new Regex(RegexPattern);
            Match match = null;

            int i = 0; // stores current position (relative)
            int i_sub = 0; // stores the total size of the tags
            int pos = 0; // stores current position (absolute)
            int valid = 0; //stores the last valid cutting point
            string with = ""; // the text with the tags
            string without = ""; // the text without the tags
            string temp = ""; // a temporary value to store matches from the regex
            System.Text.RegularExpressions.Group tag = null; // a temporary value to store matches from the regex

            //for (i = 0; i < max;)
            while (i <= max)
            {
                if (without.Length > max)
                {
                    break;
                }

                pos = where + i;

                match = regex.Match(text.Substring(pos));
                // if the text from i starts with a tag
                if (match.Success && (tag = match.Groups["tag"]) != null && text.Substring(pos).StartsWith(temp = tag.Value))
                {
                    i += temp.Length;
                    //i_sub += temp.Length;
                    valid = i;
                    with += temp;
                    continue;
                }
                else
                {
                    if (char.IsWhiteSpace(text[pos]))
                    {
                        valid = i;
                    }
                    with += text[pos];
                    without += text[pos];
                }
                i++;
            }

            /*while (i >= 0 && !char.IsWhiteSpace(text[where + i]))
                i--;
            if (i < 0)
                return max;
            while (i >= 0 && char.IsWhiteSpace(text[where + i]))
                i--;*/
            //return (valid == 0 ? with.Length < max ? with.Length : max : valid + 1);
            int ret = with.Length <= max ? with.Length : valid == 0 ? max : valid;
            Console.WriteLine($"{with} - {ret}");
            return ret;
        }

        private static int OldLineBreak(string text, int where, int max)
        {
            int i = max;
            while (i >= 0 && !char.IsWhiteSpace(text[where + i]))
                i--;

            if (i < 0)
                return max;

            while (i >= 0 && char.IsWhiteSpace(text[where + i]))
                i--;

            return i + 1;
        }
    }
}
