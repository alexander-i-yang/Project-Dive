using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Animation
{
    public abstract class AnimationStateManager<T> : MonoBehaviour where T : Enum
    {
        private Animator _animator;

        private string _currentState;

        public abstract Dictionary<T, string> Animations { get; }

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetInitialState(T newState)
        {
            string newStateStr = GetAnimationStr(newState);

            if (newStateStr != null)
            {
                SetState(newStateStr);
            }
        }

        public void ChangeState(T newState)
        {
            string newStateStr = GetAnimationStr(newState);

            if (newStateStr != null)
            {
                ChangeState(newStateStr);
            }
        }

        private void SetState(string newState)
        {
            _animator.Play(newState);
            _currentState = newState;
        }

        private void ChangeState(string newState)
        {
            if (_currentState.Equals(newState))
            {
                return;
            }

            SetState(newState);
        }

        private string GetAnimationStr(T animation)
        {
            string animationStr;
            if (!Animations.TryGetValue(animation, out animationStr))
            {
                Debug.LogError($"Animation {animation} not defined.");
            }
            return animationStr;
        }
    }
}