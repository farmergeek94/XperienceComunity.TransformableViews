export interface ITransformableViewCategoryItem {
    transformableViewCategoryTitle: string;
    transformableViewCategoryGuid?: string;
    transformableViewCategoryID?: number;
    transformableViewCategoryLastModified?: Date;
    transformableViewCategoryName?: string;
    transformableViewCategoryParentID?: number | null;
    transformableViewCategoryRootID?: number | null;
    transformableViewCategoryOrder?: number | null;
}
export default interface TransformableViewCategoryItem extends ITransformableViewCategoryItem {
    children?: TransformableViewCategoryItem[]
}