using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using HBS.TransformableViews_Experience;

[assembly: RegisterObjectType(typeof(TransformableViewInfo), TransformableViewInfo.OBJECT_TYPE)]

namespace HBS.TransformableViews_Experience
{
    /// <summary>
    /// Data container class for <see cref="TransformableViewInfo"/>.
    /// </summary>
    [Serializable, InfoCache(InfoCacheBy.ID | InfoCacheBy.Name | InfoCacheBy.Guid)]
    public partial class TransformableViewInfo : AbstractInfo<TransformableViewInfo, ITransformableViewInfoProvider>, IInfoWithId, IInfoWithName, IInfoWithGuid
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "hbs.transformableview";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(TransformableViewInfoProvider), OBJECT_TYPE, "HBS.TransformableView", "TransformableViewID", "TransformableViewLastModified", "TransformableViewGuid", "TransformableViewName", "TransformableViewDisplayName", null, null, null)
        {
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("TransformableViewTransformableViewTagID", "cms.tag", ObjectDependencyEnum.Required),
            },
        };


        /// <summary>
        /// Transformable view ID.
        /// </summary>
        [DatabaseField]
        public virtual int TransformableViewID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(TransformableViewID)), 0);
            set => SetValue(nameof(TransformableViewID), value);
        }


        /// <summary>
        /// Transformable view name.
        /// </summary>
        [DatabaseField]
        public virtual string TransformableViewName
        {
            get => ValidationHelper.GetString(GetValue(nameof(TransformableViewName)), String.Empty);
            set => SetValue(nameof(TransformableViewName), value);
        }


        /// <summary>
        /// Transformable view display name.
        /// </summary>
        [DatabaseField]
        public virtual string TransformableViewDisplayName
        {
            get => ValidationHelper.GetString(GetValue(nameof(TransformableViewDisplayName)), String.Empty);
            set => SetValue(nameof(TransformableViewDisplayName), value);
        }


        /// <summary>
        /// Transformable view content.
        /// </summary>
        [DatabaseField]
        public virtual string TransformableViewContent
        {
            get => ValidationHelper.GetString(GetValue(nameof(TransformableViewContent)), String.Empty);
            set => SetValue(nameof(TransformableViewContent), value);
        }


        /// <summary>
        /// Transformable view type.
        /// </summary>
        [DatabaseField]
        public virtual int TransformableViewType
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(TransformableViewType)), 0);
            set => SetValue(nameof(TransformableViewType), value);
        }


        /// <summary>
        /// Transformable view transformable view tag ID.
        /// </summary>
        [DatabaseField]
        public virtual int TransformableViewTransformableViewTagID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(TransformableViewTransformableViewTagID)), 0);
            set => SetValue(nameof(TransformableViewTransformableViewTagID), value);
        }


        /// <summary>
        /// Transformable view class name.
        /// </summary>
        [DatabaseField]
        public virtual string TransformableViewClassName
        {
            get => ValidationHelper.GetString(GetValue(nameof(TransformableViewClassName)), String.Empty);
            set => SetValue(nameof(TransformableViewClassName), value);
        }


        /// <summary>
        /// Transformable view guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid TransformableViewGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(TransformableViewGuid)), Guid.Empty);
            set => SetValue(nameof(TransformableViewGuid), value);
        }


        /// <summary>
        /// Transformable view last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime TransformableViewLastModified
        {
            get => ValidationHelper.GetDateTime(GetValue(nameof(TransformableViewLastModified)), DateTimeHelper.ZERO_TIME);
            set => SetValue(nameof(TransformableViewLastModified), value);
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected TransformableViewInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="TransformableViewInfo"/> class.
        /// </summary>
        public TransformableViewInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="TransformableViewInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public TransformableViewInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}