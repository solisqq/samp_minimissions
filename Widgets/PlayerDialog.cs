using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace partymode.Widgets
{
    public class PlayerDialog : TDialog
    {
        Player player;
        TSlider armTSlider;
        TSlider hpTSlider;
        TLabel score;
        TLabel overallScore;

        public PlayerDialog(Player player) :
            base(new IPlayerTD(player), new SampSharp.GameMode.Vector2(547, 26),
                TDialog.VerticalAlignment.Top, TDialog.HorizontalAlignment.Left,
                TLabel.DefaultColors.BackgroundNoAlpha)
        {
            addChild(new TLabel(
                new IPlayerTD(player),
                new TLabel.TextStyle(TLabel.DefaultColors.Title, TLabel.FontSize.Medium),
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Center, 16, true),
                new Tuple<int, int, int, int>(2, 0, 0, 0), "WZGaming"));

            addChild(new TLabel(
                new IPlayerTD(player),
                new TLabel.TextStyle(TLabel.DefaultColors.Pleasant, TLabel.FontSize.Small),
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Right, 16, true),
                new Tuple<int, int, int, int>(0, 2, 0, 4), "Freeroam"));

            var currentscore = Database.instance.get<int>("score", "samp_player", "name", player.Name, 0);

            overallScore = new TLabel(
                new IPlayerTD(player),
                TLabel.DefaultTextStyles.DefaultText,
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 32, true),
                new Tuple<int, int, int, int>(0, 0, 4, 0), "Punkty: ~w~"+ currentscore.Value.ToString());
            addChild(overallScore);

            armTSlider = new TSlider(
                new IPlayerTD(player),
                new SampSharp.GameMode.SAMP.Color(21, 23, 30, 240),
                new SampSharp.GameMode.SAMP.Color(224, 224, 224, 250),
                new Size(64, 10),
                new Tuple<int, int, int, int>(4, 0, 4, 4)
                );
            armTSlider.setValue(player.Armour);
            addChild(armTSlider);

            hpTSlider = new TSlider(
                new IPlayerTD(player),
                new SampSharp.GameMode.SAMP.Color(21, 23, 30, 240),
                new SampSharp.GameMode.SAMP.Color(204, 82, 51, 250),
                new Size(64, 10),
                new Tuple<int, int, int, int>(4, 0, 4, 4)
                );
            hpTSlider.setValue(player.Health);
            addChild(hpTSlider);
            score = new TLabel(
                new IPlayerTD(player),
                TLabel.DefaultTextStyles.DefaultText,
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 32, true),
                new Tuple<int, int, int, int>(4, -2, 4, 0), "Wynik: ~w~20");
            addChild(score);
            this.player = player;
            player.Update += Player_Update;
        }

        private void Player_Update(object sender, SampSharp.GameMode.Events.PlayerUpdateEventArgs e)
        {
            if(hpTSlider.getValue()!=player.Health) hpTSlider.setValue(player.Health);
            if(armTSlider.getValue()!=player.Armour) armTSlider.setValue(player.Armour);
        }
        public void updateScore(int score)
        {
            this.score.setText("Wynik: ~w~" + score.ToString());
        }
        public void addToOverallScore(int toAdd)
        {
            var score = Database.instance.get<int>("score", "samp_player", "name", player.Name, 0);
            int newScore = score.Value + toAdd;
            Database.instance.set(new Dictionary<string, object>() { { "score", newScore } }, "samp_player", "name", player.Name);
            this.overallScore.setText("Punkty: ~w~" + newScore.ToString());
        }
    }
}
