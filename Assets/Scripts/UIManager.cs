using System;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors
{
    public class UIManager : MonoBehaviour
    {
        private readonly Dictionary<Type, GameObject> _screens = new Dictionary<Type, GameObject>();
        private readonly Dictionary<Type, GameObject> _activeScreens = new Dictionary<Type, GameObject>();

        public void Initialize()
        {
            foreach (Transform childTransform in transform)
            {
                _screens[childTransform.GetComponent<IScreen>().GetType()] = childTransform.gameObject;
            }
        }
        
        public T Show<T>() where T : Component, IScreen
        {
            var screen = _screens[typeof(T)].GetComponent<T>();
            screen.gameObject.SetActive(true);
            _activeScreens[screen.GetType()] = screen.gameObject;
            return screen;
        }

        public T Show<T, U>(U context) where T : Component, IScreen<U>
        {
            var screen = Show<T>();
            screen.Initialize(context);
            return screen;
        }

        public void Hide<T>() where T : Component, IScreen
        {
            var screenType = typeof(T);
            _activeScreens[screenType].gameObject.SetActive(false);
            _activeScreens.Remove(screenType);
        }

        public void Hide<T>(T screen) where T : Component, IScreen
        {
            Hide<T>();
        }
    }

    public interface IScreen
    {}
    
    public interface IScreen<U> : IScreen
    {
        void Initialize(U context);
    }
}