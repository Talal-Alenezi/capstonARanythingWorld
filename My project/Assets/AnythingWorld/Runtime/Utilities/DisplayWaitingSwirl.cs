using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using System.Collections;


namespace AnythingWorld.Utilities
{
    [ExecuteAlways]
    public class DisplayWaitingSwirl : MonoBehaviour
    {
        protected float maskAmount = 0;
        protected Image loadingImage;
        public bool StartOnEnable = false;
        private float waitTime = 2f;
        private float maskIncrement;
        private Coroutine swirlCoroutine;
#if UNITY_EDITOR
        private EditorCoroutine editorSwirlCoroutine;
#endif
        private bool isShowing;
        public float WaitTime
        {
            get
            {
                return waitTime;
            }
        }
        private bool _shouldSwirl;

        private Image[] _postWaitImages;

        void Awake()
        {
            loadingImage = GetComponent<Image>();
            maskAmount = 0;
            isShowing = false;
        }

        public void StartSwirly(bool shouldStart)
        {
            _shouldSwirl = shouldStart;
            maskIncrement = 0.01f;
            if (shouldStart)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    editorSwirlCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(UpdateSwirly());
#endif
                }
                else
                {
                    swirlCoroutine = StartCoroutine(UpdateSwirly());
                }
            }
            isShowing = shouldStart;
        }

        void OnDisable()
        {
            SetSwirlyMask();
        }


        private IEnumerator UpdateSwirly()
        {
            while (isShowing)
            {
                if (maskAmount >= 1f)
                {
                    maskIncrement = -0.01f;
                }
                else if (maskAmount <= 0f)
                {
                    maskIncrement = 0.01f;
                }
                maskAmount += maskIncrement;
                SetSwirlyMask();

                if (Application.isEditor && !Application.isPlaying)
                {
#if UNITY_EDITOR
                    yield return new EditorWaitForSeconds(0.05f);
#endif
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }

        }

        protected void SetSwirlyMask()
        {
            loadingImage.fillAmount = maskAmount;
            transform.Rotate(new Vector3(0, 0, 2f));
        }
    }

}
