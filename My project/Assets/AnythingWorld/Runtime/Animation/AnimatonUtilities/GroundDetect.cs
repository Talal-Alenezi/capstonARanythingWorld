using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Ground detection for creature animation.
    /// </summary>
    public class GroundDetect : MonoBehaviour
    {
        public delegate void HitsGroundDelegate();
        public HitsGroundDelegate hitsGroundDelegate;

        private bool isGrounded = true;
        public bool IsGrounded
        {
            get
            {
                return isGrounded;
            }
        }

        /// <summary>
        /// Detect collision with floor
        /// </summary>
        /// <param name="hit"></param>
        public void OnCollisionEnter(Collision hit)
        {
            

            /*
            if (hit.gameObject.CompareTag("ground"))
            {
                isGrounded = true;
            }
            */
        }
        public void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("ground"))
            {
                hitsGroundDelegate?.Invoke();
            }

        }
        /// <summary>
        /// Detect collision exit with floor
        /// </summary>
        /// <param name="hit"></param>
        public void OnCollisionExit(Collision hit)
        {
        }
    }

}
