using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealismOverhaul
{
    class Patches
    {
		public static bool SandLevelPrefix(ref float __result)
		{
			__result = Main.GetSecondsElapsed();
			return false;
		}

		public static bool FuelPrefix(ref float ____currentFuel, ref float amount)
		{
			____currentFuel = Mathf.Max(____currentFuel - (amount * Main._fuelDrainSpeed), 0f);
			return false;
		}

		/*
		public static IEnumerable<CodeInstruction> CanBreakTranspile(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
			list.RemoveRange(0, 6);
			return list.AsEnumerable<CodeInstruction>();
		}
		*/
	}
}
