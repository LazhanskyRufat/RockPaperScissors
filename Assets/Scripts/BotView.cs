using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors
{
    public class BotView : MonoBehaviour, IPlayerView, IScreen<IObservable<PlayerDecisionResult>>, ISerializationCallbackReceiver
    {
        [SerializeField] private float _makeDecisionDelay = 3f;
        [SerializeField] private float _showDecisionDelay = 3f;
        
        [SerializeField] private Button _rockButton;
        [SerializeField] private Button _paperButton;
        [SerializeField] private Button _scissorsButton;

        [SerializeField] private ColorBlock _animatedColorBlock;
        [SerializeField] private ColorBlock _selectedColorBlock;
        
        private IObservable<PlayerDecisionResult> _playerDecisionResultObservable;
        private Dictionary<PlayerDecisionResult, Button> _buttons;

        private Button _selectedButton;

        private Coroutine _animationCoroutine;
        
        public IObservable<PlayerDecisionResult> PlayerDecisionAsObservable
        {
            get { return GetDecision().ContinueWith(ShowAnimation).ContinueWith(ShowDecision); }
        }

        public void Initialize(IObservable<PlayerDecisionResult> playerDecisionResultObservable)
        {
            _playerDecisionResultObservable = playerDecisionResultObservable;
        }
        
        private IObservable<PlayerDecisionResult> ShowAnimation(PlayerDecisionResult decisionResult)
        {
            return Observable.Timer(TimeSpan.FromSeconds(_makeDecisionDelay))
                .DoOnCompleted(StopAnimation)
                .Select(_ => decisionResult);
        }

        private IObservable<PlayerDecisionResult> ShowDecision(PlayerDecisionResult decisionResult)
        {
            return Observable.Timer(TimeSpan.FromSeconds(_showDecisionDelay))
                .DoOnSubscribe(() => StartShowDecision(decisionResult))
                .DoOnCompleted(EndShowDecisionResult)
                .Select(_ => decisionResult);
        }

        private IObservable<PlayerDecisionResult> GetDecision()
        {
            return _playerDecisionResultObservable.DoOnSubscribe(StartAnimation);
        }

        private void StartAnimation()
        {
            _animationCoroutine = StartCoroutine(ButtonsAnimationRoutine());
        }

        private void StopAnimation()
        {
            StopCoroutine(_animationCoroutine);
            ResetSelectedButtonColor();
        }

        private void StartShowDecision(PlayerDecisionResult result)
        {
            _selectedButton = _buttons[result];
            _selectedButton.colors = _selectedColorBlock;
        }

        private void EndShowDecisionResult()
        {
            ResetSelectedButtonColor();
        }
        
        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            _buttons = new Dictionary<PlayerDecisionResult, Button>
            {
                {PlayerDecisionResult.Rock, _rockButton},
                {PlayerDecisionResult.Paper, _paperButton},
                {PlayerDecisionResult.Scissors, _scissorsButton}
            };
        }

        private IEnumerator ButtonsAnimationRoutine()
        {            
            for (;;)
            {
                yield return new WaitForSeconds(0.3f);

                ResetSelectedButtonColor();

                _selectedButton = _buttons[PlayerDecisionResultExt.GetRandomResult()];
                _selectedButton.colors = _animatedColorBlock;
                
                yield return null;
            }
        }

        private void ResetSelectedButtonColor()
        {
            if (_selectedButton != null)
            {
                _selectedButton.colors = ColorBlock.defaultColorBlock;
            }
        }
    }
}