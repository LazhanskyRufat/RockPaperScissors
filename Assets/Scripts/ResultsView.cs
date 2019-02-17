using System;
using UniRx;
using UnityEngine;

namespace RockPaperScissors
{
    public sealed class ResultsView : MonoBehaviour, IScreen<bool>
    {
        [SerializeField] private GameObject _winText;
        [SerializeField] private GameObject _looseText;

        [SerializeField] private float _delay;
        
        public IObservable<Unit> OnAnimationComplete
        {
            get { return Observable.Timer(TimeSpan.FromSeconds(_delay)).AsUnitObservable(); }
        }
        
        public void Initialize(bool result)
        {
            _winText.gameObject.SetActive(result);
            _looseText.gameObject.SetActive(!result);
        }
    }
}