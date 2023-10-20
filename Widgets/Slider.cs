using System;
using System.Drawing;
using System.Linq;

namespace partymode.Widgets
{
    public class TSlider : TFrame
    {
        SampSharp.GameMode.Vector2 limits = new SampSharp.GameMode.Vector2(0, 100);
        TFrame innerSlider = null;
        private double value = 0;
        int borderWidth = 1;
        public TSlider(
            TextDrawInterface td,
            SampSharp.GameMode.SAMP.Color background,
            SampSharp.GameMode.SAMP.Color foreground,
            Size size, Tuple<int, int, int, int> marginTBLR,
            PointF pos = default) :
            base(td, background, size, marginTBLR)
        {
            innerSlider = new TFrame(
                td.makeEmptyCopy(),
                foreground,
                new Size((int)bbox.Width - borderWidth * 2, (int)bbox.Height - borderWidth * 2),
                new Tuple<int, int, int, int>(0, 0, 0, 0));
        }
        public void setBorderWidth(int width)
        {
            borderWidth = width;
            redraw();
        }
        public double getValue() { return value; }
        public void setValue(double value)
        {
            this.value = value;
            redraw();
        }
        public override void redraw()
        {
            graphic.setWidth(bbox.Right);
            graphic.setHeight(bbox.Bottom);
            graphic.setPosition(new SampSharp.GameMode.Vector2(bbox.X, bbox.Y));
            text = string.Join("", Enumerable.Repeat("~n~", (int)(bbox.Height / 2)));
            var newWidth = (bbox.Width - borderWidth * 2) * Math.Max(limits.X, Math.Min(limits.Y, value)) / limits.Y;
            innerSlider.setFrameInfo(new RectangleF(bbox.X + borderWidth, bbox.Y + borderWidth, (float)newWidth, bbox.Height - borderWidth * 2));
        }
        public override void show(Player player)
        {
            graphic.Show(player);
            innerSlider.show(player);
        }
        public override void hide(Player player)
        {
            graphic.Hide(player);
            innerSlider.hide(player);
        }
    }
}
