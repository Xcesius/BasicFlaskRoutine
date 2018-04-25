using ImGuiNET;
using PoeHUD.Poe.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeRoutine.Menu;
using TreeRoutine.Routine.BuildYourOwnRoutine.UI;

namespace TreeRoutine.Routine.BuildYourOwnRoutine.Extension.Default.Conditions
{
    internal class PlayerHasBuffCondition : ExtensionCondition
    {
        public bool HasBuff { get; set; } = false;
        public string HasBuffString { get; set; } = "HasBuff";
        public string SearchString { get; set; } = "ignited";
        public string SearchingBuff { get; set; } = "SearchingBuff";
        public int HasBuffSelecter { get; set; } = -1;
        public List<String> HasBuffList { get; set; } 


        public int CorruptCount { get; set; } = 0;
        public string HasBuffReady { get; set; } = "HasBuffReady";
        public string SearchStringString { get; set; } = "SearchStringString";
        public string CorruptCountString { get; set; } = "CorruptCount";

        public static List<string> GetEnumList<T>()
        {
            // validate that T is in fact an enum
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException();
            }

            return Enum.GetNames(typeof(T)).ToList();
        }


        public PlayerHasBuffCondition(string owner, string name) : base(owner, name)
        {

        }

        public override void Initialise(Dictionary<String, Object> Parameters)
        {
            base.Initialise(Parameters);

               HasBuff = ExtensionComponent.InitialiseParameterBoolean(HasBuffString, HasBuff, ref Parameters);
            HasBuffReady = ExtensionComponent.InitialiseParameterString(SearchingBuff, HasBuffReady, ref Parameters);
            SearchString = ExtensionComponent.InitialiseParameterString(SearchStringString, SearchString, ref Parameters);

            //    CorruptCount = ExtensionComponent.InitialiseParameterInt32(CorruptCountString, CorruptCount, ref Parameters);
        }

        public override bool CreateConfigurationMenu(ExtensionParameter extensionParameter, ref Dictionary<String, Object> Parameters)
        {
            ImGui.TextDisabled("Condition Info");
            ImGuiExtension.ToolTip("This condition will return true if the player has any of the selected ailments or a minimum of the specified corrupted blood stacks.");

            base.CreateConfigurationMenu(extensionParameter, ref Parameters);
            var kappa = GetEnumList<BuffEnums>().ToList();
            var test = kappa.Where(x => x.Contains(SearchString)).ToList();

            HasBuffReady = ImGuiExtension.ComboBox("Buff List", HasBuffReady, test);
            Parameters[SearchingBuff] = HasBuffReady.ToString();

            SearchString = ImGuiExtension.InputText("Filter Buffs", SearchString, 32, InputTextFlags.AllowTabInput);
            Parameters[SearchStringString] = SearchString.ToString();
            //HasBuff = ExtensionComponent.InitialiseParameterBoolean(HasBuffString, HasBuff, ref Parameters);
            //  CorruptCount = ImGuiExtension.IntSlider("Corruption Count", CorruptCount, 0, 20);
            //  Parameters[CorruptCountString] = CorruptCount.ToString();
            return true;
        }

        public override Func<bool> GetCondition(ExtensionParameter extensionParameter)
        {
            return () =>
            {
                //  if (RemFrozen && hasAilment(extensionParameter, extensionParameter.Plugin.Cache.DebuffPanelConfig.Frozen))
                //        return true;
                //   if (CorruptCount > 0 && hasAilment(extensionParameter, extensionParameter.Plugin.Cache.DebuffPanelConfig.Corruption, () => CorruptCount))
                //       return true;

                return false;
            };
        }

        private bool hasBuff(ExtensionParameter profileParameter, Dictionary<string, int> dictionary, Func<int> minCharges = null)
        {
            var buffs = profileParameter.Plugin.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>().Buffs;
            foreach (var buff in buffs)
            {
                if (float.IsInfinity(buff.Timer))
                    continue;

                int filterId = 0;
                if (dictionary.TryGetValue(buff.Name, out filterId))
                {
                    // I'm not sure what the values are here, but this is the effective logic from the old plugin
                    return (filterId == 0 || filterId != 1) && (minCharges == null || buff.Charges >= minCharges());
                }
            }
            return false;
        }

        public override string GetDisplayName(bool isAddingNew)
        {
            string displayName = "Has Buff";

            if (!isAddingNew)
            {
                displayName += " [";
                //  if (RemFrozen) displayName += ("Frozen,");
                //    if (CorruptCount > 0) displayName += ("Corrupt,");
                displayName += "]";

            }

            return displayName;
        }
    }
}
