using UniRx;
using UnityEngine;

namespace RockPaperScissors
{
    public sealed class Player
    {
        private readonly ReplaySubject<PlayerDecisionResult> _lastDecisionResult =
            new ReplaySubject<PlayerDecisionResult>(1);

        public IObservable<PlayerDecisionResult> LastDecisionResult
        {
            get { return _lastDecisionResult; }
        }

        public void UpdateLastDecisionResult(PlayerDecisionResult decisionResult)
        {
            _lastDecisionResult.OnNext(decisionResult);
        }
    }

    public abstract class PlayerController
    {
        public abstract IObservable<PlayerDecisionResult> MakeDecision();
    }

    public sealed class UserController : PlayerController
    {
        private readonly UIManager _uiManager;
        private readonly Player _user;

        public UserController(UIManager uiManager, Player user)
        {
            _uiManager = uiManager;
            _user = user;
        }

        public override IObservable<PlayerDecisionResult> MakeDecision()
        {
            return Observable.Create<PlayerDecisionResult>(observer =>
            {
                var userView = _uiManager.Show<UserView>();
                return userView.PlayerDecisionAsObservable
                    .Do(_user.UpdateLastDecisionResult)
                    .First()
                    .DoOnCompleted(() => _uiManager.Hide(userView))
                    .Subscribe(observer);
            });
        }
    }

    public sealed class BotController : PlayerController
    {
        private readonly UIManager _uiManager;
        private readonly Player _bot;
        private readonly Player _player;
        private readonly IBotMode _botMode;

        public BotController(UIManager uiManager, Player bot, Player player, IBotMode botMode)
        {
            _uiManager = uiManager;
            _bot = bot;
            _player = player;
            _botMode = botMode;
        }

        public override IObservable<PlayerDecisionResult> MakeDecision()
        {
            return Observable.Create<PlayerDecisionResult>(observer =>
            {
                var botView =
                    _uiManager.Show<BotView, IObservable<PlayerDecisionResult>>(
                        _botMode.MakeDecision(_player.LastDecisionResult));
                return botView.PlayerDecisionAsObservable
                    .Do(_bot.UpdateLastDecisionResult)
                    .First()
                    .DoOnCompleted(() => _uiManager.Hide(botView))
                    .Subscribe(observer);
            });
        }
    }

    public interface IBotMode
    {
        IObservable<PlayerDecisionResult> MakeDecision(IObservable<PlayerDecisionResult> playerDecision);
    }

    public class CheatingBotMode : IBotMode
    {
        private readonly float _botDifficulty;

        public CheatingBotMode(float botDifficulty)
        {
            _botDifficulty = botDifficulty;
        }

        public IObservable<PlayerDecisionResult> MakeDecision(IObservable<PlayerDecisionResult> playerDecision)
        {
            return playerDecision.Select(MakeDecision).First();
        }

        private PlayerDecisionResult MakeDecision(PlayerDecisionResult playerResult)
        {
            return Random.Range(Mathf.Epsilon, 1f) <= _botDifficulty
                ? playerResult.GetWinResult()
                : playerResult.GetLooseResult();
        }
    }

    public sealed class CasualBotMode : CheatingBotMode
    {
        public CasualBotMode() : base(0.5f)
        {
        }
    }

    public sealed class RandomCasualBotMode : IBotMode
    {
        public IObservable<PlayerDecisionResult> MakeDecision(IObservable<PlayerDecisionResult> playerDecision)
        {
            return playerDecision.Select(_ => PlayerDecisionResultExt.GetRandomResult()).First();
        }
    }
}