using UniRx;

namespace RockPaperScissors
{
    public class RoundResultController
    {
        private readonly UIManager _uiManager;
        private readonly Player _user;
        private readonly Player _bot;

        public RoundResultController(UIManager uiManager, Player user, Player bot)
        {
            _uiManager = uiManager;
            _user = user;
            _bot = bot;
        }

        public IObservable<Player> CalculateResult()
        {
            return _user.LastDecisionResult
                .Zip(_bot.LastDecisionResult, GetRoundWinner)
                .First()
                .ContinueWith(ShowResultWindow);
        }

        private Player GetRoundWinner(PlayerDecisionResult userDecisionResult, PlayerDecisionResult botDecisionResult)
        {
            return userDecisionResult.IsWinning(botDecisionResult) ? _user : _bot;
        }

        private IObservable<Player> ShowResultWindow(Player winner)
        {
            return Observable.Create<Player>(observer =>
            {
                var view = _uiManager.Show<ResultsView, bool>(winner == _user);
                return view.OnAnimationComplete
                    .Select(_ => winner)
                    .DoOnCompleted(() => _uiManager.Hide(view))
                    .Subscribe(observer);
            });
        }
    }
}