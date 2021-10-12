using System.Collections.Generic;

namespace CustomSpawners
{
    public class SpawnAreaConfig
    {
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
    }

    public class Root
    {
        public List<SpawnAreaConfig> spawnAreaConfigList { get; set; }
    }
}
