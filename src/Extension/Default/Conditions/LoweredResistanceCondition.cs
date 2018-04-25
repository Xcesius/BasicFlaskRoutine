﻿using ImGuiNET;
using PoeHUD.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeRoutine.Menu;
using TreeSharp;

namespace TreeRoutine.Routine.BuildYourOwnRoutine.Extension.Default.Conditions
{
    internal class LoweredResistanceCondition : ExtensionCondition
    {
        private static Dictionary<String, Tuple<GameStat, GameStat>> resistanceTypes = new Dictionary<String, Tuple<GameStat, GameStat>>()
        {
            { "Cold", Tuple.Create(GameStat.ColdDamageResistancePct, GameStat.MaximumColdDamageResistancePct) },
            { "Fire", Tuple.Create(GameStat.FireDamageResistancePct, GameStat.MaximumFireDamageResistancePct) },
            { "Lightning",Tuple.Create(GameStat.LightningDamageResistancePct, GameStat.LightningDamageResistancePct) },
            { "Chaos", Tuple.Create(GameStat.ChaosDamageResistancePct, GameStat.MaximumChaosDamageResistancePct) }
        };

        private Boolean CheckCold { get; set; }
        private readonly String CheckColdString = "CheckCold";

        private Boolean CheckFire { get; set; }
        private readonly String CheckFireString = "CheckFire";

        private Boolean CheckLightning { get; set; }
        private readonly String CheckLightningString = "CheckLightning";

        private Boolean CheckChaos { get; set; }
        private readonly String CheckChaosString = "CheckChaos";

        private int ResistanceThreshold { get; set; }
        private readonly String ResistanceThresholdString = "ResistanceThreshold";

        public LoweredResistanceCondition(string owner, string name) : base(owner, name)
        {
            CheckCold = false;
            CheckFire = false;
            CheckLightning = false;
            CheckChaos = false;
        }

        public override void Initialise(Dictionary<String, Object> Parameters)
        {
            base.Initialise(Parameters);

            CheckCold = ExtensionComponent.InitialiseParameterBoolean(CheckColdString, CheckCold, ref Parameters);
            CheckFire = ExtensionComponent.InitialiseParameterBoolean(CheckFireString, CheckFire, ref Parameters);
            CheckLightning = ExtensionComponent.InitialiseParameterBoolean(CheckLightningString, CheckLightning, ref Parameters);
            CheckChaos = ExtensionComponent.InitialiseParameterBoolean(CheckChaosString, CheckChaos, ref Parameters);
            ResistanceThreshold = ExtensionComponent.InitialiseParameterInt32(ResistanceThresholdString, ResistanceThreshold, ref Parameters);
        }

        public override bool CreateConfigurationMenu(ExtensionParameter extensionParameter, ref Dictionary<String, Object> Parameters)
        {
            ImGui.TextDisabled("Condition Info");
            ImGuiExtension.ToolTip("This condition will return true if any of the selected player's resistances\nare reduced by more than or equal to the specified amount.\nReduced max resistance modifiers are taken into effect automatically (e.g. -res map mods).");


            base.CreateConfigurationMenu(extensionParameter, ref Parameters);

            CheckCold = ImGuiExtension.Checkbox("Cold", CheckCold);
            Parameters[CheckColdString] = CheckCold.ToString();

            CheckFire = ImGuiExtension.Checkbox("Fire", CheckFire);
            Parameters[CheckFireString] = CheckFire.ToString();

            CheckLightning = ImGuiExtension.Checkbox("Lightning", CheckLightning);
            Parameters[CheckLightningString] = CheckLightning.ToString();

            CheckChaos = ImGuiExtension.Checkbox("Chaos", CheckChaos);
            Parameters[CheckChaosString] = CheckChaos.ToString();

            ResistanceThreshold = ImGuiExtension.IntSlider("Resistance Threshold", ResistanceThreshold, 0, 125);
            Parameters[ResistanceThresholdString] = ResistanceThreshold.ToString();

            return true;
        }

        public override Func<bool> GetCondition(ExtensionParameter extensionParameter)
        {
            return () =>
            {
                bool finalResult = true;

                if (finalResult && CheckCold)
                    finalResult = CheckResistance(extensionParameter, "Cold");
                if (finalResult && CheckFire)
                    finalResult = CheckResistance(extensionParameter, "Fire");
                if (finalResult && CheckLightning)
                    finalResult = CheckResistance(extensionParameter, "Lightning");
                if (finalResult && CheckChaos)
                    finalResult = CheckResistance(extensionParameter, "Chaos");

                return finalResult;
            };
        }

        private bool CheckResistance(ExtensionParameter profileParameter, string key)
        {
            // Initialize to 0
            int? current = 0;
            int? maximum = 0;

            Tuple<GameStat, GameStat> playerStat;
            if (resistanceTypes.TryGetValue(key, out playerStat))
            {
                // Get the stats
                current = profileParameter.Plugin.PlayerHelper.getPlayerStat(playerStat.Item1) ?? 0;
                maximum = profileParameter.Plugin.PlayerHelper.getPlayerStat(playerStat.Item2) ?? current;
                if (maximum < current)
                    maximum = current;
            }

            // Adjust the Resistance Threshold by any reduced res mods (such as on a map)
            if (current >= ResistanceThreshold - (maximum - current))
            {
                return false;
            }
            return true;
        }

        public override string GetDisplayName(bool isAddingNew)
        {
            string displayName = "Player Lowered Resists";

            if (!isAddingNew)
            {
                displayName += " [";
                if (CheckCold) displayName += ("Cold,");
                if (CheckFire) displayName += ("Fire,");
                if (CheckLightning) displayName += ("Lightning,");
                if (CheckChaos) displayName += ("Chaos,");
                displayName += ("ResistanceThreshold=" + ResistanceThreshold.ToString());

                displayName += "]";

            }

            return displayName;
        }
    }
}
