using SampSharp.GameMode.Definitions;
using System;
using System.Drawing;
using SampSharp.GameMode;

namespace partymode.Widgets
{
    public class TLabel : TWidget
    {
        public struct FontSize
        {
            public static Vector2 Tiny = new Vector2(0.15, 0.6);
            public static Vector2 Small = new Vector2(0.2, 0.8);
            public static Vector2 Medium = new Vector2(0.3, 1.2);
            public static Vector2 Big = new Vector2(0.35, 1.4);
            public static Vector2 Large = new Vector2(0.45, 1.8);
        }
        public struct DefaultColors
        {
            public static SampSharp.GameMode.SAMP.Color Text = new SampSharp.GameMode.SAMP.Color(255, 255, 255, 255);
            public static SampSharp.GameMode.SAMP.Color Title = new SampSharp.GameMode.SAMP.Color(28, 192, 141, 255);
            public static SampSharp.GameMode.SAMP.Color Background = new SampSharp.GameMode.SAMP.Color(10, 16, 38, 220);
            public static SampSharp.GameMode.SAMP.Color BackgroundNoAlpha = new SampSharp.GameMode.SAMP.Color(10, 16, 38, 255);
            public static SampSharp.GameMode.SAMP.Color Important = new SampSharp.GameMode.SAMP.Color(192, 70, 70, 255);
            public static SampSharp.GameMode.SAMP.Color Pleasant = new SampSharp.GameMode.SAMP.Color(40, 192, 81, 255);
        }
        public class TextStyle
        {
            public readonly SampSharp.GameMode.SAMP.Color color;
            public readonly Vector2 size;

            public TextStyle(SampSharp.GameMode.SAMP.Color color, Vector2 size)
            {
                this.color = color;
                this.size = size;
            }
        }
        public class ContentStyle
        {
            public readonly TextDrawAlignment contentAlignment;
            public readonly int charsPerLine;
            public readonly int bottomMargin = 0;
            public readonly int topMargin = 0;
            public readonly int leftMargin;
            public readonly int rightMargin;
            public readonly bool breaksLine;

            public ContentStyle(
                TextDrawAlignment contentAlignment,
                int charsPerLine,
                bool breaksLine = true)
            {
                this.contentAlignment = contentAlignment;
                this.charsPerLine = charsPerLine;
            }
        }
        public struct DefaultTextStyles
        {
            public static TextStyle PlayMode = new TextStyle(DefaultColors.Title, FontSize.Big);
            public static TextStyle DefaultText = new TextStyle(DefaultColors.Text, FontSize.Small);
            public static TextStyle Greetings = new TextStyle(DefaultColors.Pleasant, FontSize.Medium);
            public static TextStyle Warning = new TextStyle(DefaultColors.Important, FontSize.Medium);
        }

        public readonly ContentStyle contentStyle;


        public TLabel(TextDrawInterface td, TextStyle textStyle, ContentStyle style, Tuple<int, int, int, int> marginsTBLR, string text) :
            base(td, marginsTBLR)
        {
            this.textStyle = textStyle;
            contentStyle = style;
            graphic.setAlignment(style.contentAlignment);
            setText(text);

            graphic.setUseBox(false);
            graphic.setForeColor(textStyle.color);
            graphic.setFont(TextDrawFont.Normal);
            graphic.setLetterSize(textStyle.size);
        }
        public void setText(string text)
        {
            var textInfo = utils.splitEveryNOnSpaces(text, "~n~", contentStyle.charsPerLine);
            int linesCount = textInfo.Item2;
            int textMaxLength = textInfo.Item3;
            this.text = textInfo.Item1;

            size = new Size(
                (int)(textStyle.size.X * 17.3 * textMaxLength) + marginsTBLR.Item3 + marginsTBLR.Item4,
                (int)(textStyle.size.Y * 9 * linesCount) + contentStyle.topMargin + contentStyle.bottomMargin + marginsTBLR.Item1 + marginsTBLR.Item2);
        }
        public override void redraw()
        {

        }
    }
}
