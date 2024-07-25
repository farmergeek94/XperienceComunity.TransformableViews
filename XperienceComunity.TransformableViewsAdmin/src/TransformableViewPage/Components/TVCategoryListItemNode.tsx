import React, { useContext } from 'react'
import { TVCategoryListContext } from './Methods';
import { Icon, TreeNodeLeadingIcon, TreeNodeTitle, TreeNodeTrailingIcon } from '@kentico/xperience-admin-components';
import TransformableViewCategoryItem from '../../Shared/TransformableViewCategoryItem';

export default ({ selected, category, level }: { selected: boolean, category: TransformableViewCategoryItem, level: number }) => {
    const { setDialog, dialogOptions  } = useContext(TVCategoryListContext);

    return <div style={{display: 'flex', flexDirection: 'row', alignItems: 'center'}}>
         <TreeNodeLeadingIcon isSelected={selected} draggable={false}>
            <Icon name={ level == 1? "xp-parent-child-scheme" : "xp-tag"} />
        </TreeNodeLeadingIcon>
        <TreeNodeTitle isSelected={selected}>{category.transformableViewCategoryTitle}</TreeNodeTitle>
    </div>
}