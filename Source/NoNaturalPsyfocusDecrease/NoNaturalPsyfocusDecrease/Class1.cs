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
            //Log.Message("Harmonytest");
        }

        [HarmonyPatch(typeof(Pawn_PsychicEntropyTracker))]
        [HarmonyPatch("PsychicEntropyTrackerTick")]
        public static class HarmonyPatches_FallByTick
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> PsychicEntropyTrackerTick_HarmonyPatch(IEnumerable<CodeInstruction> instructions)
            {
                //Log.Message("HarmonyPatchApplied ChangeNaturalPsyfocusFallRate");
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

                MethodInfo mt_meditating = AccessTools.PropertyGetter(typeof(Pawn_PsychicEntropyTracker), "IsCurrentlyMeditating");

                int pos = codes.FindIndex(c => c.opcode == OpCodes.Call && c.operand != null && c.operand == mt_meditating);
                //Log.Message(pos.ToString());
                codes.RemoveRange(pos - 3, 17);

                /*int pos = codes.FindIndex(c => c.opcode == OpCodes.Ldc_R4 && c.operand != null && (float)c.operand == 0.035f);
                //codes[pos] = new CodeInstruction(OpCodes.Ldc_R4, 0.035f);
                

                //next
                pos = codes.FindIndex(pos + 1, c => c.opcode == OpCodes.Ldc_R4 && c.operand != null && (float)c.operand == 0.055f);
                //codes[pos] = new CodeInstruction(OpCodes.Ldc_R4, 0.035f);

                //next
                pos = codes.FindIndex(pos + 1, c => c.opcode == OpCodes.Ldc_R4 && c.operand != null && (float)c.operand == 0.075f);
                //codes[pos] = new CodeInstruction(OpCodes.Ldc_R4, 0.035f);

                */

                foreach (CodeInstruction code in codes)
                {
                    yield return code;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn_PsychicEntropyTracker))]
        [HarmonyPatch("PsyfocusTipString_NewTemp")]
        public static class HarmonyPatches_Tooltip
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> PsyfocusTipString_NewTemp_HarmonyPatch(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                FieldInfo mt_meditating = AccessTools.DeclaredField(typeof(Pawn_PsychicEntropyTracker), "FallRatePerPsyfocusBand");
                int pos = codes.FindIndex(c => c.opcode == OpCodes.Ldsfld && c.operand != null && c.operand == mt_meditating);
                //Log.Message(pos.ToString());
                codes.RemoveRange(pos - 29, 51);
                foreach (CodeInstruction code in codes)
                {
                    yield return code;
                }

            }
        }
    }
}
