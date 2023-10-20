using SampSharp.GameMode.Definitions;
using System;
using System.Drawing;
using System.Linq;
using static partymode.Widgets.TLabel;

namespace partymode.Widgets
{
    namespace GUI
    {
        public class TButton : TFrame
        {
            TLabel innerText = null;
            int borderWidth = 1;

            public TButton(
                TextDrawInterface td,
                TextStyle textStyle,
                SampSharp.GameMode.SAMP.Color bgcolor,
                SampSharp.GameMode.SAMP.Color borderColor,
                Tuple<int, int, int, int> marginsTBLR, string text,
                Action<Player> onClickEvent) :
                base(td, borderColor, new Size(0, 0), marginsTBLR)
            {
                var test = new ContentStyle(TextDrawAlignment.Left, 24);
                innerText = new TLabel(td.makeEmptyCopy(), textStyle, test, new Tuple<int, int, int, int>(0, 0, 0, 0), text);
                graphic.setSelectable(true);
                innerText.graphic.setUseBox(true);
                innerText.graphic.setBoxColor(bgcolor);
                graphic.addOnClickEvent(onClickEvent);
                setText(text);
            }

            public void setBorderWidth(int width)
            {
                borderWidth = width;
                redraw();
            }
            /*public override void setPosition(float x, float y)
            {
                this.bbox = new RectangleF(x, y, innerText.size.Width + 2 * borderWidth, innerText.size.Height + 2 * borderWidth);
                redraw();
            }*/
            public override void show(Player player)
            {
                graphic.Show(player);
                innerText.show(player);
            }
            public override void hide(Player player)
            {
                graphic.Hide(player);
                innerText.hide(player);
            }
            void setText(string text)
            {
                innerText.setText(text);
                size = new Size(innerText.size.Width + 2 * borderWidth, innerText.size.Height + 2 * borderWidth);
            }
            public override void redraw()
            {
                bbox = new RectangleF(bbox.Left, bbox.Top, size.Width, size.Height);
                graphic.setWidth(bbox.Right - 1);
                graphic.setPosition(new SampSharp.GameMode.Vector2(bbox.X, bbox.Y));
                text = string.Join("", Enumerable.Repeat("~n~", (int)(bbox.Height / 2)));
                innerText.setPosition(bbox.X + borderWidth, bbox.Y + borderWidth);
                innerText.graphic.setWidth(bbox.Left + innerText.size.Width);
                innerText.graphic.setShadow(1);
            }
        }
    }
}
