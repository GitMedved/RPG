using System;
using MiaMax.Domain.MiniGames;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>Temporary OnGUI arcade shell proving that all three rule sessions can be played and rewarded.</summary>
    public sealed class ArcadeBoardPrototype : MonoBehaviour
    {
        private BackyardInteractable interactable;
        private MiniGameRewardService rewards;
        private Match3Board match3;
        private FishingTimingSession fishing;
        private MaxRunSession runner;
        private string activeGame;
        private string runId;
        private string status = "Choose a mini-game";
        private int selectedCell = -1;
        private float runnerElapsed;
        private bool rewardClaimed;

        public void Configure(BackyardInteractable target)
        {
            interactable = target;
            interactable.Interacted += OpenArcade;
        }

        private void Update()
        {
            if (rewards == null && GameSessionBootstrap.Current != null)
                rewards = new MiniGameRewardService(GameSessionBootstrap.Current);
            if (activeGame == MiniGameRewardService.MaxRun && runner != null && !runner.IsFinished)
            {
                runner.Tick(Time.deltaTime, 8f);
                runnerElapsed += Time.deltaTime;
                if (runnerElapsed >= 18f) runner.Finish();
            }
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OpenArcade;
        }

        private void OpenArcade()
        {
            activeGame = null;
            status = "Choose a mini-game in the arcade panel.";
        }

        private void StartGame(string game)
        {
            activeGame = game;
            runId = Guid.NewGuid().ToString("N");
            rewardClaimed = false;
            selectedCell = -1;
            runnerElapsed = 0f;
            if (game == MiniGameRewardService.Match3) match3 = new Match3Board();
            if (game == MiniGameRewardService.Fishing) fishing = new FishingTimingSession();
            if (game == MiniGameRewardService.MaxRun) runner = new MaxRunSession();
            status = "New run started.";
        }

        private void ClaimReward(int score)
        {
            if (rewardClaimed || rewards == null) return;
            int coins, xp;
            if (rewards.TryClaim(runId, activeGame, score, out coins, out xp))
            {
                rewardClaimed = true;
                status = "Reward: +" + coins + " coins, +" + xp + " XP";
            }
            else status = "Reward unavailable. Please start a new run.";
        }

        private void OnGUI()
        {
            if (activeGame == null)
            {
                GUI.Box(new Rect(16, 16, 300, 190), "Mia & Max Arcade");
                if (GUI.Button(new Rect(30, 50, 270, 38), "Play Match-3")) StartGame(MiniGameRewardService.Match3);
                if (GUI.Button(new Rect(30, 94, 270, 38), "Go Fishing")) StartGame(MiniGameRewardService.Fishing);
                if (GUI.Button(new Rect(30, 138, 270, 38), "Max Runner")) StartGame(MiniGameRewardService.MaxRun);
                GUI.Label(new Rect(16, 212, 340, 44), status);
                return;
            }

            GUI.Box(new Rect(16, 16, 330, 390), "Arcade · " + activeGame);
            if (GUI.Button(new Rect(26, 48, 90, 30), "Arcade menu")) activeGame = null;

            if (activeGame == MiniGameRewardService.Match3) DrawMatch3();
            else if (activeGame == MiniGameRewardService.Fishing) DrawFishing();
            else if (activeGame == MiniGameRewardService.MaxRun) DrawRunner();

            GUI.Label(new Rect(26, 365, 300, 32), status);
        }

        private void DrawMatch3()
        {
            if (match3 == null) return;
            GUI.Label(new Rect(125, 52, 200, 25), "Score: " + match3.Score + " · Moves: " + match3.MovesRemaining);
            for (int y = 0; y < match3.Height; y++)
            for (int x = 0; x < match3.Width; x++)
            {
                int cell = y * match3.Width + x;
                string[] glyphs = { "●", "■", "◆", "▲", "★" };
                string label = glyphs[match3.GetGem(x, y) % glyphs.Length];
                if (GUI.Button(new Rect(28 + x * 36, 88 + y * 32, 34, 30), (selectedCell == cell ? "[" : "") + label))
                {
                    if (selectedCell < 0) selectedCell = cell;
                    else
                    {
                        int sx = selectedCell % match3.Width, sy = selectedCell / match3.Width, points;
                        if (match3.TrySwap(sx, sy, x, y, out points)) status = "+" + points + " points";
                        else status = "That swap does not make a match.";
                        selectedCell = -1;
                    }
                }
            }
            if (match3.MovesRemaining == 0 || !match3.HasPossibleMove())
            {
                if (GUI.Button(new Rect(28, 350, 285, 28), "Collect reward")) ClaimReward(match3.Score);
            }
        }

        private void DrawFishing()
        {
            if (fishing == null) return;
            GUI.Label(new Rect(28, 90, 285, 32), "Target zone: " + fishing.TargetCenter.ToString("0.00"));
            GUI.Label(new Rect(28, 126, 285, 32), "Timing position (0–1)");
            float position = GUI.HorizontalSlider(new Rect(30, 164, 280, 24), fishing.TargetCenter, 0f, 1f);
            if (GUI.Button(new Rect(28, 205, 285, 38), "Cast / try to catch"))
            {
                bool caught = fishing.TryFinish(position);
                status = caught ? "Catch! Quality " + fishing.Quality : "The fish got away. Try another cast.";
                if (caught) ClaimReward(fishing.Quality * 100);
            }
        }

        private void DrawRunner()
        {
            if (runner == null) return;
            GUI.Label(new Rect(28, 88, 285, 30), "Distance: " + runner.Distance.ToString("0") + " · Score: " + runner.Score);
            GUI.Label(new Rect(28, 120, 285, 30), "Lane: " + (runner.Lane + 1) + " / 3");
            if (!runner.IsFinished)
            {
                if (GUI.Button(new Rect(28, 170, 130, 42), "← Left")) runner.MoveLeft();
                if (GUI.Button(new Rect(174, 170, 130, 42), "Right →")) runner.MoveRight();
                if (GUI.Button(new Rect(28, 222, 276, 42), "Collect bonus coin")) runner.CollectCoin(runner.Lane);
                if (GUI.Button(new Rect(28, 274, 276, 38), "Finish run")) runner.Finish();
            }
            else if (!rewardClaimed && GUI.Button(new Rect(28, 222, 276, 42), "Collect reward"))
                ClaimReward(runner.Score);
        }
    }
}
