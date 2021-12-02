using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems;
using Sandbox.Game.Gui;

namespace ShipMassFix
{
    [HarmonyPatch]
    internal static class HudMassDisplayPatch
    {
        public static float CurrentMass;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyShipController), "UpdateShipMass")]
        private static bool UpdateMassPrefix(MyShipController __instance)
        {
            var grid = __instance.Parent as MyCubeGrid;
            if (grid != null)
                CurrentMass = grid.GetCurrentMass(out _, out _);
            return false;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MyGridJumpDriveSystem), "GetConfirmationText")]
        private static IEnumerable<CodeInstruction> ToStringTranspiler(IEnumerable<CodeInstruction> ins)
        {
            var shipInfoGetter = AccessTools.PropertyGetter(typeof(MyHud), nameof(MyHud.ShipInfo));
            var list = ins.ToList();

            var shittyCallIndex = list.FindIndex(b => b.Calls(shipInfoGetter));
            list[shittyCallIndex] = new CodeInstruction(OpCodes.Ldarg_0);
            list[shittyCallIndex + 1] = new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(MyUpdateableGridSystem), "Grid"));
            list[shittyCallIndex + 2] = CodeInstruction.Call(typeof(HudMassDisplayPatch), nameof(GridMassToStringHelper));
            list[shittyCallIndex + 3] = new CodeInstruction(OpCodes.Nop);
            list[shittyCallIndex + 4] = new CodeInstruction(OpCodes.Nop);
            list[shittyCallIndex + 5] = new CodeInstruction(OpCodes.Nop);

            return list;
        }

        private static string GridMassToStringHelper(MyCubeGrid grid)
        {
            return grid.Mass.ToString("N");
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MyHudShipInfo), "Refresh")]
        private static IEnumerable<CodeInstruction> RefreshTranspiler(IEnumerable<CodeInstruction> ins)
        {
            var shittyMassGetter = AccessTools.PropertyGetter(typeof(MyHudShipInfo), nameof(MyHudShipInfo.Mass));
            var appendInt32Method = AccessTools.Method(typeof(StringBuilderExtensions_2), nameof(StringBuilderExtensions_2.AppendInt32));
            var list = ins.ToList();

            var index = list.FindIndex(b => b.Calls(shittyMassGetter) && list[list.IndexOf(b) + 1].Calls(appendInt32Method));
            list[index - 1] = CodeInstruction.LoadField(typeof(HudMassDisplayPatch), nameof(CurrentMass));
            list[index] = new CodeInstruction(OpCodes.Ldc_I4_2);
            list[index + 1] = CodeInstruction.Call(typeof(StringBuilderExtensions_2), nameof(StringBuilderExtensions_2.AppendDecimal), new []
            {
                typeof(StringBuilder), typeof(float), typeof(int)
            });
            return list;
        }
    }
}