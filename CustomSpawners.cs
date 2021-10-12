using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using System;
using System.IO;
using UnityEngine;

namespace CustomSpawners
{
    [BepInPlugin("Detalhes.CustomSpawners", "CustomSpawners", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class CustomSpawners : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.CustomSpawners";
        Harmony harmony = new Harmony(PluginGUID);
        static Root root = new Root();
        public static bool hasSpawned = false;
        public static readonly string ModPath = Path.GetDirectoryName(typeof(CustomSpawners).Assembly.Location);

        private void Awake()
        {
            root = SimpleJson.SimpleJson.DeserializeObject<Root>(File.ReadAllText(Path.Combine(ModPath, "customSpawners.json")));
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Player), "OnSpawned")]
        internal class OnSpawned
        {
            private static void Postfix(Player __instance)
            {
                hasSpawned = true;
                AddClonedItems();
            }
        }

        public static bool IsObjectDBReady(string name)
        {
            return PrefabManager.Instance.GetPrefab(name) != null;
        }

        public static void AddClonedItems()
        {
            Debug.LogError("xoxota");
            try
            {
                foreach (SpawnAreaConfig areaConfig in root.spawnAreaConfigList)
                {
                    if (!IsObjectDBReady(areaConfig.m_prefabName)) return;

                    GameObject customSpawner = PrefabManager.Instance.CreateClonedPrefab("CS_" + areaConfig.m_prefabName, "BonePileSpawner");
                    SpawnArea area = customSpawner.GetComponent<SpawnArea>();

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

                    foreach (SpawnArea.SpawnData data in area.m_prefabs)
                    {
                        data.m_prefab = PrefabManager.Instance.GetPrefab(areaConfig.m_prefabName);
                    }

                    int stableHashCode = customSpawner.name.GetStableHashCode();
                    ZNetScene.instance.m_prefabs.Add(customSpawner);
                    ZNetScene.instance.m_namedPrefabs.Add(stableHashCode, customSpawner);

                    ObjectDB.instance.m_items.Add(customSpawner);
                    ObjectDB.instance.UpdateItemHashes();
                }

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}
