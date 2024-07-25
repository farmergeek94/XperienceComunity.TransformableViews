import { Dialog, Icon, Input } from '@kentico/xperience-admin-components'
import React, { useEffect, useMemo, useState } from 'react'
import { SelectListItem } from '../TransformableViewObjectsFormComponent'
import { FormComponentProps, useFormComponentCommandProvider } from '@kentico/xperience-admin-base'

interface ViewDialogProperties {
    value?: string
    onSelect: (x?: string) => void
    open: boolean
    closeDialog: () => void
    props: FormComponentProps
    className: string
}

export default ({ value, onSelect, open, closeDialog, props, className }: ViewDialogProperties) => {
    const [selectedView, setSelectedView] = useState(value);
    const [search, setSearch] = useState<string>();
    const [items, setItems] = useState<SelectListItem[]>([]);

    const { executeCommand } = useFormComponentCommandProvider();

    useEffect(() => {
        executeCommand && className && className.length != 0 && executeCommand<SelectListItem[], string>(props, 'GetViews', className)
            .then(data => {
                if (data) {
                    setItems(data);
                }
            });
    }, [executeCommand, className]);

    const filteredItems = useMemo(() => {
        if (!search || search.length == 0) {
            return items;
        }
        return items.filter(x => (x.text.toLowerCase().indexOf(search.toLowerCase()) > -1));
    }, [search, items]);

    const handleConfirm = () => {
        onSelect(selectedView);
        closeDialog();
    }

    return <Dialog isOpen={open} headline={"Select Views"} overlayClassName="dialog-z-index" onOverlayClick={(e) => { e.preventDefault(); return false; } } onClose={closeDialog} isDismissable={true} headerCloseButton={{ tooltipText: "Close Dialog" }} confirmAction={{ label: "Okay", onClick: handleConfirm }} cancelAction={{ label: "Cancel", onClick: closeDialog }}>
        <div style={{ paddingBottom: 0 }}>
            <Input placeholder={ "(Filter)" } value={search} onChange={(e) => setSearch(e.currentTarget.value)} />
        </div>
        <div>
            {filteredItems.map(x => <div key={x.value} onClick={() => setSelectedView(x.value)} className={"trbv-select-list-item" + (x.value == selectedView ? " selected": "")}>
                <span><Icon name="xp-chevron-right-circle" /></span>
                <span>{x.text}</span>
                <span className="selected-icon"><Icon name="xp-check-circle" /></span>
            </div>)}
        </div>
    </Dialog>
}