
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.Effects
{
	using Voltage.Common.Logging;

	using Voltage.Story.Effects;

	using Voltage.Witches.Models;



    public class WitchesEffectResolver : IEffectResolver
    {
		public ILogger Logger { get; private set; }

		public Player Player { get; private set; }
		public Dictionary<string,string> NPCMap { get; private set; }


		public WitchesEffectResolver(Player player, IEnumerable<NPCModel> npcs, ILogger logger)
		{
			Player = player;
			NPCMap = CreateInitialsMap (npcs);

			Logger = logger;
		}

		private Dictionary<string,string> CreateInitialsMap (IEnumerable<NPCModel> npcs)
		{
			Dictionary<string,string> map = new Dictionary<string,string>();

			foreach (NPCModel npc in npcs)
			{
				map.Add (npc.Initial, npc.ID);
			}

			return map;
		}


		public void Resolve(IDictionary<string,int> effects)
		{
			if(effects != null && Player != null)
			{
				foreach(KeyValuePair<string,int> effect in effects)
				{
					string id = effect.Key;
					int value = Math.Abs(effect.Value);

					if(NPCMap.ContainsKey(id))
					{
						Player.AddAffinity(NPCMap[id], value);
						Player.TrackCurrentSceneAffectedCharacters(NPCMap[id], value);
					}
					else
					{
						Logger.Log (string.Format("WitchesEffectResolver::Resolve >>> Could not resolve effect '{0}'", id), LogLevel.WARNING);
					}
				}
			}
		}


    }
    
}




