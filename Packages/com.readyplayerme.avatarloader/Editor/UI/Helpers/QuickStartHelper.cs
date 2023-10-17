using System.Linq;
using System.Threading;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;

namespace ReadyPlayerMe.AvatarLoader.Editor
{
    public class QuickStartHelper
    {
        private const string LOADER_PACKAGE = "com.readyplayerme.avatarloader";
        private const string QUICKSTART_SAMPLE_NAME = "QuickStart";

        public bool Open()
        {
            var quickStartSample = GetQuickStartSample();
            if (quickStartSample == null)
            {
                return false;
            }

            var sample = (Sample) quickStartSample;
            ImportAndOpenSample(sample);

            return true;
        }

        private Sample? GetQuickStartSample()
        {
            var samples = Sample.FindByPackage(LOADER_PACKAGE, null).ToArray();
            if (samples.Length == 0)
            {
                return null;
            }
            
            var quickStartSample = samples.First(x => x.displayName == QUICKSTART_SAMPLE_NAME);
            return quickStartSample;
        }

        private void ImportAndOpenSample(Sample quickStartSample)
        {
            if (!quickStartSample.isImported)
            {
                quickStartSample.Import();
                while (!quickStartSample.isImported)
                    Thread.Sleep(1);
            }

            OpenQuickStartScene(quickStartSample.importPath);
        }

        private void OpenQuickStartScene(string importPath)
        {
            EditorSceneManager.OpenScene($"{importPath}/{QUICKSTART_SAMPLE_NAME}.unity");
        }
    }
}
