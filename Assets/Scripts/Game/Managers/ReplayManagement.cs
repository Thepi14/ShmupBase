using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Main.EntitySystem;
using Main.InputSystem;
using Main.Stages;
using ObjectUtils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Main.ReplaySystem
{
    public static class ReplayManagement
    {
        public static List<Replay> replays;
        public static List<string> replaysFilesPaths;

        public static readonly string REPLAYS_PATH = Application.persistentDataPath + "\\Replays\\", STANDALONE_REPLAYS_PATH = Application.dataPath + "\\Replays\\";

        public static bool replayMode = false;
        public static string replayFilePath = "";

        public const string DEFAULT_REPLAY_EXTENTIOM = ".sbreplay", JSON_REPLAY_EXTENTION = ".sbjreplay";

        public static string GetReplaysPath() => Vars.UseLocalDataPath ? STANDALONE_REPLAYS_PATH : REPLAYS_PATH;

        #region Load Files

        public static long GetHighestScore()
        {
            return replays.Max(replay => replay.highScore);
        }

        public static Replay GetHighestScoreReplay()
        {
            return replays.MaxBy(replay => replay.highScore);
        }

        public static void OrderReplays()
        {
            replays.OrderByDescending(replay => replay.highScore);
        }

        public static Replay LoadReplayFile(string path)
        {
            Replay replay = null;
            if (File.Exists(path))
            {
                if (path.Contains(JSON_REPLAY_EXTENTION))
                {
                    string data = File.ReadAllText(path);
                    replay = JsonUtility.FromJson<Replay>(data);
                }
                else if (path.Contains(DEFAULT_REPLAY_EXTENTIOM))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream stream = new FileStream(GetReplaysPath() + path, FileMode.Open);

                    replay = bf.Deserialize(stream) as Replay;

                    stream.Close();
                }

                if (replay == null)
                    throw new Exception("File " + path + " is not a recognizable replay file.");
            }
            else
                throw new Exception("File " + path + " does not exist.");

                return replay;
        }

        public static Replay[] LoadAllReplayFiles()
        {
            replaysFilesPaths = new List<string>();
            replays = new List<Replay>();
            replaysFilesPaths.AddRange(Directory.GetFiles(GetReplaysPath(), "*" + DEFAULT_REPLAY_EXTENTIOM));
            replaysFilesPaths.AddRange(Directory.GetFiles(GetReplaysPath(), "*" + JSON_REPLAY_EXTENTION));

            foreach (var path in replaysFilesPaths)
            {
                replays.Add(LoadReplayFile(path));
            }

            return replays.ToArray();
        }

        #endregion

        #region Save Files

        public static string GetCompatibleFileNameForReplay(string name)
        {
            name = name.Filter(symbols: false, punctuation: false);
            while (name.EndsWith(' ') || name.EndsWith('.'))
                name = name.Remove(name.Length - 1);
            //Debug.Log("Compatible file name: " + name);
            return name;
        }

        public static string GetReplayJson(Replay replay)
        {
            return JsonUtility.ToJson(replay);
        }

        public static void SaveReplayFile(Replay replay)
        {
            if (Vars.SaveReplaysAsJson)
                SaveReplayFileAsJson(replay);
            else
                SaveReplayFileAsBinary(replay);
        }

        public static void SaveReplayFileAsBinary(Replay replay)
        {
            string fileName = GetCompatibleFileNameForReplay(replay.name);
            var path = fileName;
            int index = 2;

            while (File.Exists(GetReplaysPath() + path + DEFAULT_REPLAY_EXTENTIOM))
            {
                path = fileName + "_" + index;
                index++;
            }
            path += DEFAULT_REPLAY_EXTENTIOM;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(GetReplaysPath() + path, FileMode.Create);

            bf.Serialize(stream, replay);
            stream.Close();
        }

        public static void SaveReplayFileAsJson(Replay replay)
        {
            string fileName = GetCompatibleFileNameForReplay(replay.name);
            var path = fileName;
            int index = 2;

            while (File.Exists(GetReplaysPath() + path + JSON_REPLAY_EXTENTION))
            {
                path = fileName + "_" + index;
                index++;
            }
            path += JSON_REPLAY_EXTENTION;

            var data = GetReplayJson(replay);
            File.WriteAllText(GetReplaysPath() + path, data);
        }

        #endregion
    }

    [Serializable]
    public record Replay
    {
        /// <summary>
        /// Data e horário do início do replay.
        /// </summary>
        public DateTime dateTime;
        public string name;
        /// <summary>
        /// Duração total do replay, em frames, sem contar pausas.
        /// </summary>
        public int framesDuration = 0;
        /// <summary>
        /// Duração total do replay, formatado, sem contar pausas, não é serializado, calculado pela duração em frames.
        /// </summary>
        [DoNotSerialize]
        public TimeSpan duration;
        /// <summary>
        /// Tempo bruto para terminar o replay, contando pausa, sem uso prático.
        /// </summary>
        public DateTime rawEndTime;

        public long highScore = 0;

        /// <summary>
        /// Seed do jogo para basear os números pseudo aleatórios.
        /// </summary>
        public int seed;

        public Difficulty difficulty = Difficulty.None;

        public int lostLifes = 0;

        /// <summary>
        /// O que o player fez frame por frame.
        /// </summary>
        public PlayerInput[] playerInput = new PlayerInput[0];

        public Replay(int seed)
        {
            this.seed = seed;
        }

        public float ReplaySecondsDuration()
        {
            return framesDuration / TimeManager.FIXED_UPDATES_PER_SECOND;
        }

        public int ReplayMinutesDuration()
        {
            return ((int)ReplaySecondsDuration()) / 60;
        }

        public int ReplayHoursDuration()
        {
            return Mathf.FloorToInt(ReplayMinutesDuration() / 60f);
        }

        public TimeSpan ReplayDuration()
        {
            float rawSeconds = ReplaySecondsDuration();
            int milliseconds = (int)((rawSeconds % 1) * 100), seconds = Mathf.FloorToInt(rawSeconds), minutes = ReplayMinutesDuration() % 60, hours = ReplayHoursDuration();
            duration = new TimeSpan(hours / 24, hours, minutes, seconds, milliseconds);
            return duration;
        }
    }
}
