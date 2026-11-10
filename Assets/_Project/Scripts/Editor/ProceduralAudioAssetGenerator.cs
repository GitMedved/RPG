#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Generates original, lightweight PCM sound effects and loopable ambience/music.
    /// No external samples, copyrighted recordings or voice likenesses are used.
    /// </summary>
    public static class ProceduralAudioAssetGenerator
    {
        private const int SampleRate = 22050;
        private const string SfxRoot = "Assets/Resources/MiaMax/Audio/SFX";
        private const string AmbienceRoot = "Assets/Resources/MiaMax/Audio/Ambience";
        private const string MusicRoot = "Assets/Resources/MiaMax/Audio/Music";

        private static readonly string[] Sfx =
        {
            "ui_tap", "plant", "harvest", "craft_start", "craft_ready", "reward",
            "collectible", "interact", "minigame_start", "minigame_win", "minigame_miss"
        };

        [MenuItem("Tools/Mia & Max/Audio/Generate Original Audio Pack")]
        public static void GenerateAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max Audio", "Exit Play Mode before generating audio assets.", "OK");
                return;
            }

            EnsureFolder("Assets/Resources");
            EnsureFolder("Assets/Resources/MiaMax");
            EnsureFolder("Assets/Resources/MiaMax/Audio");
            EnsureFolder(SfxRoot);
            EnsureFolder(AmbienceRoot);
            EnsureFolder(MusicRoot);

            int generated = 0;
            int errors = 0;
            var report = new System.Text.StringBuilder();
            report.AppendLine("MIA & MAX — ORIGINAL PROCEDURAL AUDIO GENERATION");

            foreach (string id in Sfx)
                TryWrite(SfxRoot + "/" + id + ".wav", BuildCue(id, CueDuration(id)), ref generated, ref errors, report);

            TryWrite(AmbienceRoot + "/backyard_wind.wav", BuildWind(10f, 0.13f, 0.7f), ref generated, ref errors, report);
            TryWrite(AmbienceRoot + "/backyard_birds.wav", BuildBirds(10f, false), ref generated, ref errors, report);
            TryWrite(AmbienceRoot + "/woodland_wind.wav", BuildWind(12f, 0.18f, 0.42f), ref generated, ref errors, report);
            TryWrite(AmbienceRoot + "/woodland_birds.wav", BuildBirds(12f, true), ref generated, ref errors, report);

            TryWrite(MusicRoot + "/backyard_theme.wav", BuildTheme(0), ref generated, ref errors, report);
            TryWrite(MusicRoot + "/flower_garden_theme.wav", BuildTheme(1), ref generated, ref errors, report);
            TryWrite(MusicRoot + "/woodland_theme.wav", BuildTheme(2), ref generated, ref errors, report);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            report.AppendLine();
            report.AppendLine("Generated: " + generated + " / 18");
            report.AppendLine("Errors: " + errors);
            report.AppendLine("Original synthesized PCM WAV files; no third-party recordings or voice cloning.");
            report.AppendLine("Sound design is a first implementation and still needs listening review and device mix validation.");
            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max Audio Pack",
                "Generated: " + generated + " / 18\nErrors: " + errors +
                "\n\nAudio is original procedural synthesis. Listen and tune the mix before release; full report is in Console.",
                "OK");
        }

        private static void TryWrite(string path, float[] samples, ref int generated, ref int errors,
            System.Text.StringBuilder report)
        {
            try
            {
                WriteWave(path, samples);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                if (importer != null)
                {
                    importer.forceToMono = true;
                    importer.loadInBackground = true;
                    importer.preloadAudioData = true;
                    AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                    settings.compressionFormat = AudioCompressionFormat.Vorbis;
                    settings.quality = 0.72f;
                    importer.defaultSampleSettings = settings;
                    importer.SaveAndReimport();
                }

                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null) throw new InvalidOperationException("Unity did not import the WAV as an AudioClip.");
                generated++;
                report.AppendLine("[CREATED] " + path + " (" + clip.frequency + " Hz, " +
                    clip.channels + " ch, " + clip.length.ToString("0.00") + " s)");
            }
            catch (Exception exception)
            {
                errors++;
                report.AppendLine("[ERROR] " + path + " — " + exception.Message);
            }
        }

        private static void WriteWave(string path, float[] samples)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                int dataBytes = samples.Length * 2;
                writer.Write(new[] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + dataBytes);
                writer.Write(new[] { 'W', 'A', 'V', 'E' });
                writer.Write(new[] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(SampleRate);
                writer.Write(SampleRate * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(new[] { 'd', 'a', 't', 'a' });
                writer.Write(dataBytes);
                for (int i = 0; i < samples.Length; i++)
                {
                    float value = Mathf.Clamp(samples[i], -0.98f, 0.98f);
                    writer.Write((short)Mathf.RoundToInt(value * 32767f));
                }
            }
        }

        private static float CueDuration(string id)
        {
            switch (id)
            {
                case "harvest": return 0.72f;
                case "craft_start": return 0.52f;
                case "craft_ready": return 0.95f;
                case "reward": return 1.35f;
                case "collectible": return 0.72f;
                case "minigame_start": return 0.82f;
                case "minigame_win": return 1.45f;
                case "minigame_miss": return 0.48f;
                case "plant": return 0.42f;
                case "interact": return 0.18f;
                default: return 0.12f;
            }
        }

        private static float[] BuildCue(string id, float duration)
        {
            int count = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / SampleRate;
                float value = 0f;
                switch (id)
                {
                    case "ui_tap":
                        value = Tone(t, 1120f, 0.045f, 0.20f) + Tone(t, 620f, 0.065f, 0.12f);
                        break;
                    case "interact":
                        value = Tone(t, 480f - 180f * t / duration, 0.11f, 0.28f);
                        break;
                    case "plant":
                        value = Noise(t, 19f, 0.10f) + Tone(t, 170f, 0.22f, 0.12f) +
                            Bell(t, 0.18f, 740f, 0.20f, 0.10f);
                        break;
                    case "harvest":
                        value = Bell(t, 0.02f, 660f, 0.30f, 0.23f) +
                            Bell(t, 0.20f, 880f, 0.34f, 0.18f) +
                            Bell(t, 0.40f, 1046f, 0.28f, 0.12f);
                        break;
                    case "craft_start":
                        value = Tone(t, 180f, 0.14f, 0.24f) +
                            Bell(t, 0.18f, 440f, 0.22f, 0.12f) +
                            Bell(t, 0.32f, 554f, 0.18f, 0.10f);
                        break;
                    case "craft_ready":
                        value = Bell(t, 0.04f, 587f, 0.35f, 0.19f) +
                            Bell(t, 0.28f, 740f, 0.36f, 0.18f) +
                            Bell(t, 0.54f, 880f, 0.36f, 0.17f);
                        break;
                    case "reward":
                        value = Bell(t, 0.02f, 523f, 0.46f, 0.16f) +
                            Bell(t, 0.22f, 659f, 0.48f, 0.15f) +
                            Bell(t, 0.43f, 784f, 0.50f, 0.14f) +
                            Bell(t, 0.66f, 1046f, 0.62f, 0.13f) +
                            Bell(t, 0.90f, 1318f, 0.40f, 0.08f);
                        break;
                    case "collectible":
                        value = Bell(t, 0.02f, 880f, 0.50f, 0.16f) +
                            Bell(t, 0.12f, 1320f, 0.44f, 0.10f) +
                            Bell(t, 0.30f, 1760f, 0.30f, 0.055f);
                        break;
                    case "minigame_start":
                        value = Bell(t, 0.02f, 440f, 0.28f, 0.15f) +
                            Bell(t, 0.22f, 554f, 0.30f, 0.15f) +
                            Bell(t, 0.44f, 659f, 0.34f, 0.14f);
                        break;
                    case "minigame_win":
                        value = Bell(t, 0.02f, 523f, 0.48f, 0.17f) +
                            Bell(t, 0.20f, 659f, 0.50f, 0.16f) +
                            Bell(t, 0.40f, 784f, 0.54f, 0.15f) +
                            Bell(t, 0.68f, 1046f, 0.70f, 0.16f) +
                            Bell(t, 0.92f, 1318f, 0.58f, 0.10f);
                        break;
                    case "minigame_miss":
                        value = Tone(t, 420f - 230f * t / duration, 0.28f, 0.22f) +
                            Tone(t, 210f - 70f * t / duration, 0.30f, 0.08f);
                        break;
                }
                data[i] = value * BoundaryFade(t, duration, 0.012f);
            }
            return Normalize(data, 0.82f);
        }

        private static float[] BuildWind(float duration, float level, float modulation)
        {
            int count = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[count];
            System.Random random = new System.Random(417);
            float low = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / SampleRate;
                float white = (float)(random.NextDouble() * 2.0 - 1.0);
                low += (white - low) * 0.018f;
                float swell = 0.72f + 0.28f * Mathf.Sin(t * Mathf.PI * 2f / duration * modulation);
                data[i] = low * swell * level * BoundaryFade(t, duration, 0.28f);
            }
            return Normalize(data, 0.65f);
        }

        private static float[] BuildBirds(float duration, bool woodland)
        {
            int count = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[count];
            float[] starts = woodland
                ? new[] { 0.65f, 1.05f, 3.3f, 3.62f, 6.25f, 8.7f, 9.05f, 11.1f }
                : new[] { 0.8f, 1.12f, 4.0f, 6.55f, 6.88f, 9.0f };
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / SampleRate;
                float value = 0f;
                for (int chirp = 0; chirp < starts.Length; chirp++)
                {
                    float start = starts[chirp];
                    float local = t - start;
                    if (local < 0f || local > 0.25f) continue;
                    float baseFrequency = woodland ? 1750f : 2250f;
                    value += Chirp(local, baseFrequency + (chirp % 3) * 240f, 600f, 0.22f, 0.10f);
                }
                data[i] = value * BoundaryFade(t, duration, 0.12f);
            }
            return Normalize(data, 0.48f);
        }

        private static float[] BuildTheme(int style)
        {
            const float duration = 8f;
            int count = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[count];
            float[] notes = style == 0
                ? new[] { 261.63f, 329.63f, 392.00f, 523.25f, 392.00f, 329.63f, 293.66f, 392.00f }
                : style == 1
                    ? new[] { 392.00f, 493.88f, 587.33f, 659.25f, 587.33f, 493.88f, 440.00f, 587.33f }
                    : new[] { 196.00f, 246.94f, 293.66f, 392.00f, 293.66f, 246.94f, 220.00f, 293.66f };

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / SampleRate;
                float cycle = t % 4f;
                int noteIndex = Mathf.Clamp(Mathf.FloorToInt(cycle / 0.5f), 0, 7);
                float noteStart = noteIndex * 0.5f;
                float local = cycle - noteStart;
                float frequency = notes[noteIndex];
                float pluck = Mathf.Sin(2f * Mathf.PI * frequency * local) * Mathf.Exp(-local * 3.8f);
                float overtone = Mathf.Sin(2f * Mathf.PI * frequency * 2.01f * local) * Mathf.Exp(-local * 5.2f) * 0.22f;
                float pad = Mathf.Sin(2f * Mathf.PI * (style == 2 ? 98f : 130.8f) * t) * 0.06f;
                float harmonic = Mathf.Sin(2f * Mathf.PI * (style == 2 ? 147f : 196f) * t) * 0.035f;
                data[i] = (pluck * 0.13f + overtone * 0.13f + pad + harmonic) *
                    BoundaryFade(t, duration, 0.10f);
            }
            return Normalize(data, 0.48f);
        }

        private static float Tone(float t, float frequency, float decay, float amplitude)
        {
            if (t < 0f) return 0f;
            return Mathf.Sin(2f * Mathf.PI * frequency * t) * Mathf.Exp(-t / Mathf.Max(0.01f, decay)) * amplitude;
        }

        private static float Bell(float t, float start, float frequency, float duration, float amplitude)
        {
            float local = t - start;
            if (local < 0f || local > duration) return 0f;
            float envelope = Mathf.Sin(Mathf.PI * Mathf.Clamp01(local / duration));
            envelope *= Mathf.Exp(-local * 2.7f);
            float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * local);
            float partial = Mathf.Sin(2f * Mathf.PI * frequency * 2.76f * local) * 0.22f;
            return (fundamental + partial) * envelope * amplitude;
        }

        private static float Chirp(float t, float startFrequency, float sweep, float duration, float amplitude)
        {
            float envelope = Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / duration));
            float phase = 2f * Mathf.PI * (startFrequency * t + 0.5f * sweep * t * t / duration);
            return Mathf.Sin(phase) * envelope * amplitude;
        }

        private static float Noise(float t, float decay, float amplitude)
        {
            int seed = Mathf.FloorToInt(t * SampleRate * 0.5f);
            float pseudo = Mathf.Sin(seed * 12.9898f) * 43758.5453f;
            float noise = (pseudo - Mathf.Floor(pseudo)) * 2f - 1f;
            return noise * Mathf.Exp(-t * decay) * amplitude;
        }

        private static float BoundaryFade(float t, float duration, float edge)
        {
            float fadeIn = Mathf.Clamp01(t / edge);
            float fadeOut = Mathf.Clamp01((duration - t) / edge);
            return Mathf.Min(fadeIn, fadeOut);
        }

        private static float[] Normalize(float[] data, float peak)
        {
            float maximum = 0f;
            for (int i = 0; i < data.Length; i++) maximum = Mathf.Max(maximum, Mathf.Abs(data[i]));
            if (maximum < 0.00001f) return data;
            float scale = peak / maximum;
            for (int i = 0; i < data.Length; i++) data[i] *= scale;
            return data;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string folder = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
#endif
