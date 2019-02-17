using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors
{
    public class StatisticsView : MonoBehaviour, IScreen<StatisticsView.Context>
    {
        [SerializeField] private Text _statisticsText;
        [SerializeField] private Button _continueButton;

        public IObservable<Unit> OnContinue
        {
            get { return _continueButton.OnClickAsObservable().First(); }
        }

        public void Initialize(Context context)
        {
            _statisticsText.text = string.Format("{0} - {1}", context.UserScore, context.BotScore);
        }

        public sealed class Context
        {
            public int UserScore;
            public int BotScore;
        }
    }
}