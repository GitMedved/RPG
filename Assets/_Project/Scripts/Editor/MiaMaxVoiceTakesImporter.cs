#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Copies the six original HeyGen character takes generated for this game
    /// into the exact Unity Resources paths consumed by the runtime.
    /// Explicit menu action only; no network request during a player's offline game.
    /// The source links may eventually expire and should be replaced by committed WAVs.
    /// </summary>
    public static class MiaMaxVoiceTakesImporter
    {
        private const string VoiceFolder = "Assets/Resources/MiaMax/Audio/Voice";
        private static bool downloading;
        private static readonly (string name, string url)[] Takes =
        {
            ("mia_birdhouse_ready_01",
                "https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/95d52e056b7b495dab8f090b44fa2ec4/id=771fc9b2099e4af69fae46d559bcfdd8.wav"),
            ("max_birdhouse_01",
                "https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/e5f0084d0ddd4164a12a38a83457446c/id=8cf06089158e43dc9d95037519437a82.wav"),
            ("mia_secret_garden_01",
                "https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/95d52e056b7b495dab8f090b44fa2ec4/id=c6e53b2ba3744687aaadee2f2c090c75.wav"),
            ("max_garden_discovery_01",
                "https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/e5f0084d0ddd4164a12a38a83457446c/id=56823dc5d1fe4dd793c6017319dd29d2.wav"),
            ("mia_greeting_01",
                "https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/95d52e056b7b495dab8f090b44fa2ec4/id=90d14b30a3244a8780f8923a7ece2d8f.wav"),
            ("max_treasure_win_01",
                "https://resource2.heygen.ai/text_to_speech/6a44461d68cc4a39ba6f4d5245aed20a/e5f0084d0ddd4164a12a38a83457446c/id=efdb25b8d002485f90c7e60cf97ad1f8.wav")
        };

        [MenuItem("Mia & Max/MVP/4. Install Generated English Voice Takes")]
        private static async void Install()
        {
            if (downloading) return;
            bool confirmed = EditorUtility.DisplayDialog("Install Mia & Max Voice Takes",
                "Download six HeyGen English character WAVs into the project now? The game remains offline afterward. These generated files must be committed if you want them in Git.",
                "Download Voice Takes", "Cancel");
            if (!confirmed) return;
            downloading = true;
            int ok = 0;
            int failed = 0;
            try
            {
                Directory.CreateDirectory(VoiceFolder);
                foreach (var take in Takes)
                {
                    string path = VoiceFolder + "/" + take.name + ".wav";
                    if (File.Exists(path) && new FileInfo(path).Length > 1000)
                    {
                        ok++;
                        continue;
                    }
                    try
                    {
                        using (UnityWebRequest request = UnityWebRequest.Get(take.url))
                        {
                            request.timeout = 45;
                            UnityWebRequestAsyncOperation op = request.SendWebRequest();
                            while (!op.isDone) await Task.Yield();
                            if (request.result != UnityWebRequest.Result.Success)
                                throw new IOException(request.error);
                            byte[] data = request.downloadHandler.data;
                            if (data == null || data.Length < 4096 ||
                                data[0] != (byte)'R' || data[1] != (byte)'I' ||
                                data[2] != (byte)'F' || data[3] != (byte)'F')
                                throw new InvalidDataException("Not a WAV file or download incomplete.");
                            File.WriteAllBytes(path, data);
                            ok++;
                            Debug.Log("[Mia & Max Voice] Imported: " + path);
                        }
                    }
                    catch (Exception e)
                    {
                        failed++;
                        Debug.LogError("[Mia & Max Voice] Failed " + take.name + ": " + e.Message);
                    }
                }
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Mia & Max Voice Import",
                    "Ready: " + ok + "/6, failed: " + failed +
                    ". Voice clips are stored in the Unity project. Check Console and audio playback in Play Mode.",
                    "OK");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally { downloading = false; }
        }

        [MenuItem("Mia & Max/MVP/4. Install Generated English Voice Takes", true)]
        private static bool CanInstall() => !EditorApplication.isPlaying && !downloading;
    }
}
#endif
