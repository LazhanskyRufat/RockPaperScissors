using UniRx;

namespace RockPaperScissors
{
    public interface IPlayerView
    {
        IObservable<PlayerDecisionResult> PlayerDecisionAsObservable { get; }
    }
}