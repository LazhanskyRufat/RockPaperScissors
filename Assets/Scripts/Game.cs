using UniRx;
using UnityEngine;

namespace RockPaperScissors
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;

        [SerializeField] private bool _useCheatingBot;
        [SerializeField, Range(0f, 1f)] private float _cheatingBotDifficulty = 0.5f;

        private void Awake()
        {
            _uiManager.Initialize();
            GameLoop().Subscribe().AddTo(this);
        }

        private IObservable<Unit> GameLoop()
        {
            var user = new Player();
            var bot = new Player();
            
            var userController = new UserController(_uiManager, user);
            var botMode = _useCheatingBot
                ? new CheatingBotMode(_cheatingBotDifficulty)
                : (IBotMode) new RandomCasualBotMode();
            var botController = new BotController(_uiManager, bot, user, botMode);

            var roundResultController = new RoundResultController(_uiManager, user, bot);
            var gameStatisticsController = new GameStatisticsController(_uiManager, user, bot);

            return Observable.Concat(userController.MakeDecision(), botController.MakeDecision())
                .ContinueWith(_ => roundResultController.CalculateResult())
                .ContinueWith(gameStatisticsController.CalculateGameStatistics)
                .RepeatUntilDestroy(this)
                .AsUnitObservable();
        }
    }
}