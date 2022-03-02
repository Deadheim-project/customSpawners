using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSpawners
{
	[HarmonyPatch]
	class RPC
	{
		public static void RPC_FileSync(long sender, ZPackage pkg)
		{
			if (!ZNet.instance.IsServer()) return;

			ZPackage inventory = new ZPackage();

			if (File.Exists(CustomSpawners.FileDirectory))
			{
				inventory.Write((File.ReadAllText(CustomSpawners.FileDirectory)));
			}	

			ZRoutedRpc.instance.InvokeRoutedRPC(sender, "FileSyncClient", inventory);

		}

		public static void RPC_FileSyncClient(long sender, ZPackage pkg)
		{
			string json = pkg.ReadString();
			CustomSpawners.AddClonedItems(new JsonLoader().GetSpawnAreaConfigs(json));
		}

		[HarmonyPatch(typeof(Game), "Start")]
		public static class GameStart
		{
			public static void Postfix()
			{
				if (ZRoutedRpc.instance == null)
					return;

				ZRoutedRpc.instance.Register<ZPackage>("FileSync", new Action<long, ZPackage>(RPC_FileSync));
				ZRoutedRpc.instance.Register<ZPackage>("FileSyncClient", new Action<long, ZPackage>(RPC_FileSyncClient));
			}
		}

	}
}
