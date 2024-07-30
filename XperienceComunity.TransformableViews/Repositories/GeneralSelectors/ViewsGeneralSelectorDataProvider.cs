using HBS.TransformableViews_Experience;
using HBS.Xperience.TransformableViewsShared.Library;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;

namespace XperienceComunity.TransformableViews.Repositories.GeneralSelectors
{
    public class ViewsGeneralSelectorDataProvider : IGeneralSelectorDataProvider
    {
        private IEnumerable<ObjectSelectorListItem<string>> items;
        // Item representing an invalid selector option
        private static ObjectSelectorListItem<string> InvalidItem => new ObjectSelectorListItem<string>() { IsValid = false };


        // Returns items displayed in the general selector drop-down list
        public async Task<PagedSelectListItems<string>> GetItemsAsync(string searchTerm, int pageIndex, CancellationToken cancellationToken)
        {
            var views = TransformableViewInfo.Provider.Get().WhereEquals(nameof(TransformableViewInfo.TransformableViewType), (int)TransformableViewTypeEnum.Listing);


            // If a search term is entered, only loads users users whose first name starts with the term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                views.Where(cs => cs
                    .WhereContains(nameof(TransformableViewInfo.TransformableViewDisplayName), searchTerm)
                    .Or()
                    .WhereContains(nameof(TransformableViewInfo.TransformableViewName), searchTerm)
                );
            }

            views.Page(pageIndex, 20);

            var items = (await views.GetEnumerableTypedResultAsync()).Select(x => new ObjectSelectorListItem<string>
            {
                Text = x.TransformableViewDisplayName
                ,
                Value = x.TransformableViewName
                ,
                IsValid = true
            });


            return new PagedSelectListItems<string>()
            {
                NextPageAvailable = views.NextPageAvailable,
                Items = items
            };
        }

        // Returns ObjectSelectorListItem<string> options for all item values that are currently selected
        public async Task<IEnumerable<ObjectSelectorListItem<string>>> GetSelectedItemsAsync(IEnumerable<string> selectedValues, CancellationToken cancellationToken)
        {
            var views = await TransformableViewInfo.Provider.Get().Columns(nameof(TransformableViewInfo.TransformableViewDisplayName), nameof(TransformableViewInfo.TransformableViewName)).WhereEquals(nameof(TransformableViewInfo.TransformableViewType), (int)TransformableViewTypeEnum.Listing).GetEnumerableTypedResultAsync();
            return views.Where(x=> selectedValues.Contains(x.TransformableViewName)).Select(x => new ObjectSelectorListItem<string>
            {
                Text = x.TransformableViewDisplayName
                ,
                Value = x.TransformableViewName
                ,
                IsValid = true
            }) ?? [];
        }
    }
}
