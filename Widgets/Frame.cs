using SampSharp.GameMode.Definitions;
using System;
using System.Drawing;
using System.Linq;

namespace partymode.Widgets
{
    public class TFrame : TWidget
    {
        protected RectangleF bbox;
        public TFrame(TextDrawInterface td,
            SampSharp.GameMode.SAMP.Color background,
            Size size, Tuple<int, int, int, int> marginTBLR,
            PointF pos = default) :
            base(td, marginTBLR, true)
        {
            bbox = new RectangleF(pos, size);
            this.size = size;
            graphic.setAlignment(TextDrawAlignment.Left);
            graphic.setFont(TextDrawFont.Normal);
            graphic.setLetterSize(new SampSharp.GameMode.Vector2(0.25, 0.25));
            graphic.setForeColor(new SampSharp.GameMode.SAMP.Color(0, 0, 0, 0));
            graphic.setUseBox(true);
            graphic.setBoxColor(background);
        }
        public void setFrameInfo(RectangleF bbox)
        {
            this.bbox = bbox;
            redraw();
        }
        public override void setPosition(float x, float y)
        {
            bbox = new RectangleF(x, y, bbox.Width, bbox.Height);
            redraw();
        }
        public override void redraw()
        {
            graphic.setWidth(bbox.Right);
            graphic.setHeight(bbox.Bottom);
            graphic.setPosition(new SampSharp.GameMode.Vector2(bbox.X, bbox.Y));
            text = string.Join("", Enumerable.Repeat("~n~", (int)(bbox.Height / 2)));
        }
    }
}
