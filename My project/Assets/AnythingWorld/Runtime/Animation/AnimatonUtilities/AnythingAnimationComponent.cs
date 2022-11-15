using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Base class for all the subscripts making up a single animation
    /// </summary>
    public abstract class AnythingAnimationComponent : MonoBehaviour
    {
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// Should be overridden in each subscript.
        /// </summary>
        /// <param name="parameters"></param>
        public abstract void ModifyParameter(Parameter parameters);

        /// <summary>
        /// Method used for reseting position of the body part, to which the script is being attached, to the original state.
        /// </summary>
        public abstract void ResetState();
    }

}
