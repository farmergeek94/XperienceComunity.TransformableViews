import { Icon, TreeNode, TreeNodeLeadingIcon, TreeNodeTitle, TreeNodeTrailingIcon, TreeView } from '@kentico/xperience-admin-components';
import React, { useContext, useMemo } from 'react'
import CategoryListItem from './TVCategoryListItem';
import { TVCategoryListContext } from './Methods';
import HierarchicalCategories from '../../Shared/HierarchicalCategories';

export default () => {
    const { tags, setDialog, dialogOptions, taxonomies } = useContext(TVCategoryListContext);

    const hierarchicalList = useMemo(() => {
        return HierarchicalCategories(taxonomies, tags);
    }, [tags, taxonomies]);

    return <TreeView>
            {hierarchicalList.map(x => <CategoryListItem key={x.transformableViewCategoryID} category={x} level={1} />)}
    </TreeView>
}