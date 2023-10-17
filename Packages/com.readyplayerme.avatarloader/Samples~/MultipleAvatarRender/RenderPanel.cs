using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.AvatarLoader
{
    public class RenderPanel : MonoBehaviour
    {
        public Text heading;
        public Image image;

        public void SetHeading(string text)
        {
            var headingText = string.Concat(text.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            heading.text = headingText;
        }

        public void SetImage(Texture2D texture)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            image.sprite = sprite;
            image.preserveAspect = true;
        }

    }
}
