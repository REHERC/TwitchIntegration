using System;
using System.Text;

namespace TwitchIntegration
{
    public static class StringExtensions
    {
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

        private static int LineBreak(string text, int where, int max)
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
