using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors
{
    public enum PlayerDecisionResult
    {
        Rock = 0,
        Paper = 1,
        Scissors = 2
    }

    public static class PlayerDecisionResultExt
    {
        public static PlayerDecisionResult GetWinResult(this PlayerDecisionResult otherDecisionResult)
        {
            return (PlayerDecisionResult) (((int) otherDecisionResult + 1) % 3);
        }

        public static PlayerDecisionResult GetLooseResult(this PlayerDecisionResult otherDecisionResult)
        {
            return (PlayerDecisionResult) (((int) otherDecisionResult + 2) % 3);
        }

        public static PlayerDecisionResult GetRandomResult()
        {
            return (PlayerDecisionResult) Random.Range(0, 3);
        }

        public static bool IsWinning(this PlayerDecisionResult playerDecisionResult, PlayerDecisionResult otherDecisionResult)
        {
            return playerDecisionResult.GetWinResult() != otherDecisionResult;
        }

        public static IObservable<PlayerDecisionResult> AsButtonClickResultObservable(this PlayerDecisionResult result, Button button)
        {
            return button.OnClickAsObservable().Select(_ => result);
        }
    }
}