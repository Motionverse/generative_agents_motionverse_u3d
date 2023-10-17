using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This static class contains useful helper functions used in the <see cref="ReadyPlayerMe" /> API.
    /// </summary>
    public static class AvatarAnimatorHelper
    {
        private const string ANIMATOR_CONTROLLER_NAME = "Avatar Animator";
        private static RuntimeAnimatorController animatorController;

        /// <summary>
        /// Loads and sets the <see cref="RuntimeAnimatorController" /> property of the <c>GameObjects</c>'s
        /// <see cref="Animator" /> component.
        /// </summary>
        /// <param name="bodyType">
        /// The avatar body type. E.g. <see cref="BodyType.FullBody" /> or <see cref="BodyType.HalfBody" />
        /// </param>
        /// <param name="avatar"></param>
        public static void SetupAnimator(BodyType bodyType, GameObject avatar)
        {
            if (bodyType != BodyType.FullBody || avatar == null)
            {
                return;
            }

            if (animatorController == null)
            {
                animatorController = Resources.Load<RuntimeAnimatorController>(ANIMATOR_CONTROLLER_NAME);
            }

            var animator = avatar.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = animatorController;
            }
        }
    }
}
