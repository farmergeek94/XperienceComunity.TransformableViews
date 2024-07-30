using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;

namespace XperienceComunity.TransformableViews.Repositories.GeneralSelectors
{
    public class ClassGeneralSelectorDataProvider : IGeneralSelectorDataProvider
    {
        private IEnumerable<ObjectSelectorListItem<string>> items;
        // Item representing an invalid selector option
        private static ObjectSelectorListItem<string> InvalidItem => new ObjectSelectorListItem<string>() { IsValid = false };


        // Returns items displayed in the general selector drop-down list
        public Task<PagedSelectListItems<string>> GetItemsAsync(string searchTerm, int pageIndex, CancellationToken cancellationToken)
        {
            var exisitingTypes = ObjectTypeManager.ExistingObjectTypes;
            var types = ObjectTypeManager.RegisteredTypes.Where(x => exisitingTypes.Contains(x.ObjectType));


            // If a search term is entered, only loads users users whose first name starts with the term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                types = types.Where(x => x.ObjectType.Contains(searchTerm) || x.ObjectClassName.Contains(searchTerm));
            }

            var total = types.Count();

            var nextPageAvailable = false;

            if (pageIndex > 1)
            {
                types = types.Skip((pageIndex - 1) * 20);
                nextPageAvailable = pageIndex * 20 < total;
            }

            var items = types.Take(20).Select(x => new ObjectSelectorListItem<string>
            {
                Text = x.ObjectClassName
                ,
                Value = x.ObjectType
                ,
                IsValid = true
            });


            return Task.FromResult(new PagedSelectListItems<string>()
            {
                NextPageAvailable = nextPageAvailable,
                Items = items
            });
        }

        // Returns ObjectSelectorListItem<string> options for all item values that are currently selected
        public Task<IEnumerable<ObjectSelectorListItem<string>>> GetSelectedItemsAsync(IEnumerable<string> selectedValues, CancellationToken cancellationToken)
        {
            var exisitingTypes = ObjectTypeManager.ExistingObjectTypes;
            var types = ObjectTypeManager.RegisteredTypes.Where(x => exisitingTypes.Contains(x.ObjectType));

            var selectedType = types.Where(x => selectedValues.Contains(x.ObjectType)).Select(x => new ObjectSelectorListItem<string>
            {
                Text = x.ObjectClassName
                ,
                Value = x.ObjectType
                ,
                IsValid = true
            });

            return Task.FromResult(selectedType ?? []);
        } 
    }
}
