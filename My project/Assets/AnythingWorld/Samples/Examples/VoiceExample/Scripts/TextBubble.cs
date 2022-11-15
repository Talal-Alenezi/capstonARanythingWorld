using UnityEngine;
using UnityEngine.UI;


namespace AnythingWorld.Examples
{
    public class TextBubble : MonoBehaviour
    {

        private Text _text;
        private Image _image;
        private Image _globeImage;
        private Color _activeColor;
        private Color _inActiveColor;
        private bool _isComputer;
        private RectTransform _rectTransform;

        private const int BUBBLE_SPACING = 90;

        void Awake()
        {
            _text = transform.Find("Text").GetComponent<Text>();
            _image = GetComponent<Image>();
            _activeColor = _image.color;
            _inActiveColor = _activeColor;
            _inActiveColor.a = 0;
            _image.color = _inActiveColor;
            _isComputer = gameObject.name.IndexOf("Computer") == -1 ? false : true;
            _rectTransform = GetComponent<RectTransform>();
            if (_isComputer)
            {
                _globeImage = transform.Find("Globe").GetComponent<Image>();
                _globeImage.color = new Color(1, 1, 1, 0);
            }
        }

        public void ShowText(string newText)
        {
            if (newText == null)
                return;

            if (newText.Length < 1)
                return;

            // Debug.Log("TextBubble : ShowText : " + newText);

            if (!_isComputer & newText.Length > 40)
            {
                newText = newText.Substring(newText.Length - 40);
                var spaceFind = newText.IndexOf(" ");
                newText = newText.Substring(spaceFind);
            }
            _text.text = newText;
            if (_isComputer)
                _text.enabled = false;
        }

        public void AppendText(string newText)
        {
            _text.text += newText;
        }

        public string GetText()
        {
            return _text.text;
        }

        public void MoveUp()
        {
            _image.color = _activeColor;
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y + BUBBLE_SPACING);
            if (_isComputer)
            {
                _text.enabled = true;
                _globeImage.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        public void MoveOut()
        {
            DestroyBubble();
        }

        public void DestroyBubble()
        {
            Destroy(this.gameObject);
        }
    }
}

