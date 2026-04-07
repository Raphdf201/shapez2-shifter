using System;
using Core.Logging;
using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class GameScenarioInterceptor : IDisposable
    {
        private readonly IRewirerProvider RewirerProvider;
        private readonly ILogger Logger;
        private readonly Hook ScenarioDeserializationHook;

        public GameScenarioInterceptor(IRewirerProvider rewirerProvider, ILogger logger)
        {
            RewirerProvider = rewirerProvider;
            Logger = logger;
            ScenarioDeserializationHook = DetourHelper.CreatePostfixHook<GameData, ScenarioId, GameScenario>(
                original: (gameData, uniqueId) => gameData.GetScenarioCloned(uniqueId),
                postfix: Postfix);
        }

        private GameScenario Postfix(GameData data, ScenarioId uniqueId, GameScenario gameScenario)
        {
            Logger.Info?.Log("Modifying research");
            var scenarioRewirers = RewirerProvider.RewirersOfType<IGameScenarioRewirer>();
            foreach (IGameScenarioRewirer scenarioRewirer in scenarioRewirers)
            {
                gameScenario = scenarioRewirer.ModifyGameScenario(gameScenario);
            }

            return gameScenario;
        }

        public void Dispose()
        {
            ScenarioDeserializationHook.Dispose();
        }
    }
}
