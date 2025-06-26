using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class AnimatorAction : MonoBehaviour
    {
        public Animator animator;
        public AnimatorControllerParameterType type;
        public string animatorParameterName;
        public float animatorParameterValue;

        private int _parameterID = 0;

        public void Invoke()
        {
            if (animator == null || string.IsNullOrEmpty(animatorParameterName))
            {
                Debug.LogWarning("AnimatorAction is missing an Animator or Parameter Name.", animator);
                return;
            }

            if (_parameterID == 0)
                _parameterID = Animator.StringToHash(animatorParameterName);

            switch (type)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(_parameterID);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(_parameterID, animatorParameterValue > 0.5f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(_parameterID, Mathf.RoundToInt(animatorParameterValue));
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(_parameterID, animatorParameterValue);
                    break;
            }
        }
    }
}
