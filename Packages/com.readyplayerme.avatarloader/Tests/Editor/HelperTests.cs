using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Tests
{
    public class HelperTests
    {
        private readonly List<GameObject> gameObjects = new List<GameObject>();

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var avatar in gameObjects)
            {
                Object.DestroyImmediate(avatar);
            }
            gameObjects.Clear();
        }
        
        [Test]
        public void Setup_Animator_Fullbody()
        {
            var gameObject = new GameObject();
            gameObjects.Add(gameObject);
            gameObject.AddComponent<Animator>();
            AvatarAnimatorHelper.SetupAnimator(BodyType.FullBody, gameObject);
            var animator = gameObject.GetComponent<Animator>();
            Assert.True(animator.runtimeAnimatorController != null);
        }

        [Test]
        public void Setup_Animator_Halfbody()
        {
            var gameObject = new GameObject();
            gameObjects.Add(gameObject);
            AvatarAnimatorHelper.SetupAnimator(BodyType.HalfBody, gameObject);
            var animator = gameObject.GetComponent<Animator>();
            Assert.True(animator == null);
        }
        
        [Test]
        public void Setup_Animator_Null_Avatar()
        {
            AvatarAnimatorHelper.SetupAnimator(BodyType.FullBody, null);
            Assert.Pass();
        }
    }
}
