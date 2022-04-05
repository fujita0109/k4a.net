using UnityEngine;
using UnityEngine.UI;

namespace SGSample
{
    public class ArmRenderer : MonoBehaviour
    {
        [SerializeField]
        private Image armImage = null;

        [SerializeField]
        private bool isLeft = false;

        private void Start()
        {
            var y = Screen.height * 0.1f;
            var x = (Screen.width * 0.5f) * 0.4f;

            if (isLeft)
            {
                x = -x;
            }

            armImage.rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
}