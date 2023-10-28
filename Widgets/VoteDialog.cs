using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace partymode.Widgets
{
    
    public class VoteDialog : TDialog
    {
        public class VoteItem : TFrame
        {
            TLabel innerLabel;
            public VoteItem(SampSharp.GameMode.SAMP.Color background, Size size, Tuple<int, int, int, int> marginTBLR, PointF pos = default) : 
                base(
                    new IGlobalTD(),
                    new SampSharp.GameMode.SAMP.Color(20, 26, 52, 230),
                    new Size(32, 26),
                    new Tuple<int, int, int, int>(1,1,1,1))
            {
                innerLabel = new TLabel(
                    this.graphic.makeEmptyCopy(),
                    TLabel.DefaultTextStyles.DefaultText,
                    new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Center, 32),
                    new Tuple<int, int, int, int>(1, 1, 2, 2),
                    ""
                    );
            }
        }
        TLabel playModeLabel;
        TLabel scoresLabel;

        public VoteDialog() : 
            base(new IGlobalTD(), 
                new SampSharp.GameMode.Vector2(320,240), 
                VerticalAlignment.Center, 
                HorizontalAlignment.Center, 
                TLabel.DefaultColors.Background) 
        {
            var voteLabel = new TLabel(
                new IGlobalTD(),
                TLabel.DefaultTextStyles.PlayMode,
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Center, 100), new Tuple<int, int, int, int>(4, 0, 2, 2), "Glosuj na nastepna gre:");

            /*scoresLabel = new TLabel(
                new IGlobalTD(),
                TLabel.DefaultTextStyles.DefaultText,
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 500), new Tuple<int, int, int, int>(4, 0, 3, 2), "");
            
            var gamesDoneLabel = new TLabel(
                new IGlobalTD(),
                new TLabel.TextStyle(TLabel.DefaultColors.Title, TLabel.FontSize.Small),
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 500), new Tuple<int, int, int, int>(6, 2, 2, 4), "Rozgrywka ukonczona.");

            addChild(voteLabel);
            addChild(scoresLabel);
            addChild(gamesDoneLabel);*/
        }
        public virtual void showToAll()
        {
            playModeLabel.setText(GameMode.currentPlayMode.getFullName(), false);
            string scoreString = "";
            var players = GameMode.GetPlayers();
            for (int i=0; i< players.Count; i++)
            {
                scoreString += (i+1).ToString() + ". " + players[i].Name + " " + players[i].Score+"~n~";
            }
            scoresLabel.setText(scoreString);
            foreach (var player in players)
            {
                base.show(player);
            }
        }
        public virtual void hideToAll()
        {
            foreach (var player in GameMode.GetPlayers())
            {
                base.hide(player);
            }
        }
    }
}
