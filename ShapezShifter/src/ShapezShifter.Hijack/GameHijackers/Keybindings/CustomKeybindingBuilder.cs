using System.Collections.Generic;
using UnityEngine;

namespace ShapezShifter.Hijack
{
    /// <summary>
    /// Fluent builder for creating custom keybindings with a clean, readable API
    /// </summary>
    public class CustomKeybindingBuilder
    {
        private string LayerId;
        private string PartialId;
        private KeySet PrimaryKeySet;
        private KeySet? SecondaryKeySet;
        private bool BlockableByUI = true;
        private float AxisThreshold = 0.001f;
        private readonly List<KeybindingsLayer> Layers;

        internal CustomKeybindingBuilder(List<KeybindingsLayer> layers)
        {
            Layers = layers;
        }

        /// <summary>
        /// Sets the layer ID where this keybinding should be added
        /// </summary>
        public CustomKeybindingBuilder InLayer(string layerId)
        {
            LayerId = layerId;
            return this;
        }

        /// <summary>
        /// Sets the partial ID (name) of the keybinding
        /// </summary>
        public CustomKeybindingBuilder WithId(string partialId)
        {
            PartialId = partialId;
            return this;
        }

        /// <summary>
        /// Sets a simple primary key (e.g., "T")
        /// </summary>
        public CustomKeybindingBuilder WithPrimaryKey(KeyCode key)
        {
            PrimaryKeySet = new KeySet(key);
            return this;
        }

        /// <summary>
        /// Sets a primary key with one modifier (e.g., Ctrl+S)
        /// </summary>
        public CustomKeybindingBuilder WithPrimaryKey(KeyCode key, KeyCode modifier)
        {
            PrimaryKeySet = new KeySet(key, modifier);
            return this;
        }

        /// <summary>
        /// Sets a primary key with two modifiers (e.g., Ctrl+Shift+S)
        /// </summary>
        public CustomKeybindingBuilder WithPrimaryKey(KeyCode key, KeyCode modifier1, KeyCode modifier2)
        {
            PrimaryKeySet = new KeySet(key, modifier1, modifier2);
            return this;
        }

        /// <summary>
        /// Sets a controller button as the primary input
        /// </summary>
        public CustomKeybindingBuilder WithPrimaryController(ControllerBinding button)
        {
            PrimaryKeySet = new KeySet(controllerSource: button);
            return this;
        }

        /// <summary>
        /// Sets a simple secondary/alternative key
        /// </summary>
        public CustomKeybindingBuilder WithSecondaryKey(KeyCode key)
        {
            SecondaryKeySet = new KeySet(key);
            return this;
        }

        /// <summary>
        /// Sets a secondary key with one modifier
        /// </summary>
        public CustomKeybindingBuilder WithSecondaryKey(KeyCode key, KeyCode modifier)
        {
            SecondaryKeySet = new KeySet(key, modifier);
            return this;
        }

        /// <summary>
        /// Sets a controller button as secondary input
        /// </summary>
        public CustomKeybindingBuilder WithSecondaryController(ControllerBinding button)
        {
            SecondaryKeySet = new KeySet(controllerSource: button);
            return this;
        }

        /// <summary>
        /// Sets whether the UI can block this keybinding (default: true)
        /// </summary>
        public CustomKeybindingBuilder WithBlockableByUI(bool blockable = true)
        {
            BlockableByUI = blockable;
            return this;
        }

        /// <summary>
        /// Sets the axis threshold for analog inputs (default: 0.001)
        /// </summary>
        public CustomKeybindingBuilder WithAxisThreshold(float threshold)
        {
            AxisThreshold = threshold;
            return this;
        }

        /// <summary>
        /// Builds and adds the keybinding to the specified layer
        /// </summary>
        public void Build()
        {
            if (string.IsNullOrEmpty(LayerId))
                throw new System.InvalidOperationException("Layer ID must be set using InLayer()");
            
            if (string.IsNullOrEmpty(PartialId))
                throw new System.InvalidOperationException("Partial ID must be set using WithId()");
            
            if (PrimaryKeySet.Empty)
                throw new System.InvalidOperationException("Primary key must be set using WithPrimaryKey() or WithPrimaryController()");

            KeybindingsLayer layer = KeybindingsHelper.FindOrCreateLayer(Layers, LayerId);
            KeybindingsHelper.AddKeybindingToLayer(
                layer,
                PartialId,
                PrimaryKeySet,
                SecondaryKeySet,
                BlockableByUI,
                AxisThreshold
            );
        }
    }

    /// <summary>
    /// Extension methods for easy access to the fluent builder
    /// </summary>
    public static class KeybindingBuilderExtensions
    {
        /// <summary>
        /// Starts building a new custom keybinding
        /// </summary>
        public static CustomKeybindingBuilder AddKeybinding(this List<KeybindingsLayer> layers)
        {
            return new CustomKeybindingBuilder(layers);
        }
    }
}
