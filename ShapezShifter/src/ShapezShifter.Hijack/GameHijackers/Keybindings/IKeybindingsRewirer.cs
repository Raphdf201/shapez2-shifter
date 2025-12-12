using System.Collections.Generic;

namespace ShapezShifter.Hijack
{
    public interface IKeybindingsRewirer : IRewirer
    {
        /// <summary>
        /// Modify the keybinding layers. You can add new layers or modify existing ones.
        /// </summary>
        void ModifyKeybindingLayers(List<KeybindingsLayer> layers);
    }
}
