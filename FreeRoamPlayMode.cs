using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using partymode.Widgets;
using SampSharp.GameMode;
using SampSharp.GameMode.World;

namespace partymode
{
    class FreeRoamPM : PlayMode
    {
        TDialog textDraw;
        public FreeRoamPM() : 
            base("freeroam")
        {
            
        }

        public override void InitializeStatics()
        {
            /*autoBegin = true;
            WeaponItems.AK47.Spawn(new Vector3(1540.5671, 1615.7604, 10.8203125), new Vector3(0,0,0), 30);
            textDraw = new TDialog(
                new IGlobalTD(null),
                new SampSharp.GameMode.Vector2(320, 240), 
                TDialog.VerticalAlignment.Center, 
                TDialog.HorizontalAlignment.Center,
                TLabel.DefaultColors.Background);
            textDraw.addChild(new TLabel(
                new IGlobalTD(null),
                TLabel.DefaultTextStyles.PlayMode,new TLabel.ContentStyle(
                    SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 32, true), 
                new Tuple<int, int, int, int>(0, 0, 0, 0), "Freeroam"));
            textDraw.addChild(new TLabel(
                new IGlobalTD(null),
                TLabel.DefaultTextStyles.DefaultText,
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 50, true),
                new Tuple<int, int, int, int>(0, 0, 0, 0),
                "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."));
            textDraw.addChild(new TLabel(
                new IGlobalTD(null),
                TLabel.DefaultTextStyles.Greetings, 
                new TLabel.ContentStyle(SampSharp.GameMode.Definitions.TextDrawAlignment.Left, 80, true),
                new Tuple<int, int, int, int>(0, 0, 0, 0),
                "Have a good time!"));*/
            /*var someBtn = new Button(
                new TLabel.TextStyle(TLabel.DefaultColors.Title, TLabel.FontSize.Small),
                TLabel.DefaultColors.Background,
                TLabel.DefaultColors.Title,
                new Tuple<int,int,int,int>(5,0,2,0), "Przycisk", playerData.hide);
            playerData.addChild(someBtn);*/

            /*foreach (Player p in GameMode.GetPlayers())
            {
                textDraw.show(p);
            }*/
        }
        public override void OverwriteKillBehaviour(Player killed, BasePlayer killer)
        {
            killed.AddScore(-10);
            (killer as Player).AddScore(50);
        }
        protected override void OnEnd(List<Player> players)
        {
            /*foreach (Player p in GameMode.GetPlayers())
            {
                textDraw.hide(p);
            }
            */
            
        }

        protected override void OnStart(List<Player> players)
        {
            var stopGame = new StopGameRules(this);
            addAttribute(new OverTimeReward(3000, 2 * GameMode.fastScoreMult, (Player player) => { return (player.IsAlive && player.IsConnected && currentState == PlayModeState.BEGAN); }));
            stopGame.addRule(StopGameRules.StopRule.ScoreLimit, 1000);
            stopGame.addRule(StopGameRules.StopRule.TimeLimit, 60000*5);
            addAttribute(stopGame);
        }
        public override bool isAbleToStart(){ return true; }
    }
}
