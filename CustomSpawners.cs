using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using System.Linq;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CustomSpawners
{
    [BepInPlugin("Detalhes.CustomSpawners", "CustomSpawners", "1.0.0")]
    public class CustomSpawners : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.CustomSpawners";
        Harmony harmony = new Harmony(PluginGUID);
        static Root root = new Root();
        public static bool hasSpawned = false;
        public static readonly string ModPath = Path.GetDirectoryName(typeof(CustomSpawners).Assembly.Location);

        public static ConfigEntry<string> SpawnAreaConfigList;


        private void Awake()
        {

            SpawnAreaConfigList = Config.Bind("Server config", "SpawnAreaConfigList", "prefabToCopy=stone_wall_1x1;m_spawnTimer=30;m_onGroundOnly=false;m_maxTotal=10;m_maxNear=3;m_farRadius=30;m_spawnRadius=10;m_setPatrolSpawnPoint=false;m_triggerDistance=10;m_spawnIntervalSec=10;m_levelupChance=10;m_prefabName=Boar,Neck,Deer;m_nearRadius=10; | prefabToCopy=woodiron_beam;m_spawnTimer=30;m_onGroundOnly=false;m_maxTotal=10;m_maxNear=3;m_farRadius=30;m_spawnRadius=10;m_setPatrolSpawnPoint=false;m_triggerDistance=10;m_spawnIntervalSec=10;m_levelupChance=10;m_prefabName=Greydwarf;m_nearRadius=10;",
                    new ConfigDescription("SpawnAreaConfigList", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            harmony.PatchAll();

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    Jotunn.Logger.LogMessage("Initial Config sync event received");
                    AddClonedItems();
                }
                else
                {
                    Jotunn.Logger.LogMessage("Config sync event received");
                    AddClonedItems();
                }
            };
        }

        public static void AddClonedItems()
        {
            var hammer = ObjectDB.instance.m_items.FirstOrDefault(x => x.name == "Hammer");

            if (!hammer)
            {
                Debug.LogError("Custom Spawners - Hammer could not be loaded"); return;
            }

            PieceTable table = hammer.GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;

            var list = root.GetSpawnAreaConfigs();
            foreach (SpawnAreaConfig areaConfig in list)
            {
                string newName = "CS_" + string.Join("_", areaConfig.m_prefabName);

                if (table.m_pieces.Exists(x => x.name == newName))
                {
                    continue;
                }
                GameObject customSpawner = PrefabManager.Instance.CreateClonedPrefab(newName, areaConfig.name);
                if (customSpawner == null)
                {
                    Debug.LogError("nao achei porrra");
                    continue;
                }

                customSpawner.GetComponent<ZNetView>().m_syncInitialScale = true;

                SpawnArea area = customSpawner.AddComponent<SpawnArea>();

                Piece piece = customSpawner.GetComponent<Piece>();
                if(!piece) customSpawner.AddComponent<Piece>();

                piece.m_description = "Spawner ";
                piece.name = customSpawner.name;

                Object.Destroy(customSpawner.GetComponent<Destructible>());
                Object.Destroy(customSpawner.GetComponent<WearNTear>());

                area.m_spawnTimer = areaConfig.m_spawnTimer;
                area.m_onGroundOnly = areaConfig.m_onGroundOnly;
                area.m_maxTotal = areaConfig.m_maxTotal;
                area.m_maxNear = areaConfig.m_maxNear;
                area.m_farRadius = areaConfig.m_farRadius;
                area.m_spawnRadius = areaConfig.m_spawnRadius;
                area.m_setPatrolSpawnPoint = areaConfig.m_setPatrolSpawnPoint;
                area.m_triggerDistance = areaConfig.m_triggerDistance;
                area.m_spawnIntervalSec = areaConfig.m_spawnIntervalSec;
                area.m_levelupChance = areaConfig.m_levelupChance;
                area.m_nearRadius = areaConfig.m_nearRadius;
                area.m_prefabs = new List<SpawnArea.SpawnData>();

                foreach (string prefab in areaConfig.m_prefabName)
                {
                    var newArea = new SpawnArea.SpawnData();
                    newArea.m_weight = 100 / areaConfig.m_prefabName.Count;
                    newArea.m_minLevel = 1;
                    newArea.m_maxLevel = 3;
                    newArea.m_prefab = PrefabManager.Instance.GetPrefab(prefab);
                    piece.m_description += prefab + " ";
                    if (newArea.m_prefab == null) continue;

                    area.m_prefabs.Add(newArea);
                }

                PieceManager.Instance.RegisterPieceInPieceTable(customSpawner, "Hammer", "Custom Spawner");

                if (!SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    table.m_pieces.Remove(customSpawner);
                }
            }
        }
    }
}
