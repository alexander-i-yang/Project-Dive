using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Animation
{
    public abstract class AnimationStateManager<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private AnimationStateReference _animation;
        private Animator _animator;

        private string _currentState;

        public abstract Dictionary<T, string> Animations { get; }

        public Animator Animator => _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Play(T newAnimation)
        {
            string newStateStr = GetAnimationStr(newAnimation);

            if (newStateStr != null)
            {
                _animator.Play(newStateStr);
                _currentState = newStateStr;
            }
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