using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using HBS.Xperience.TransformableViews.Components;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using XperienceComunity.TransformableViews.Models;

namespace XperienceComunity.TransformableViews.Repositories.GeneralSelectors
{
    public class ColumnsGeneralSelectorDataProvider : IGeneralSelectorDataProvider
    {
        private IEnumerable<ObjectSelectorListItem<string>> items;
        // Item representing an invalid selector option
        private static ObjectSelectorListItem<string> InvalidItem => new ObjectSelectorListItem<string>() { IsValid = false };


        // Returns items displayed in the general selector drop-down list
        public async Task<PagedSelectListItems<string>> GetItemsAsync(string searchTerm, int pageIndex, CancellationToken cancellationToken)
        {
            var columns = await GetClassColumns();


            // If a search term is entered, only loads users users whose first name starts with the term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                columns = columns.Where(x => x.Contains(searchTerm));
            }

            var total = columns.Count();

            var nextPageAvailable = false;

            if (pageIndex > 1)
            {
                columns = columns.Skip((pageIndex - 1) * 20);
                nextPageAvailable = pageIndex * 20 < total;
            }

            var items = columns.Take(20).Select(x => new ObjectSelectorListItem<string>
            {
                Text = x
                ,
                Value = x
                ,
                IsValid = true
            });


            return new PagedSelectListItems<string>()
            {
                NextPageAvailable = nextPageAvailable,
                Items = items
            };
        }

        // Returns ObjectSelectorListItem<string> options for all item values that are currently selected
        public async Task<IEnumerable<ObjectSelectorListItem<string>>> GetSelectedItemsAsync(IEnumerable<string> selectedValues, CancellationToken cancellationToken)
        {
            var columns = await GetClassColumns();
            return columns.Where(x => selectedValues.Contains(x)).Select(x => new ObjectSelectorListItem<string>
            {
                Text = x
                ,
                Value = x
                ,
                IsValid = true
            }) ?? [];
        }

        private async Task<IEnumerable<string>> GetClassColumns()
        {
            // Get the http context
            var httpContextAccessor = Service.ResolveOptional<IHttpContextAccessor>();
            var form = httpContextAccessor?.HttpContext?.Request.Form;
            if (form != null)
            {
                // make sure there is a command
                if (form.TryGetValue("command", out StringValues command))
                {
                    // Get the form data passed back to the filter. 
                    if (form.TryGetValue("data", out StringValues data))
                    {
                        try
                        {
                            // parse the form data down to a readable properties format
                            var formData = JsonSerializer.Deserialize<WidgetPropertiesForm<TransformableViewObjectsWidgetProperties>>(data, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (formData?.Form != null && !string.IsNullOrEmpty(formData?.Form.ClassName))
                            {
                                var className = formData?.Form.ClassName;

                                var classForm = await DataClassInfoProvider.ProviderObject.GetAsync(className);

                                if (classForm != null) {

                                    var formInfo = new FormInfo(classForm.ClassFormDefinition);
                                    return formInfo.GetColumnNames();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return [];
                        }
                    }
                }
            }
            return [];
        }
    }
}
