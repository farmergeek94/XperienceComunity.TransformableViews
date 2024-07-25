import TransformableViewCategoryItem from "./TransformableViewCategoryItem";

export default (taxonomies: TransformableViewCategoryItem[], tags: TransformableViewCategoryItem[]) => {
    tags.forEach(f => {
        f.children = tags
            .filter(g => g.transformableViewCategoryParentID === f.transformableViewCategoryID)
            .sort((a, b) => (a.transformableViewCategoryOrder ?? 0) - (b.transformableViewCategoryOrder ?? 0))
    });
    tags = tags.filter(f => f.transformableViewCategoryParentID == 0).sort((a, b) => (a.transformableViewCategoryOrder ?? 0) - (b.transformableViewCategoryOrder ?? 0));
    taxonomies.forEach(f => {
            f.children = tags
                .filter(g => g.transformableViewCategoryRootID === f.transformableViewCategoryID)
                .sort((a, b) => (a.transformableViewCategoryOrder ?? 0) - (b.transformableViewCategoryOrder ?? 0))
        
    });

    return taxonomies;
}