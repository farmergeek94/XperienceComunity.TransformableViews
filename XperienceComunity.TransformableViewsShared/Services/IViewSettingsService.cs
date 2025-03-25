namespace XperienceComunity.TransformableViewsShared.Services
{
    public interface IViewSettingsService
    {
        bool DeleteViewsOnImport { get; }
        string WorkSpaceName { get; set; }
    }
}