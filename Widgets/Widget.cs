using SampSharp.GameMode.Definitions;
using System;
using System.Drawing;
using static partymode.Widgets.TLabel;

namespace partymode.Widgets
{
    public class TWidget
    {
        public event EventHandler requestRedraw;
        public Tuple<int, int, int, int> marginsTBLR { get; protected set; }
        public bool breaksLine { get; set; }
        public TextDrawAlignment alignment { get { return graphic.getAlignment(); } }
        public TextStyle textStyle = DefaultTextStyles.DefaultText;
        public Size size { get; protected set; }
        public string text { get { return graphic.getText(); } protected set { graphic.setText(value); } }
        public TextDrawInterface graphic = null;
        public bool selectable { get { return graphic.getSelectable(); } }
        public TWidget(TextDrawInterface td, Tuple<int, int, int, int> marginsTBLR, bool breaksLine = true)
        {
            this.marginsTBLR = marginsTBLR;
            this.breaksLine = breaksLine;
            graphic = td;
            text = "";
        }
        public virtual void setPosition(float x, float y)
        {
            if(graphic.getPosition().X == x && graphic.getPosition().Y == y) { return; }
            graphic.setPosition(new SampSharp.GameMode.Vector2(x, y));
        }
        public virtual void show(Player player)
        {
            graphic.Show(player);
        }
        public virtual void hide(Player player)
        {
            graphic.Hide(player);
        }
        public virtual void redraw() { }
        protected void raiseRedraw()
        {
            var eh = requestRedraw;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }
    }
}
