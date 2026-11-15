using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Plays imported rig animation states while keeping fixed-camera tap gameplay.
    /// State names are emitted by the Editor rig approval pipeline.
    /// </summary>
    public sealed class MiaMaxRigAnimator : MonoBehaviour
    {
        private Animator animator;
        private Animation legacy;
        private float restoreIdleAt;
        private bool talking;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>(true);
            legacy = GetComponentInChildren<Animation>(true);
            Play("Idle");
        }

        private void Update()
        {
            if (restoreIdleAt > 0f && Time.time >= restoreIdleAt)
            {
                restoreIdleAt = 0f;
                talking = false;
                Play("Idle");
            }
        }

        private void Play(string state)
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                if (HasState(state))
                {
                    animator.CrossFadeInFixedTime(state, .12f, 0);
                    return;
                }
            }
            if (legacy != null && legacy.GetClipCount() > 0)
            {
                if (legacy.GetClip(state) != null)
                    legacy.CrossFade(state, .12f);
                else if (!legacy.isPlaying)
                    legacy.Play();
            }
        }

        private bool HasState(string state)
        {
            int hash = Animator.StringToHash(state);
            return animator != null && animator.HasState(0, hash);
        }

        public void React()
        {
            if (talking) return;
            Play("React");
            restoreIdleAt = Time.time + 1.3f;
        }

        public void Speak(float duration)
        {
            talking = true;
            Play("Talk");
            restoreIdleAt = Time.time + Mathf.Max(.1f, duration);
        }
    }
}
