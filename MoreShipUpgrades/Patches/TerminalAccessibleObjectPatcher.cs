﻿using HarmonyLib;
using MoreShipUpgrades.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace MoreShipUpgrades.Patches
{
    [HarmonyPatch(typeof(TerminalAccessibleObject))]
    internal class TerminalAccessibleObjectPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("CallFunctionFromTerminal")]
        private static bool DestroyObject(ref TerminalAccessibleObject __instance, ref float ___currentCooldownTimer, ref bool ___inCooldown)
        {
            if(!UpgradeBus.instance.DestroyTraps || __instance.gameObject.layer != LayerMask.NameToLayer("MapHazards")) { return true; }
            if (Plugin.cfg.DESTROY_TRAP)
            {
                if (Plugin.cfg.EXPLODE_TRAP) { Landmine.SpawnExplosion(__instance.transform.position, true); }
                UpgradeBus.instance.trapHandler.ReqDestroyObjectServerRpc(new NetworkObjectReference(__instance.gameObject.transform.parent.gameObject.GetComponent<NetworkObject>()));
                return false;
            }
            if (!___inCooldown) { ___currentCooldownTimer = Plugin.cfg.DISARM_TIME; }
            return true;
        }
    }
}