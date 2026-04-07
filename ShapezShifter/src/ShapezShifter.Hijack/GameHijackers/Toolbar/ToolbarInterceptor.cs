using MonoMod.RuntimeDetour;
using ShapezShifter.SharpDetour;

namespace ShapezShifter.Hijack
{
    internal class ToolbarInterceptor
    {
        private readonly IRewirerProvider RewirerProvider;

        private readonly Hook ModifyToolbarDataHook;
        private readonly Hook ModifyToolbarModelHook;

        public ToolbarInterceptor(IRewirerProvider rewirerProvider)
        {
            RewirerProvider = rewirerProvider;
            ModifyToolbarDataHook = DetourHelper.CreatePrefixHook<ToolbarBuilder, ToolbarData, IParentToolbarElement>(
                original: (t, a) => t.BuildToolbar(a),
                prefix: ModifyToolbarData);

            ModifyToolbarModelHook = DetourHelper.CreatePostfixHook<ToolbarBuilder, ToolbarData, IParentToolbarElement>(
                original: (t, a) => t.BuildToolbar(a),
                postfix: ModifyToolbarModel);
        }

        private ToolbarData ModifyToolbarData(ToolbarBuilder builder, ToolbarData toolbarData)
        {
            var toolbarDataRewirers = RewirerProvider.RewirersOfType<IToolbarDataRewirer>();

            foreach (IToolbarDataRewirer toolbarDataRewirer in toolbarDataRewirers)
            {
                toolbarData = toolbarDataRewirer.ModifyToolbarData(toolbarData);
            }

            return toolbarData;
        }

        private IParentToolbarElement ModifyToolbarModel(
            ToolbarBuilder builder,
            ToolbarData data,
            IParentToolbarElement toolbar)
        {
            var toolbarModelRewirers = RewirerProvider.RewirersOfType<IToolbarModelRewirer>();

            foreach (IToolbarModelRewirer toolbarModelRewirer in toolbarModelRewirers)
            {
                toolbar = toolbarModelRewirer.ModifyToolbarData(toolbar);
            }

            return toolbar;
        }

        public void Dispose()
        {
            ModifyToolbarModelHook.Dispose();
            ModifyToolbarDataHook.Dispose();
        }
    }
}
