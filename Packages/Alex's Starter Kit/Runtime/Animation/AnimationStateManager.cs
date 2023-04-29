using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASK.Helpers.Animation
{
    public abstract class AnimationStateManager<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private AnimationStateReference _animation;
        private Animator _animator;

        private string _currentState;

        public abstract Dictionary<T, string> Animations { get; }

        public Animator Animator
        {
            get
            {
                if (_animator == null) return GetComponentInChildren<Animator>(includeInactive:true);
                return _animator;
            }
        }

        private void OnEnable()
        {
            _animator = GetComponentInChildren<Animator>(includeInactive: true);
        }

        public virtual void Play(T newAnimation)
        {
            string newStateStr = GetAnimationStr(newAnimation);

            if (newStateStr != null)
            {
                Animator.Play(newStateStr);
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