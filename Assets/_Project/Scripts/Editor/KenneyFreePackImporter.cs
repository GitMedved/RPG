#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Downloads selected official Kenney CC0 packs into the source-model area.
    /// Never modifies scenes, prefabs, or production approval flags.
    /// </summary>
    [InitializeOnLoad]
    public static class KenneyFreePackImporter
    {
        private const string TempRoot = "Library/MiaMaxAssetDownloads/Kenney";
        private const string AssetRoot = "Assets/_Project/Art/Environment/Runtime/Models/Kenney";
        private static UnityWebRequest activeRequest;
        private static UnityWebRequestAsyncOperation activeOperation;
        private static string activePackName;
        private static string activeArchivePath;
        private static string activeDestination;
        private static bool polling;

        [MenuItem("Tools/Mia & Max/Assets/Kenney/Download Nature Kit (CC0)")]
        public static void DownloadNature()
        {
            StartDownload(
                "Nature Kit",
                "https://kenney.nl/media/pages/assets/nature-kit/37ac38a37b-1677698939/kenney_nature-kit.zip",
                "NatureKit");
        }

        [MenuItem("Tools/Mia & Max/Assets/Kenney/Download Furniture Kit (CC0)")]
        public static void DownloadFurniture()
        {
            StartDownload(
                "Furniture Kit",
                "https://kenney.nl/media/pages/assets/furniture-kit/download",
                "FurnitureKit");
        }

        [MenuItem("Tools/Mia & Max/Assets/Kenney/Download Building Kit (CC0)")]
        public static void DownloadBuilding()
        {
            StartDownload(
                "Building Kit",
                "https://kenney.nl/media/pages/assets/building-kit/download",
                "BuildingKit");
        }

        private static void StartDownload(string packName, string url, string folderName)
        {
            if (activeRequest != null)
            {
                EditorUtility.DisplayDialog("Mia & Max Assets", "Another asset download is already running.", "OK");
                return;
            }

            string destination = AssetRoot + "/" + folderName;
            if (Directory.Exists(destination) && Directory.GetFiles(destination, "*", SearchOption.AllDirectories).Length > 0)
            {
                EditorUtility.DisplayDialog(
                    "Mia & Max Assets",
                    packName + " already has imported files. Nothing was overwritten.\n\n" +
                    "Folder: " + destination,
                    "OK");
                return;
            }

            Directory.CreateDirectory(TempRoot);
            string archive = Path.Combine(TempRoot, folderName + ".zip");
            if (File.Exists(archive)) File.Delete(archive);

            activePackName = packName;
            activeArchivePath = archive;
            activeDestination = destination;
            activeRequest = UnityWebRequest.Get(url);
            activeRequest.downloadHandler = new DownloadHandlerFile(archive);
            activeRequest.timeout = 180;
            activeOperation = activeRequest.SendWebRequest();
            EditorApplication.update += Poll;
            polling = true;

            Debug.Log("[Mia & Max Assets] Download started: Kenney " + packName + " (CC0).");
        }

        private static void Poll()
        {
            if (activeRequest == null || activeOperation == null || !activeOperation.isDone) return;

            if (polling)
            {
                EditorApplication.update -= Poll;
                polling = false;
            }

            string error = activeRequest.error;
            bool downloaded = activeRequest.result == UnityWebRequest.Result.Success &&
                              File.Exists(activeArchivePath) &&
                              new FileInfo(activeArchivePath).Length > 1024;
            activeRequest.Dispose();
            activeRequest = null;
            activeOperation = null;

            if (!downloaded)
            {
                CleanupArchive();
                Debug.LogError("[Mia & Max Assets] Kenney " + activePackName + " download failed: " + error);
                EditorUtility.DisplayDialog("Mia & Max Assets", "Download failed for " + activePackName + ".\n\n" + error, "OK");
                ResetState();
                return;
            }

            try
            {
                ExtractSafely(activeArchivePath, activeDestination);
                File.Delete(activeArchivePath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                Debug.Log("[Mia & Max Assets] Kenney " + activePackName + " extracted to " + activeDestination);
                EditorUtility.DisplayDialog(
                    "Mia & Max Assets",
                    "Kenney " + activePackName + " was extracted and sent to Unity import.\n\n" +
                    activeDestination +
                    "\n\nSource models only: no scene objects or prefabs were replaced. Inspect scale, materials, meshes and mobile cost before production use.",
                    "OK");
            }
            catch (Exception exception)
            {
                CleanupArchive();
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Mia & Max Assets",
                    "The archive could not be safely extracted. See Console for details. No existing assets were overwritten.",
                    "OK");
            }

            ResetState();
        }

        private static void ExtractSafely(string archivePath, string destination)
        {
            string fullRoot = Path.GetFullPath(destination);
            Directory.CreateDirectory(fullRoot);
            string rootPrefix = fullRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            using (FileStream file = File.OpenRead(archivePath))
            using (ZipArchive archive = new ZipArchive(file, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count == 0)
                    throw new InvalidDataException("The downloaded file is not a valid asset archive.");

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

        private static void CleanupArchive()
        {
            if (!string.IsNullOrEmpty(activeArchivePath) && File.Exists(activeArchivePath))
                File.Delete(activeArchivePath);
        }

        private static void ResetState()
        {
            activeRequest = null;
            activeOperation = null;
            activePackName = null;
            activeArchivePath = null;
            activeDestination = null;
        }
    }
}
#endif
