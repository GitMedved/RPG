#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Downloads the verified free Quaternius Stylized Nature MegaKit (standard subset)
    /// from its OpenGameArt mirror and imports the source files into the project.
    /// It does not replace any scene objects or promote assets to production-approved.
    /// </summary>
    [InitializeOnLoad]
    public static class FreeEnvironmentAssetImporter
    {
        private const string PackUrl = "https://opengameart.org/sites/default/files/stylized_nature_megakitstandard.zip";
        private const string DownloadDirectory = "Library/MiaMaxAssetDownloads";
        private const string ZipPath = DownloadDirectory + "/stylized_nature_megakitstandard.zip";
        private const string ImportDirectory = "Assets/_Project/Art/Environment/Runtime/Models/QuaterniusStylizedNature";

        private static UnityWebRequest request;
        private static UnityWebRequestAsyncOperation operation;
        private static bool polling;

        [MenuItem("Tools/Mia & Max/Assets/Download Quaternius Stylized Nature (CC0)")]
        public static void DownloadNaturePack()
        {
            if (request != null)
            {
                EditorUtility.DisplayDialog("Mia & Max Assets", "A download is already running.", "OK");
                return;
            }

            if (Directory.Exists(ImportDirectory) && Directory.GetFiles(ImportDirectory, "*", SearchOption.AllDirectories).Length > 0)
            {
                EditorUtility.DisplayDialog(
                    "Mia & Max Assets",
                    "The nature pack folder already contains files:\n" + ImportDirectory +
                    "\n\nNothing was overwritten. Remove or rename that folder manually if you want to import a fresh copy.",
                    "OK");
                return;
            }

            Directory.CreateDirectory(DownloadDirectory);
            request = UnityWebRequest.Get(PackUrl);
            request.downloadHandler = new DownloadHandlerFile(ZipPath);
            request.timeout = 180;
            operation = request.SendWebRequest();
            EditorApplication.update += PollDownload;
            polling = true;

            Debug.Log("[Mia & Max Assets] Download started: Quaternius Stylized Nature MegaKit (CC0).");
        }

        private static void PollDownload()
        {
            if (request == null || operation == null || !operation.isDone) return;

            if (polling)
            {
                EditorApplication.update -= PollDownload;
                polling = false;
            }

            string error = request.error;
            bool success = request.result == UnityWebRequest.Result.Success && File.Exists(ZipPath) &&
                           new FileInfo(ZipPath).Length > 1024 * 1024;
            request.Dispose();
            request = null;
            operation = null;

            if (!success)
            {
                if (File.Exists(ZipPath)) File.Delete(ZipPath);
                Debug.LogError("[Mia & Max Assets] Nature pack download failed: " + error);
                EditorUtility.DisplayDialog("Mia & Max Assets", "Download failed. Check your internet connection and try again.\n\n" + error, "OK");
                return;
            }

            try
            {
                ExtractSafely(ZipPath, ImportDirectory);
                File.Delete(ZipPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                Debug.Log("[Mia & Max Assets] Nature pack extracted and sent to Unity import: " + ImportDirectory);
                EditorUtility.DisplayDialog(
                    "Mia & Max Assets",
                    "Quaternius Stylized Nature MegaKit files were extracted to:\n" + ImportDirectory +
                    "\n\nUnity is importing the models and their included textures. This is a source-asset import only; prefabs, materials, colliders, LODs and visual approval remain separate tasks.",
                    "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Mia & Max Assets", "The archive could not be safely extracted. See the Console for details.", "OK");
            }
        }

        private static void ExtractSafely(string zipPath, string destination)
        {
            string fullRoot = Path.GetFullPath(destination);
            Directory.CreateDirectory(fullRoot);
            string rootPrefix = fullRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            using (FileStream file = File.OpenRead(zipPath))
            using (ZipArchive archive = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.FullName)) continue;
                    string normalized = entry.FullName.Replace('\\', '/');
                    if (normalized.StartsWith("/", StringComparison.Ordinal) ||
                        Path.IsPathRooted(normalized) ||
                        normalized.Contains("../") ||
                        normalized == "..")
                        throw new InvalidDataException("Unsafe archive entry: " + entry.FullName);

                    string outputPath = Path.GetFullPath(Path.Combine(fullRoot, normalized));
                    if (!outputPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Archive entry escapes destination: " + entry.FullName);

                    if (normalized.EndsWith("/", StringComparison.Ordinal))
                    {
                        Directory.CreateDirectory(outputPath);
                        continue;
                    }

                    string parent = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(parent)) Directory.CreateDirectory(parent);
                    using (Stream input = entry.Open())
                    using (FileStream output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write))
                        input.CopyTo(output);
                }
            }
        }
    }
}
#endif
