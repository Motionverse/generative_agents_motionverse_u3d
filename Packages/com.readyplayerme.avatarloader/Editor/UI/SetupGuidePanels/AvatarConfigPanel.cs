using ReadyPlayerMe.Core.Editor;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class AvatarConfigPanel : IEditorWindowComponent
    {
        private const string HEADING = "Avatar Configuration";
        private const string DESCRIPTION =
            "To optimize your game or experience, you have multiple options to select the right Quality/Performance settings. You can select between the presets Low (optimum performance), Medium (best tradeoff) and High (optimum visual quality).";
        private const string INFO_TEXT = "You can change this setting later, or fine-tune with your own created Scriptable Object.";

        public bool IsAvatarConfigFieldEmpty => avatarConfigFields.IsAvatarConfigFieldEmpty;

        private readonly AvatarConfigFields avatarConfigFields;

        public AvatarConfigPanel()
        {
            avatarConfigFields = new AvatarConfigFields();
        }

        public void Draw(Rect position = new Rect())
        {
            HeadingAndDescriptionField.SetDescription(HEADING, DESCRIPTION);

            GUILayout.Space(10);
            Layout.Horizontal(() =>
            {
                avatarConfigFields.DrawAvatarConfig();
                GUILayout.FlexibleSpace();
            });
            GUILayout.Space(10);

            GUILayout.Label(INFO_TEXT, new GUIStyle(GUI.skin.label)
            {
                fixedWidth = 435,
                wordWrap = true,
                margin = new RectOffset(15,15,0,0)
            });
        }
    }
}
