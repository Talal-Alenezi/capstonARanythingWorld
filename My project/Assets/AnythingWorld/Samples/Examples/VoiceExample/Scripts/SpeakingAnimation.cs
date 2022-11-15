using System.Collections;
using UnityEngine;

namespace AnythingWorld.Examples
{
    public class SpeakingAnimation : MonoBehaviour
    {

        public Transform[] SpeakingBars;
        private float _moveY;
        private float _origY;
        private float _scaleUp;
        private float _startScale;
        private bool _animateBars;

        void Awake()
        {
            _origY = SpeakingBars[0].position.y;
            _moveY = SpeakingBars[0].position.y + 16;
            _scaleUp = 0.6f;
            _startScale = 0.2f;
            _animateBars = false;
            ResetBars();
            ShowDots(false);
        }

        void ShowDots(bool shouldShow)
        {
            foreach (var dotTrans in SpeakingBars)
            {
                dotTrans.gameObject.SetActive(shouldShow);
            }
        }

        public void StartSpeaking()
        {
            ShowDots(true);
            _animateBars = true;
            StartCoroutine(StartThinkTween());
        }

        public void StopSpeaking()
        {
            _animateBars = false;
            ShowDots(false);
            ResetBars();
        }

        private void ResetBars()
        {
            // _startScale = Random.Range(0.2f,0.3f);
            for (var i = 0; i < SpeakingBars.Length; i++)
            {
                SpeakingBars[i].localScale = new Vector3(1f, _startScale, 1f);
            }
        }

        private IEnumerator StartThinkTween()
        {
            var intervalStep = 0.4f;

            while (_animateBars)
            {
                for (var i = 0; i < SpeakingBars.Length; i++)
                {

                    _scaleUp = Random.Range(0.2f, 1.2f);
                    intervalStep = 1.6f - _scaleUp;

                    var targScale = new Vector3(SpeakingBars[i].localScale.x, _scaleUp, SpeakingBars[i].localScale.z);

                    var diff = Random.Range(0.2f, 0.5f);
                    var j = 0f;
                    while (j < 1)
                    {
                        j += diff;
                        SpeakingBars[i].localScale = Vector3.Lerp(SpeakingBars[i].localScale, targScale, j);
                        yield return new WaitForEndOfFrame();
                    }
                    yield return new WaitForEndOfFrame();
                }

            }
        }

    }
}

