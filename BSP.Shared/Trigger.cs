using UnityEngine;

namespace tarkin.BSP.Shared
{
    [RequireComponent(typeof(Collider))]
    internal class Trigger : MonoBehaviour
    {
        public Animator animator;
        public AnimatorControllerParameterType type;
        public string animatorParameterName;
        public float animatorParameterValue;

        Collider col;

        void Start()
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;

            if (animator == null)
            {
                Debug.LogError("Animator is not assigned!!!");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(animatorParameterName);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(animatorParameterName, animatorParameterValue > 0.5f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(animatorParameterName, Mathf.RoundToInt(animatorParameterValue));
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(animatorParameterName, animatorParameterValue);
                    break;
            }
        }
    }
}
