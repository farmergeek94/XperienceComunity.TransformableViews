using static HBS.Xperience.TransformableViewsShared.TransformableViewServices;

namespace XperienceComunity.TransformableViewsTool
{
    public interface ITransformableViewsCommandLine
    {
        Task CreateJsonLoadFile(TransformableExport model);
        Task Install();
        Task ReadJsonLoadFile(TransformableImport model);
    }
}