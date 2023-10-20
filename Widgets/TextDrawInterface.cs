using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using System;

namespace partymode.Widgets
{
    public interface TextDrawInterface
    {
        public abstract TextDrawAlignment getAlignment();
        public abstract void setAlignment(TextDrawAlignment alignment);

        public abstract bool getUseBox();
        public abstract void setUseBox(bool useBox);

        public abstract SampSharp.GameMode.SAMP.Color getForeColor();
        public abstract void setForeColor(SampSharp.GameMode.SAMP.Color color);

        public abstract SampSharp.GameMode.SAMP.Color getBackColor();
        public abstract void setBackColor(SampSharp.GameMode.SAMP.Color color);

        public abstract SampSharp.GameMode.SAMP.Color getBoxColor();
        public abstract void setBoxColor(SampSharp.GameMode.SAMP.Color color);

        public abstract float getHeight();
        public abstract void setHeight(float height);

        public abstract float getWidth();
        public abstract void setWidth(float width);

        public abstract SampSharp.GameMode.Vector2 getPosition();
        public abstract void setPosition(SampSharp.GameMode.Vector2 pos);

        public abstract bool getSelectable();
        public abstract void setSelectable(bool selectable);

        public abstract string getText();
        public abstract void setText(string text);

        public abstract TextDrawFont getFont();
        public abstract void setFont(TextDrawFont font);

        public abstract int getShadow();
        public abstract void setShadow(int shadow);

        public abstract SampSharp.GameMode.Vector2 getLetterSize();
        public abstract void setLetterSize(SampSharp.GameMode.Vector2 letterSize);

        public abstract void Hide(Player player);
        public abstract void Show(Player player);

        public abstract void addOnClickEvent(Action<Player> someAction);

        public abstract TextDrawInterface makeEmptyCopy();
    }
    public class IPlayerTD : TextDrawInterface
    {
        PlayerTextDraw td = null;
        public IPlayerTD(Player player)
        {
            td = new PlayerTextDraw(player);
        }

        public TextDrawAlignment getAlignment() { return td.Alignment; }
        public void setAlignment(TextDrawAlignment Alignment) { td.Alignment = Alignment; }

        public bool getUseBox() { return td.UseBox; }
        public void setUseBox(bool UseBox) { td.UseBox = UseBox; }

        public SampSharp.GameMode.SAMP.Color getForeColor() { return td.ForeColor; }
        public void setForeColor(SampSharp.GameMode.SAMP.Color ForeColor) { td.ForeColor = ForeColor; }

        public SampSharp.GameMode.SAMP.Color getBackColor() { return td.BackColor; }
        public void setBackColor(SampSharp.GameMode.SAMP.Color BackColor) { td.BackColor = BackColor; }

        public SampSharp.GameMode.SAMP.Color getBoxColor() { return td.BoxColor; }
        public void setBoxColor(SampSharp.GameMode.SAMP.Color BoxColor) { td.BoxColor = BoxColor; }

        public float getHeight() { return td.Height; }
        public void setHeight(float Height) { td.Height = Height; }

        public float getWidth() { return td.Width; }
        public void setWidth(float Width) { td.Width = Width; }

        public SampSharp.GameMode.Vector2 getPosition() { return td.Position; }
        public void setPosition(SampSharp.GameMode.Vector2 Position) { td.Position = Position; }

        public bool getSelectable() { return td.Selectable; }
        public void setSelectable(bool Selectable) { td.Selectable = Selectable; }

        public string getText() { return td.Text; }
        public void setText(string Text) { td.Text = Text; }

        public TextDrawFont getFont() { return td.Font; }
        public void setFont(TextDrawFont Font) { td.Font = Font; }

        public int getShadow() { return td.Shadow; }
        public void setShadow(int Shadow) { td.Shadow = Shadow; }

        public SampSharp.GameMode.Vector2 getLetterSize() { return td.LetterSize; }
        public void setLetterSize(SampSharp.GameMode.Vector2 LetterSize) { td.LetterSize = LetterSize; }

        public void addOnClickEvent(Action<Player> someAction)
        {
            td.Click += (a, b) => someAction.Invoke(b.Player as Player);
        }

        public void Hide(Player player) { td.Hide(); }
        public void Show(Player player) { td.Show(); }

        public TextDrawInterface makeEmptyCopy()
        {
            return new IPlayerTD(td.Owner as Player);
        }
    }
    public class IGlobalTD : TextDrawInterface
    {
        TextDraw td = null;
        Player player;
        public IGlobalTD(Player player)
        {
            td = new TextDraw();
            this.player = player;
        }

        public TextDrawAlignment getAlignment() { return td.Alignment; }
        public void setAlignment(TextDrawAlignment Alignment) { td.Alignment = Alignment; }

        public bool getUseBox() { return td.UseBox; }
        public void setUseBox(bool UseBox) { td.UseBox = UseBox; }

        public SampSharp.GameMode.SAMP.Color getForeColor() { return td.ForeColor; }
        public void setForeColor(SampSharp.GameMode.SAMP.Color ForeColor) { td.ForeColor = ForeColor; }

        public SampSharp.GameMode.SAMP.Color getBackColor() { return td.BackColor; }
        public void setBackColor(SampSharp.GameMode.SAMP.Color BackColor) { td.BackColor = BackColor; }

        public SampSharp.GameMode.SAMP.Color getBoxColor() { return td.BoxColor; }
        public void setBoxColor(SampSharp.GameMode.SAMP.Color BoxColor) { td.BoxColor = BoxColor; }

        public float getHeight() { return td.Height; }
        public void setHeight(float Height) { td.Height = Height; }

        public float getWidth() { return td.Width; }
        public void setWidth(float Width) { td.Width = Width; }

        public SampSharp.GameMode.Vector2 getPosition() { return td.Position; }
        public void setPosition(SampSharp.GameMode.Vector2 Position) { td.Position = Position; }

        public bool getSelectable() { return td.Selectable; }
        public void setSelectable(bool Selectable) { td.Selectable = Selectable; }

        public string getText() { return td.Text; }
        public void setText(string Text) { td.Text = Text; }

        public TextDrawFont getFont() { return td.Font; }
        public void setFont(TextDrawFont Font) { td.Font = Font; }

        public int getShadow() { return td.Shadow; }
        public void setShadow(int Shadow) { td.Shadow = Shadow; }

        public SampSharp.GameMode.Vector2 getLetterSize() { return td.LetterSize; }
        public void setLetterSize(SampSharp.GameMode.Vector2 LetterSize) { td.LetterSize = LetterSize; }

        public void Hide(Player player) { td.Hide(player); }
        public void Show(Player player) { td.Show(player); }

        public void addOnClickEvent(Action<Player> someAction)
        {
            td.Click += (a, b) => someAction.Invoke(b.Player as Player);
        }

        public TextDrawInterface makeEmptyCopy()
        {
            return new IGlobalTD(player);
        }
    }
}
