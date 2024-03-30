using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyGame { 
    public class AnimatorAgency : MonoBehaviour, IAnimatorEventReceiver
    {
        public IAnimatorEventReceiver animatorEventReceiver;
        void Start() { 
        }
        public void OnAnimatorIK()
        {
            if (animatorEventReceiver != null) animatorEventReceiver.OnAnimatorIK();
        }
        public void OnAnimatorMove()
        {
            if (animatorEventReceiver != null) animatorEventReceiver.OnAnimatorMove();
        }
    }

    public interface IAnimatorEventReceiver
    {
        void OnAnimatorIK();
        void OnAnimatorMove();
    }
}
