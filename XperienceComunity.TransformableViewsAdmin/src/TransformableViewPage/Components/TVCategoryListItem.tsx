import React, { useContext, useEffect, useState } from 'react'
import { TVCategoryListContext } from './Methods';
import { DropPlacement, Icon, TreeNode, TreeNodeLeadingIcon, TreeNodeTitle } from '@kentico/xperience-admin-components';
import CategoryListItemNode from './TVCategoryListItemNode';
import TransformableViewCategoryItem, { ITransformableViewCategoryItem } from '../../Shared/TransformableViewCategoryItem';

const CategoryListItem =  ({ category, level }: { category: TransformableViewCategoryItem, level: number }) => {
    const { dialogOptions, setDialog, tags, setCurrentCategory } = useContext(TVCategoryListContext);

    const [expanded, setExpended] = useState(level > 1);

    return <TreeNode
        key={category.transformableViewCategoryName ?? ""}
        nodeIdentifier={category.transformableViewCategoryID?.toString() ?? ""}
        isDraggable={false}
        isToggleable={(category.children?.length ?? 0) > 0}
        onNodeToggle={(e) => setExpended(e)}
        isExpanded={expanded}
        onNodeClick={() => { level > 1 ? setCurrentCategory(category) : setCurrentCategory() }}
        isSelectable={level > 1}
        renderNode={(isNodeSelected) => <div><CategoryListItemNode selected={isNodeSelected} category={category} level={ level } /></div>}
        level={level}>
        {category.children && category.children.map(x => <CategoryListItem key={x.transformableViewCategoryID} category={x} level={level + 1} />)}
    </TreeNode>
}
export default CategoryListItem;