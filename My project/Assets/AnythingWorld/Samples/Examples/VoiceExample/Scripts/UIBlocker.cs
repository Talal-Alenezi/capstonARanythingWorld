using System.Collections;
using UnityEngine;

namespace AnythingWorld.Examples
{
    public class UIBlocker : MonoBehaviour
    {

        private static UIBlocker instance;
        public static UIBlocker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("UIBlocker").AddComponent<UIBlocker>();
                }
                return instance;
            }
        }
        private bool _isBlocked;
        public bool IsBlocked
        {
            get
            {
                _isBlocked = false;
                if (_canvasOneBlocked || _canvasTwoBlocked)
                {
                    _isBlocked = true;
                }

                // Debug.LogWarning("Canvas blocked = " + _isBlocked);

                return _isBlocked;
            }
        }
        private bool _canvasOneBlocked;
        public bool CanvasOneBlocked
        {
            get
            {
                return _canvasOneBlocked;
            }
            set
            {
                // Debug.Log("canvas one blocked " + value);
                _canvasOneBlocked = value;
                StartCoroutine(CanvasOneReturn());
            }
        }
        private bool _canvasTwoBlocked;
        public bool CanvasTwoBlocked
        {
            get
            {
                return _canvasTwoBlocked;
            }
            set
            {
                // Debug.Log("canvas two blocked " + value);
                _canvasTwoBlocked = value;
                StartCoroutine(CanvasTwoReturn());
            }
        }

        private IEnumerator CanvasOneReturn()
        {
            yield return new WaitForSeconds(0.4f);
            _canvasOneBlocked = false;
        }
        private IEnumerator CanvasTwoReturn()
        {
            yield return new WaitForSeconds(0.4f);
            _canvasTwoBlocked = false;
        }

        void Awake()
        {
            instance = this;
        }
    }
}

