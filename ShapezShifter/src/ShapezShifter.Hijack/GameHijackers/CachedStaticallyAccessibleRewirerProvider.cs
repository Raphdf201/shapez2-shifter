using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Collections.Scoped;
using Core.Logging;

namespace ShapezShifter.Hijack
{
    internal class CachedStaticallyAccessibleRewirerProvider : IRewirerProvider, IDisposable
    {
        private readonly ILogger Logger;
        private int VersionCacheIsBased;

        private readonly MultiValueDictionary<Type, IRewirer, ScopedList<IRewirer>> DataPerTypeCache = new(
            activator: ScopedList<IRewirer>.Get,
            deactivator: ScopedList<IRewirer>.Return);

        private readonly ScopedHashSet<Type> CachedTypes = ScopedHashSet<Type>.Get();

        public CachedStaticallyAccessibleRewirerProvider(ILogger logger)
        {
            Logger = logger;
        }

        public IEnumerable<TRewirer> RewirersOfType<TRewirer>()
            where TRewirer : IRewirer
        {
            if (VersionCacheIsBased != GameRewirers.Version)
            {
                CachedTypes.Clear();
                DataPerTypeCache.Clear();
                VersionCacheIsBased = GameRewirers.Version;
            }

            // LogDictionary();

            if (!CachedTypes.Contains(typeof(TRewirer)))
            {
                UpdateCacheEntryForType<TRewirer>();
                CachedTypes.Add(typeof(TRewirer));
            }

            // LogDictionary();

            var rewirersList = DataPerTypeCache.TryGetValuesForKey(key: typeof(TRewirer), values: out var list)
                ? list.Cast<TRewirer>()
                : Array.Empty<TRewirer>();

            return rewirersList;
        }

        private void LogDictionary()
        {
            Logger.Info?.Log($"KeyCount: {DataPerTypeCache.KeyCount}");
            Logger.Info?.Log($"ValueCount: {DataPerTypeCache.ValueCount()}");

            foreach (Type type in DataPerTypeCache.Keys)
            {
                if (!DataPerTypeCache.TryGetValuesForKey(key: type, values: out var list))
                {
                    Logger.Error?.Log("Huh?");
                }
                else
                {
                    Logger.Info?.Log($"Type {type} has {list.Count} entries");
                    foreach (IRewirer rewirer in list)
                    {
                        Logger.Info?.Log($"\t {rewirer}");
                    }
                }
            }
        }

        private void UpdateCacheEntryForType<TData>()
        {
            using var data = ScopedList<object>.Get();
            foreach (IRewirer obj in GameRewirers.Rewirers)
            {
                if (obj is TData)
                {
                    DataPerTypeCache.AddValue(key: typeof(TData), value: obj);
                }
            }
        }

        public void Dispose()
        {
            DataPerTypeCache.Dispose();
        }
    }
}
