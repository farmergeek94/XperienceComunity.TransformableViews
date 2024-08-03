using static HBS.Xperience.TransformableViewsShared.TransformableViewServices;

namespace XperienceComunity.TransformableViewsTool
{
    public interface ITransformableViewsCommandLine
    {
        void CreateJsonLoadFile(TransformableExport model);
        void Install();
        void ReadJsonLoadFile(TransformableImport model);
    }
}