using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealismOverhaul
{
    public class Main : ModBehaviour
    {
		private static float _LoopLength = 22f;

		private static float _debugTimeScale = 1f;

		private bool _destroyBrittleHollow;

		private bool _debugControls;

		private bool _isStarted;

		private static float _debugTimeOffset;

		public static float _fuelDrainSpeed = 10f;

		private void Start()
		{
			base.ModHelper.Console.WriteLine("[In StopTime] :");
			base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
			IModEvents events = base.ModHelper.Events;
			events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));
			GlobalMessenger.AddListener("LearnLaunchCodes", new Callback(this.SaveGame));
			base.ModHelper.Console.WriteLine(": Disabling statue...");
			base.ModHelper.HarmonyHelper.EmptyMethod<MemoryUplinkTrigger>("OnTriggerEnter");

			base.ModHelper.Console.WriteLine(": Disabling interloper destruction...");
			base.ModHelper.HarmonyHelper.EmptyMethod<TempCometCollisionFix>("Update");

			base.ModHelper.Console.WriteLine(": Disabling starfield updates...");
			base.ModHelper.HarmonyHelper.EmptyMethod<StarfieldController>("Update");

			base.ModHelper.Console.WriteLine(": Disabling sun expansion...");
			base.ModHelper.HarmonyHelper.EmptyMethod<SunController>("UpdateScale");

			base.ModHelper.Console.WriteLine(": Disabling sun logic...");
			base.ModHelper.HarmonyHelper.EmptyMethod<SunController>("Update");

			base.ModHelper.Console.WriteLine(": Disabling sun collapse SFX...");
			base.ModHelper.HarmonyHelper.EmptyMethod<SunController>("OnTriggerSupernova");

			base.ModHelper.Console.WriteLine(": Disabling End Times music...");
			base.ModHelper.HarmonyHelper.EmptyMethod<GlobalMusicController>("UpdateEndTimesMusic");

			base.ModHelper.Console.WriteLine(": Patching GetSecondsElapsed...");
			base.ModHelper.HarmonyHelper.AddPrefix<TimeLoop>("GetSecondsElapsed", typeof(Patches), "SandLevelPrefix");

			base.ModHelper.Console.WriteLine(": Patching ShipResources...");
			base.ModHelper.HarmonyHelper.AddPrefix<ShipResources>("DrainFuel", typeof(Patches), "FuelPrefix");

			/*
			bool destroyBrittleHollow = this._destroyBrittleHollow;
			if (destroyBrittleHollow)
			{
				bool printDebug9 = this._printDebug;
				if (printDebug9)
				{
					base.ModHelper.Console.WriteLine(": Patching CanBreak...");
				}
				base.ModHelper.HarmonyHelper.Transpile<FragmentIntegrity>("CanBreak", typeof(Patches), "CanBreakTranspile");
			}
			*/
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			bool flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
			bool flag2 = flag;
			if (flag2)
			{
				this.SaveGame();
				base.ModHelper.Console.WriteLine(": Starting time loop...");
				TimeLoop.SetTimeLoopEnabled(true);

				base.ModHelper.Console.WriteLine(": Setting isTimeFlowing to false...");
				typeof(TimeLoop).GetAnyField("_isTimeFlowing").SetValue(null, false);

				base.ModHelper.Console.WriteLine(string.Format(": Sand-loop timescale set to {0}x", _debugTimeScale));
				base.ModHelper.Console.WriteLine(string.Format(": Sand-loop length set to {0} minutes.", _LoopLength));
				this._isStarted = true;
			}
		}

		private void SaveGame()
		{
			bool flag = PlayerData.KnowsLaunchCodes() && PlayerData.LoadLoopCount() == 1;
			if (flag)
			{
				PlayerData.SaveLoopCount(5);
				PlayerData.SaveCurrentGame();
			}
		}

		private void Update()
		{
			bool flag = this._isStarted && this._debugControls;
			if (flag)
			{
				bool keyDown = global::Input.GetKeyDown(KeyCode.Keypad4);
				if (keyDown)
				{
					_debugTimeOffset -= 60f;
				}
				bool keyDown2 = global::Input.GetKeyDown(KeyCode.Keypad5);
				if (keyDown2)
				{
					_debugTimeOffset = 0f;
				}
				bool keyDown3 = global::Input.GetKeyDown(KeyCode.Keypad6);
				if (keyDown3)
				{
					_debugTimeOffset += 60f;
				}
			}
		}

		public static float GetSecondsElapsed()
		{
			return _LoopLength * 60f - 2f * Math.Abs(Time.timeSinceLevelLoad * (_debugTimeScale - 0.5f) % (_LoopLength * 60f) - _LoopLength * 30f) + _debugTimeOffset;
		}

		public override void Configure(IModConfig config)
		{
			_debugTimeScale = config.GetSettingsValue<float>("sandDebugTimescale");
			//_destroyBrittleHollow = config.GetSettingsValue<bool>("destoyBrittleHollow");
			_debugControls = config.GetSettingsValue<bool>("debugControls");
		}
	}
}
