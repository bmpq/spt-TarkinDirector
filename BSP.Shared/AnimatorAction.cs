using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public abstract class AnimatorAction : MonoBehaviour
    {
        public Animator animator;

        [Space(5)]
        public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Bool;
        public string parameterName = "open";
        public float parameterValue = 1f;

        [Space(5)]
        public float delay = 0f;

        private int _parameterID = 0;

        protected void Invoke()
        {
            if (animator == null || string.IsNullOrEmpty(parameterName))
            {
                Debug.LogWarning("AnimatorAction is missing an Animator or Parameter Name.", animator);
                return;
            }

            if (_parameterID == 0)
                _parameterID = Animator.StringToHash(parameterName);

            StartCoroutine(DelayAction(delay));
        }

        private IEnumerator DelayAction(float d)
        {
            if (d > 0)
                yield return new WaitForSeconds(d);

            switch (parameterType)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(_parameterID);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(_parameterID, parameterValue > 0.5f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(_parameterID, Mathf.RoundToInt(parameterValue));
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(_parameterID, parameterValue);
                    break;
            }
        }
    }
}
