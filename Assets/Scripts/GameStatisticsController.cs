using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RockPaperScissors
{
    public class GameStatisticsController
    {
        private readonly UIManager _uiManager;
        private readonly Player _user;
        private readonly Player _bot;

        private readonly Dictionary<Player, int> _gameResultsStatistics;

        public GameStatisticsController(UIManager uiManager, Player user, Player bot)
        {
            _uiManager = uiManager;
            _user = user;
            _bot = bot;

            _gameResultsStatistics = new Dictionary<Player, int>
            {
                {_user, 0},
                {_bot, 0}
            };
        }

        public IObservable<Unit> CalculateGameStatistics(Player roundWinner)
        {
            return Observable.Create<Unit>(observer =>
            {
                _gameResultsStatistics[roundWinner]++;

                var view = _uiManager.Show<StatisticsView, StatisticsView.Context>(new StatisticsView.Context
                {
                    UserScore = _gameResultsStatistics[_user],
                    BotScore = _gameResultsStatistics[_bot]
                });

                return view.OnContinue
                    .DoOnCompleted(() => _uiManager.Hide(view))
                    .Subscribe(observer);
            });
        }
    }
}