namespace XperienceComunity.TransformableViewsTool
{
    public interface ITransformableViewsCommandLine
    {
        void CreateJsonLoadFile(string extractPath);
        void Install();
        void ReadJsonLoadFile(string importPath);
    }
}