import React, { useContext, useEffect, useState } from 'react'
import { TVCategoryListContext } from './Methods';
import { Dialog, Input } from '@kentico/xperience-admin-components';
export default () => {
    const { dialogOptions, setDialog, selectedCategory } = useContext(TVCategoryListContext);
    const [displayName, setDisplayName] = useState("");
    useEffect(() => { setDisplayName(selectedCategory?.transformableViewCategoryTitle ?? "") }, [selectedCategory])
    const handleCancel = () => {
        setDialog({ dialogOptions: { ...dialogOptions, isOpen: false }, selectCategory: null })
    }
    const handleConfirm = () => {
        if (dialogOptions.type == "setCategory" && selectedCategory) {
            setCategory({ ...selectedCategory, transformableViewCategoryDisplayName: displayName });
        } else if (dialogOptions.type == "deleteCategory" && selectedCategory) {
            deleteCategory(selectedCategory.transformableViewCategoryID ?? -1);
        }
        setDialog({ dialogOptions: { ...dialogOptions, isOpen: false }, selectCategory: null })
    }
    return <Dialog isOpen={dialogOptions.isOpen} headline={dialogOptions.headline} onClose={handleCancel} isDismissable={true} headerCloseButton={{ tooltipText: "Close Dialog" }} confirmAction={{ label: "Okay", onClick: handleConfirm }} cancelAction={{ label: "Cancel", onClick: handleCancel }}>
        {dialogOptions.type == "setCategory" && <Input label="Display Name" value={displayName} onChange={ (e) => setDisplayName(e.currentTarget.value) } />}
        {dialogOptions.message}
    </Dialog>
}