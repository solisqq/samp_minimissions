using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace partymode.Widgets
{

    public class TDialog
    {
        static int MAX_WIDTH = 640;
        static int MAX_HEIGHT = 480;
        TFrame background = null;
        private readonly SampSharp.GameMode.Vector2 pos;
        private readonly VerticalAlignment valing;
        private readonly HorizontalAlignment haling;
        public readonly Size size;

        public enum VerticalAlignment
        {
            Bottom = 0, Top = 1, Center = 2
        }
        public enum HorizontalAlignment
        {
            Left = 0, Right = 1, Center = 2
        }
        private readonly List<TWidget> widgets = new List<TWidget>();
        public bool clickable = false;
        public TDialog(TextDrawInterface tdif, SampSharp.GameMode.Vector2 pos, VerticalAlignment valing, HorizontalAlignment haling, SampSharp.GameMode.SAMP.Color color)
        {
            this.pos = pos;
            this.valing = valing;
            this.haling = haling;
            background = new TFrame(tdif, color, new Size(1, 1), new Tuple<int, int, int, int>(0, 0, 0, 0));
        }

        public void addChild(TWidget tdhandler)
        {
            widgets.Add(tdhandler);
            redraw();
        }

        public void show(Player player)
        {
            background.show(player);
            foreach (var widget in widgets)
            {
                widget.show(player);
            }
            if (clickable) { player.SelectTextDraw(new SampSharp.GameMode.SAMP.Color(64, 255, 136, 250)); }
        }
        public void hide(Player player)
        {
            background.hide(player);
            foreach (var widget in widgets)
            {
                widget.hide(player);
            }
            player.CancelSelectTextDraw();
        }
        private void redraw()
        {
            RectangleF bbox = new RectangleF(
                pos.X, pos.Y,
                widgets.Max(obj => obj.size.Width + obj.marginsTBLR.Item3 + obj.marginsTBLR.Item4),
                widgets.Sum(obj => obj.size.Height + obj.marginsTBLR.Item1 + obj.marginsTBLR.Item2));

            if (valing == VerticalAlignment.Center) bbox.Y -= bbox.Height / 2;
            else if (valing == VerticalAlignment.Bottom) bbox.Y -= bbox.Height;
            if (haling == HorizontalAlignment.Center) bbox.X -= bbox.Width / 2;
            else if (haling == HorizontalAlignment.Right) bbox.X -= bbox.Width;

            background.setFrameInfo(bbox);

            float currentY = bbox.Y;

            foreach (var widget in widgets)
            {
                float tdX = 0;
                float tdY = currentY + widget.marginsTBLR.Item1;
                if (widget.breaksLine) currentY += widget.size.Height + widget.marginsTBLR.Item1 + widget.marginsTBLR.Item2;
                if (widget.graphic.getAlignment() == SampSharp.GameMode.Definitions.TextDrawAlignment.Center)
                    tdX = bbox.Left + bbox.Width / 2 + widget.marginsTBLR.Item3 - widget.marginsTBLR.Item4;
                else if (widget.graphic.getAlignment() == SampSharp.GameMode.Definitions.TextDrawAlignment.Right)
                    tdX = bbox.Right - widget.marginsTBLR.Item4;
                else if (widget.graphic.getAlignment() == SampSharp.GameMode.Definitions.TextDrawAlignment.Left)
                    tdX = bbox.Left + widget.marginsTBLR.Item3;
                widget.setPosition(tdX, tdY);
            }
        }
    }
}
