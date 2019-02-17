using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors
{
    public sealed class UserView : MonoBehaviour, IPlayerView, IScreen
    {
        [SerializeField] private Button _rockButton;
        [SerializeField] private Button _paperButton;
        [SerializeField] private Button _scissorsButton;

        public IObservable<PlayerDecisionResult> PlayerDecisionAsObservable
        {
            get
            {
                return Observable.Merge(PlayerDecisionResult.Rock.AsButtonClickResultObservable(_rockButton),
                    PlayerDecisionResult.Paper.AsButtonClickResultObservable(_paperButton),
                    PlayerDecisionResult.Scissors.AsButtonClickResultObservable(_scissorsButton));
            }
        }
    }
}