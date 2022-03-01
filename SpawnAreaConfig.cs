using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CustomSpawners
{
    public class JsonLoader
    {
        public List<Spawner> GetSpawnAreaConfigs()
        {
            Root items = SimpleJson.SimpleJson.DeserializeObject<Root>(File.ReadAllText(CustomSpawners.FileDirectory));
            return items.spawners;
        }

        public List<Spawner> GetSpawnAreaConfigs(string json)
        {
            Root items = SimpleJson.SimpleJson.DeserializeObject<Root>(json);

            return items.spawners;

        }
    }

    public class Spawner
    {
        public string name { get; set; }
        public string prefabToCopy { get; set; }
        public int m_spawnTimer { get; set; }
        public bool m_onGroundOnly { get; set; }
        public int m_maxTotal { get; set; }
        public int m_maxNear { get; set; }
        public int m_farRadius { get; set; }
        public int m_spawnRadius { get; set; }
        public bool m_setPatrolSpawnPoint { get; set; }
        public int m_triggerDistance { get; set; }
        public int m_spawnIntervalSec { get; set; }
        public int m_levelupChance { get; set; }
        public string m_prefabName { get; set; }
        public int m_nearRadius { get; set; }
        public int minLevel { get; set; }
        public int maxLevel { get; set; }
    }

    public class Root
    {
        public List<Spawner> spawners { get; set; }
    }


}
