
using HBS;

namespace XperienceComunity.TransformableViewsShared.Services
{
    public interface ITransformableViewService
    {
        bool DeleteViewsOnImport { get; }

        Task<bool> ExportViews(int id = 0);
        Task<bool> ImportSingleViewInternal(int id);
        Task<bool> ImportViewInternal();
        Task<bool> ImportViews(int id = 0);
        Task InsertTransformableView(string displayName, string workspaceName, IHBSTransformableDatabaseView view);
        Task UpdateTransformableView(int id, string editor);
    }
}