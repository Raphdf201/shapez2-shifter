using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShapezShifter.Hijack
{
    public static class KeybindingsHelper
    {
        /// <summary>
        /// Finds an existing keybinding layer by ID
        /// </summary>
        public static KeybindingsLayer FindLayer(List<KeybindingsLayer> layers, string layerId)
        {
            return layers.FirstOrDefault(l => l.Id == layerId);
        }

        /// <summary>
        /// Finds or creates a keybinding layer. If the layer doesn't exist, creates a new one.
        /// </summary>
        public static KeybindingsLayer FindOrCreateLayer(List<KeybindingsLayer> layers, string layerId)
        {
            KeybindingsLayer existing = FindLayer(layers, layerId);
            if (existing != null)
                return existing;

            KeybindingsLayer newLayer = new KeybindingsLayer(layerId, new List<Keybinding>());
            layers.Add(newLayer);
            return newLayer;
        }

        public static void AddKeybindingToLayer(
            KeybindingsLayer layer,
            string partialId,
            KeySet primaryKey,
            KeySet? secondaryKey = null,
            bool blockableByUI = true,
            float axisThreshold = 0.001f)
        {
            Keybinding newBinding = new Keybinding(
                partialId,
                primaryKey,
                secondaryKey,
                blockableByUI,
                systemDefined: false,
                axisThreshold: axisThreshold
            );

            // Use reflection to add the binding to the layer's collection
            List<Keybinding> bindings = (List<Keybinding>)layer.Bindings;
            bindings.Add(newBinding);
            
            // Assign the full ID and load any saved preferences
            newBinding.AssignFullIdAndLoad($"{layer.Id}.{partialId}");
        }
        
        public static void AddKeybindingToLayer(KeybindingsLayer layer, Keybinding binding)
        {
            // Use reflection to add the binding to the layer's collection
            List<Keybinding> bindings = (List<Keybinding>)layer.Bindings;
            bindings.Add(binding);
                    
            // Assign the full ID and load any saved preferences
            binding.AssignFullIdAndLoad($"{layer.Id}.{binding.PartialId}");
        }

        /// <summary>
        /// Creates a simple single-key keybinding
        /// </summary>
        public static Keybinding CreateSimpleKeybinding(string partialId, KeyCode key, bool blockableByUI = true)
        {
            return new Keybinding(
                partialId,
                new KeySet(key),
                secondaryKeySet: null,
                blockableByUI: blockableByUI,
                systemDefined: false
            );
        }

        /// <summary>
        /// Creates a keybinding with a modifier (e.g., Ctrl+S)
        /// </summary>
        public static Keybinding CreateModifiedKeybinding(
            string partialId,
            KeyCode key,
            KeyCode modifier,
            bool blockableByUI = true)
        {
            return new Keybinding(
                partialId,
                new KeySet(key, modifier),
                secondaryKeySet: null,
                blockableByUI: blockableByUI,
                systemDefined: false
            );
        }

        /// <summary>
        /// Creates a keybinding with two modifiers (e.g., Ctrl+Shift+S)
        /// </summary>
        public static Keybinding CreateDoubleModifiedKeybinding(
            string partialId,
            KeyCode key,
            KeyCode modifier1,
            KeyCode modifier2,
            bool blockableByUI = true)
        {
            return new Keybinding(
                partialId,
                new KeySet(key, modifier1, modifier2),
                secondaryKeySet: null,
                blockableByUI: blockableByUI,
                systemDefined: false
            );
        }

        /// <summary>
        /// Creates a keybinding with both keyboard and controller support
        /// </summary>
        public static Keybinding CreateCrossInputKeybinding(
            string partialId,
            KeyCode keyboardKey,
            ControllerBinding controllerButton,
            bool blockableByUI = true)
        {
            return new Keybinding(
                partialId,
                new KeySet(keyboardKey),
                new KeySet(controllerSource: controllerButton),
                blockableByUI: blockableByUI,
                systemDefined: false
            );
        }

        public static bool KeybindingExists(KeybindingsLayer layer, string partialId)
        {
            return layer.Bindings.Any(b => b.PartialId == partialId);
        }

        /// <summary>
        /// Gets the default Control key based on platform (Cmd on Mac, Ctrl on others)
        /// </summary>
        public static KeyCode GetDefaultControlKey()
        {
            return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor
                ? KeyCode.LeftMeta
                : KeyCode.LeftControl;
        }
    }
}
