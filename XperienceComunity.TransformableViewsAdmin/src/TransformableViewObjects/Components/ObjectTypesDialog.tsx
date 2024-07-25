import { Dialog, Icon, Input } from '@kentico/xperience-admin-components'
import React, { useEffect, useMemo, useState } from 'react'
import { SelectListItem } from '../TransformableViewObjectsFormComponent'
import { FormComponentProps, useFormComponentCommandProvider } from '@kentico/xperience-admin-base'

interface ObjectTypeDialogProperties {
    value: string
    onSelect: (x: string) => void
    open: boolean
    closeDialog: () => void
    props: FormComponentProps
}

export default ({value, onSelect, open, closeDialog, props }: ObjectTypeDialogProperties) => {
    const [selectedType, setSelectedType] = useState(value);
    const [search, setSearch] = useState<string>();
    const [items, setItems] = useState<SelectListItem[]>([]);

    const { executeCommand } = useFormComponentCommandProvider();

    useEffect(() => {
        executeCommand && executeCommand<SelectListItem[]>(props, 'GetObjectTypes')
            .then(data => {
                if (data) {
                    setItems(data);
                }
            });
    }, [executeCommand]);

    const filteredItems = useMemo(() => {
        if (!search || search.length == 0) {
            return items;
        }
        return items.filter(x => (x.text.toLowerCase().indexOf(search.toLowerCase()) > -1));
    }, [search, items]);

    const handleConfirm = () => {
        onSelect(selectedType);
        closeDialog();
    }

    return <Dialog isOpen={open} headline={"Select Types"} overlayClassName="dialog-z-index" onOverlayClick={ (e) => e.preventDefault()} onClose={closeDialog} isDismissable={true} headerCloseButton={{ tooltipText: "Close Dialog" }} confirmAction={{ label: "Okay", onClick: handleConfirm }} cancelAction={{ label: "Cancel", onClick: closeDialog }}>
        <div style={{ paddingBottom: 0 }}>
            <Input placeholder={ "(Filter)" } value={search} onChange={(e) => setSearch(e.currentTarget.value)} />
        </div>
        <div>
            {filteredItems.map(x => <div key={x.value} onClick={() => setSelectedType(x.value)} className={"trbv-select-list-item" + (x.value == selectedType ? " selected": "")}>
                <span><Icon name="xp-chevron-right-circle" /></span>
                <span>{x.text}</span>
                <span className="selected-icon"><Icon name="xp-check-circle" /></span>
            </div>)}
        </div>
    </Dialog>
}