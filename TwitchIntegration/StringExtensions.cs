using System;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable IDE0059
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

            int end = text.Substring(where).IndexOf('\n');
            if (end < 0)
            {
                end = text.Substring(where).Length;
            }
            int i = 0; // stores current position (relative)
            int i_sub = 0; // stores the total size of the tags
            int pos = 0; // stores current position (absolute)
            int valid = -1; //stores the last valid cutting point
            string with = ""; // the text with the tags
            string without = ""; // the text without the tags
            string temp = ""; // temporary string for holding values
            System.Text.RegularExpressions.Group tag = null; // a temporary value to store matches from the regex

            for (i = 0; i <= end;)
            {
                if (without.Length >= max)
                {
                    break;
                }
                if (where + i >= text.Length - 1)
                {
                    break;
                }

                pos = where + i;

                temp = text.Substring(pos);
                match = regex.Match(temp);
                if (match.Success)
                {
                    tag = match.Groups["tag"];
                    if (tag != null && temp.StartsWith(tag.Value))
                    {
                        temp = tag.Value;
                        i_sub += temp.Length;
                        i += temp.Length;

                        with += temp;

                        valid = i;
                        continue;
                    }
                }
                char chr = text[pos];
                if (char.IsWhiteSpace(chr))
                {
                    valid = i + 1;
                }
                with += chr;
                without += chr;
                i++;
            }

            if (i == end + 1)
            {
                valid = end;
            }

            //int result = valid == -1 ? without.Length <= max ? i : max : valid;
            //int result = (without.Length <= max ? with.Length : valid == -1 ? max + i_sub : valid) + 1;
            int result = (without.Length > max ? valid >= 0 ? valid : max + i_sub : with.Length);
            return result;
        }
    }
}
