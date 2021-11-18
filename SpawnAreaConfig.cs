using System.Collections.Generic;
using System.Linq;

namespace CustomSpawners
{
    public class SpawnAreaConfig
    {
        public string name { get; set; }
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
        public List<string> m_prefabName { get; set; }
        public int m_nearRadius { get; set; }
    }

    public class Root
    {       
        public List<SpawnAreaConfig> GetSpawnAreaConfigs()
        {
            List<SpawnAreaConfig> SpawnAreaConfigList = new List<SpawnAreaConfig>();
            foreach (string str in CustomSpawners.SpawnAreaConfigList.Value.Trim(' ').Split('|'))
            {
                var areaInfo = str.Split(';');
                var cfg = new SpawnAreaConfig();
                cfg.name = areaInfo[0].Split('=')[1];
                cfg.m_spawnTimer = System.Convert.ToInt32(areaInfo[1].Split('=')[1]);
                cfg.m_onGroundOnly = bool.Parse(areaInfo[2].Split('=')[1]);
                cfg.m_maxTotal = System.Convert.ToInt32(areaInfo[3].Split('=')[1]);
                cfg.m_maxNear   = System.Convert.ToInt32(areaInfo[4].Split('=')[1]);
                cfg.m_farRadius  = System.Convert.ToInt32(areaInfo[5].Split('=')[1]);
                cfg.m_spawnRadius = System.Convert.ToInt32(areaInfo[6].Split('=')[1]);
                cfg.m_setPatrolSpawnPoint = bool.Parse(areaInfo[7].Split('=')[1]);
                cfg.m_triggerDistance = System.Convert.ToInt32(areaInfo[8].Split('=')[1]);
                cfg.m_spawnIntervalSec = System.Convert.ToInt32(areaInfo[9].Split('=')[1]);
                cfg.m_levelupChance = System.Convert.ToInt32(areaInfo[10].Split('=')[1]);
                cfg.m_prefabName = areaInfo[11].Split('=')[1].Split(',').ToList();
                cfg.m_nearRadius = System.Convert.ToInt32(areaInfo[12].Split('=')[1]);

                SpawnAreaConfigList.Add(cfg);
            }

            return SpawnAreaConfigList;
        }
    }
}
