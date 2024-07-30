using Kentico.PageBuilder.Web.Mvc;

namespace XperienceComunity.TransformableViews.Models
{
    /// <summary>
    /// Class wrapper around the FormData field passed back in request.
    /// </summary>
    public class WidgetPropertiesForm<T> where T : class, IComponentProperties
    {
        /// <summary>
        /// The data returned from the http context can come in two different forms.  Grab which ever of those is not null and use it.
        /// </summary>
        public T Form => FormData == null ? FieldValues : FormData;
        public T FormData { get; set; }
        public T FieldValues { get; set; }
    }
}
