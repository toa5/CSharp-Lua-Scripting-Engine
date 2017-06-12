using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MonogameExample
{
    public static class StringExtension
    {
        public static IEnumerable<string> Wrap(this string str, SpriteFont font, float maxWidth)
        {
            if (font.MeasureString(str).X < maxWidth)
            {
                yield return str;
            }
            else
            {
                string[] words = Regex.Split(str, @"(?<=[ \/])");
                StringBuilder wrappedText = new StringBuilder();
                float linewidth = 0f;
                float spaceWidth = font.MeasureString(" ").X;
                for (int i = 0; i < words.Length; ++i)
                {
                    Vector2 size = font.MeasureString(words[i]);
                    if (linewidth + size.X < maxWidth)
                    {
                        linewidth += size.X + spaceWidth;
                    }
                    else
                    {
                        wrappedText.Append('\n');
                        linewidth = size.X + spaceWidth;
                    }
                    wrappedText.Append(words[i]);
                    wrappedText.Append(" ");
                }

                var newWords = wrappedText.ToString().Split('\n');
                foreach (var word in newWords)
                {
                    yield return word;
                }
            }
        }
    }
}
