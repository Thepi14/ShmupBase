using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Main.InputSystem;
using ObjectUtils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Main.ReplaySystem
{
    public static class ReplayManagement
    {
        public static List<Replay> replays;
        public static List<string> replaysPaths;

        public static readonly string REPLAYS_PATH = Application.persistentDataPath + "\\Replays\\", STANDALONE_REPLAYS_PATH = Application.dataPath + "\\Replays\\";

        public static bool replayMode = false;
        public static string replayFileName = "";

        public const string DEFAULT_REPLAY_EXTENTIOM = ".csreplay", JSON_REPLAY_EXTENTION = ".jreplay";

        public static string GetReplaysPath() => Vars.UseLocalDataPath ? STANDALONE_REPLAYS_PATH : REPLAYS_PATH;

        #region Load Files

        public static int GetHighestScore()
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
            replaysPaths = new List<string>();
            replaysPaths.AddRange(Directory.GetFiles(GetReplaysPath(), "*" + DEFAULT_REPLAY_EXTENTIOM));
            replaysPaths.AddRange(Directory.GetFiles(GetReplaysPath(), "*" + JSON_REPLAY_EXTENTION));

            List<Replay> replays = new List<Replay>();

            foreach (var path in replaysPaths)
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
            Debug.Log("Compatible file name: " + name);
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

            var data = replay.SerializeReplay();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(GetReplaysPath() + path, FileMode.Create);

            bf.Serialize(stream, data);
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
        public string name;

        public int highScore = 0;
        public int startLifes = Vars.STARTING_LIFES;
        public int startBombs = Vars.STARTING_BOMBS;

        public int lostLifes = 0;
        public int usedBombs = 0;

        /// <summary>
        /// Seed do jogo para basear os números pseudo aleatórios.
        /// </summary>
        public int seed;

        public int framesDuration = 0;
        public Difficulty difficulty = Difficulty.None;

        /// <summary>
        /// O que o player fez frame por frame.
        /// </summary>
        public PlayerInput[] playerInput = new PlayerInput[0];

        public Replay(int seed)
        {
            this.seed = seed;
        }

        public Replay SerializeReplay()
        {
            for (int i = 0; i < playerInput.Length; i++)
            {
                playerInput[i] = playerInput[i].ConvertSerializable();
            }
            return this;
        }
    }

    [Serializable]
    public enum Difficulty : byte
    {
        None,
        Easy,
        Normal,
        Hard,
        Lunatic,
        Extra,
    }
}
