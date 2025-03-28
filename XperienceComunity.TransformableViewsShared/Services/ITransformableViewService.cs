
using HBS;

namespace XperienceComunity.TransformableViewsShared.Services
{
    public interface ITransformableViewService
    {
        bool DeleteViewsOnImport { get; }

        Task<bool> ExportViews(string language, int id = 0);
        string GetViewTypeString(IHBSTransformableDatabaseView view);
        Task<bool> ImportViews(string language, int id = 0);
        Task<bool> InsertTransformableView(string displayName, string workspaceName, IHBSTransformableDatabaseView view, string language);
        Task<bool> UpdateTransformableView(IHBSTransformableDatabaseView view, string language);
    }
}