using System.Collections;
using UnityEngine;

namespace AnythingWorld.Examples
{
    public class ThinkingAnimation : MonoBehaviour
    {
        public Transform[] ThinkingDots;
        private float _moveY;
        private float _origY;
        private float _scaleUp;
        private RectTransform[] _rectTransforms;
        private GameObject _stopButton;
        private bool _animateBars;

        void Awake()
        {
            _rectTransforms = new RectTransform[ThinkingDots.Length];
            for (var i = 0; i < ThinkingDots.Length; i++)
            {
                _rectTransforms[i] = ThinkingDots[i].GetComponent<RectTransform>();
            }

            _origY = _rectTransforms[0].anchoredPosition.y;
            _moveY = _rectTransforms[0].anchoredPosition.y + 16;
            _stopButton = transform.Find("ThinkStopButton").gameObject;
            _scaleUp = 0.06f;
            _animateBars = false;
            ShowDots(false);
        }

        void ShowDots(bool shouldShow)
        {
            foreach (var dotTrans in ThinkingDots)
            {
                dotTrans.gameObject.SetActive(shouldShow);
            }
            _stopButton.SetActive(shouldShow);
        }

        public void StartThinking()
        {
            _animateBars = true;
            ShowDots(true);
            StartThinkTween();
        }

        public void StopThinking()
        {
            _animateBars = false;
            ShowDots(false);
        }

        private IEnumerator StartThinkTween()
        {
            var intervalStep = 0.4f;

            while (_animateBars)
            {
                for (var i = 0; i < ThinkingDots.Length; i++)
                {

                    _scaleUp = Random.Range(0.2f, 1.2f);
                    intervalStep = 1.6f - _scaleUp;

                    var targScale = new Vector3(ThinkingDots[i].localScale.x, _scaleUp, ThinkingDots[i].localScale.z);

                    var diff = Random.Range(0.2f, 0.5f);
                    var j = 0f;
                    while (j < 1)
                    {
                        j += diff;
                        ThinkingDots[i].localScale = Vector3.Lerp(ThinkingDots[i].localScale, targScale, j);
                        yield return new WaitForEndOfFrame();
                    }
                }

            }
        }

    }

}
