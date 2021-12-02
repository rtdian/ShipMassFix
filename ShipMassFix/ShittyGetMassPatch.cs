using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems;
using Sandbox.Game.GUI;

namespace ShipMassFix
{
    [HarmonyPatch]
    internal static class ShittyGetMassPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(MyRemoteControl), "AvoidCollisionsVs2");
            yield return AccessTools.Method(typeof(MyGridJumpDriveSystem), "DepleteJumpDrives");
            yield return AccessTools.Method(typeof(MyGridJumpDriveSystem), "GetMaxJumpDistance");
            yield return AccessTools.Method(typeof(MyStatControlledEntityMass), "Update");
            yield return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalInfoController, Sandbox.Game", true), "UpdateBeforeDraw");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> ins)
        {
            var list = ins.ToList();
            Rewrite(list);
            return list;
        }

        private static void Rewrite(List<CodeInstruction> ins)
        {
            var shittyGetMassMethod = AccessTools.Method(typeof(MyCubeGrid), nameof(MyCubeGrid.GetCurrentMass), Array.Empty<Type>());
            var appendInt32Method = AccessTools.Method(typeof(StringBuilderExtensions_2), nameof(StringBuilderExtensions_2.AppendInt32));

            // Do not use Where instead of FindAll because we're modifying the list
            foreach (var shittyCallIns in ins.FindAll(b => b.Calls(shittyGetMassMethod)))
            {
                var index = ins.IndexOf(shittyCallIns);
                var nextIns = ins[index + 1];

                if (nextIns.opcode == OpCodes.Conv_R4)
                {
                    ins[index] = CodeInstruction.Call(typeof(ShittyGetMassPatch), nameof(GetShipMassFloat)).WithLabels(nextIns.labels);
                    ins[index + 1] = new CodeInstruction(OpCodes.Nop);
                }
                else if (nextIns.opcode == OpCodes.Conv_R8)
                {
                    ins[index] = CodeInstruction.Call(typeof(ShittyGetMassPatch), nameof(GetShipMassDouble)).WithLabels(nextIns.labels);
                    ins[index + 1] = new CodeInstruction(OpCodes.Nop);
                }
                else if (nextIns.Calls(appendInt32Method))
                {
                    ins[index] = new CodeInstruction(OpCodes.Ldc_I4_2);
                    ins[index + 1] = CodeInstruction.Call(typeof(StringBuilderExtensions_2), nameof(StringBuilderExtensions_2.AppendDecimal), new []
                    {
                        typeof(StringBuilder), typeof(float), typeof(int)
                    });
                    // Insert call before ldc_i4_2 and move ldc_i4_2 down
                    ins.Insert(index, CodeInstruction.Call(typeof(ShittyGetMassPatch), nameof(GetShipMassFloat)));
                }
            }
        }

        private static float GetShipMassFloat(MyCubeGrid grid) => grid?.GetCurrentMass(out _, out _) ?? -1;
        private static double GetShipMassDouble(MyCubeGrid grid) => grid?.GetCurrentMass(out _, out _) ?? -1;
    }
}