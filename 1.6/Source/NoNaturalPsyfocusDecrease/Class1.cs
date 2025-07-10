using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace NoNaturalPsyfocusDecrease
{
    [StaticConstructorOnStartup]
    public class NoNaturalPsyfocusDecrease
    {
        static NoNaturalPsyfocusDecrease()
        {
            Harmony harmony = new Harmony("rimworld.nonaturalpsyfocusdecrease");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(Pawn_PsychicEntropyTracker))]
        [HarmonyPatch("PsychicEntropyTrackerTickInterval")]
        public static class HarmonyPatches_FallByTick
        {

            /* Patch using HarmonyTranspiler
             * for Pawn_PsychicEntropyTracker.PsychicEntropyTrackerTickInterval
             * from 		
            if (this.NeedsPsyfocus && this.pawn.IsHashIntervalTick(150))
			{
				float num = 400f;
				if (!this.IsCurrentlyMeditating)
				{
					this.currentPsyfocus = Mathf.Clamp(this.currentPsyfocus - this.PsyfocusFallPerDay / num, 0f, 1f);
				}
				MeditationUtility.CheckMeditationScheduleTeachOpportunity(this.pawn);
			}
             * into
            if (this.NeedsPsyfocus && this.pawn.IsHashIntervalTick(150))
			{
				MeditationUtility.CheckMeditationScheduleTeachOpportunity(this.pawn);
			}
            */

            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> PsychicEntropyTrackerTickInterval_HarmonyPatch(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo mt_meditating = AccessTools.PropertyGetter(typeof(Pawn_PsychicEntropyTracker), "IsCurrentlyMeditating");
                int pos = codes.FindIndex(c => c.opcode == OpCodes.Call && c.operand != null && c.operand == mt_meditating);
                codes.RemoveRange(pos - 3, 17);

                foreach (CodeInstruction code in codes)
                {
                    yield return code;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn_PsychicEntropyTracker))]
        [HarmonyPatch("PsyfocusTipString")]
        public static class HarmonyPatches_Tooltip
        {

            /* Patch using HarmonyTranspiler
             * for Pawn_PsychicEntropyTracker.PsyfocusTipString
             * from 		
			if (Pawn_PsychicEntropyTracker.psyfocusLevelInfoCached == null)
			{
				for (int i = 0; i < Pawn_PsychicEntropyTracker.PsyfocusBandPercentages.Count - 1; i++)
				{
					Pawn_PsychicEntropyTracker.psyfocusLevelInfoCached += "PsyfocusLevelInfoRange".Translate((Pawn_PsychicEntropyTracker.PsyfocusBandPercentages[i] * 100f).ToStringDecimalIfSmall(), (Pawn_PsychicEntropyTracker.PsyfocusBandPercentages[i + 1] * 100f).ToStringDecimalIfSmall()) + ": " + "PsyfocusLevelInfoPsycasts".Translate(Pawn_PsychicEntropyTracker.MaxAbilityLevelPerPsyfocusBand[i]) + "\n";
				}
				Pawn_PsychicEntropyTracker.psyfocusLevelInfoCached += "\n";
				for (int j = 0; j < Pawn_PsychicEntropyTracker.PsyfocusBandPercentages.Count - 1; j++)
				{
					Pawn_PsychicEntropyTracker.psyfocusLevelInfoCached += "PsyfocusLevelInfoRange".Translate((Pawn_PsychicEntropyTracker.PsyfocusBandPercentages[j] * 100f).ToStringDecimalIfSmall(), (Pawn_PsychicEntropyTracker.PsyfocusBandPercentages[j + 1] * 100f).ToStringDecimalIfSmall()) + ": " + "PsyfocusLevelInfoFallRate".Translate(Pawn_PsychicEntropyTracker.FallRatePerPsyfocusBand[j].ToStringPercent()) + "\n";
				}
			}
             * into
			if (Pawn_PsychicEntropyTracker.psyfocusLevelInfoCached == null)
			{
				for (int i = 0; i < Pawn_PsychicEntropyTracker.PsyfocusBandPercentages.Count - 1; i++)
				{
					Pawn_PsychicEntropyTracker.psyfocusLevelInfoCached += "PsyfocusLevelInfoRange".Translate((Pawn_PsychicEntropyTracker.PsyfocusBandPercentages[i] * 100f).ToStringDecimalIfSmall(), (Pawn_PsychicEntropyTracker.PsyfocusBandPercentages[i + 1] * 100f).ToStringDecimalIfSmall()) + ": " + "PsyfocusLevelInfoPsycasts".Translate(Pawn_PsychicEntropyTracker.MaxAbilityLevelPerPsyfocusBand[i]) + "\n";
				}
			}
            */
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> PsyfocusTipString_HarmonyPatch(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                FieldInfo mt_meditating = AccessTools.DeclaredField(typeof(Pawn_PsychicEntropyTracker), "FallRatePerPsyfocusBand");
                int pos = codes.FindIndex(c => c.opcode == OpCodes.Ldsfld && c.operand != null && c.operand == mt_meditating);
                codes.RemoveRange(pos - 29, 51);
                foreach (CodeInstruction code in codes)
                {
                    yield return code;
                }

            }
        }
    }
}
