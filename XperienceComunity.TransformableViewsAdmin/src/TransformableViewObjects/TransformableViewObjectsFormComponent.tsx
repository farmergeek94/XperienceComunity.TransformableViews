import { FormComponentProps, useFormComponentCommandProvider } from '@kentico/xperience-admin-base'
import { Button, ButtonColor, DropDownSelectMenu, FormItemWrapper, Input, MenuItem, Select, TextArea, WindowPortal } from '@kentico/xperience-admin-components'
import React, { useEffect, useMemo, useState } from 'react'

import './TransformableViewObjectsFormComponent.css'
import ObjectTypesDialog from './Components/ObjectTypesDialog'
import ViewsDialog from './Components/ViewsDialog'

interface TransformableViewObjectsFormComponentModel {
    className: string
    columns?: string
    whereCondition?: string
    orderBy?: string
    topN?: number
    view?: string
    viewTitle?: string
    viewClassNames?: string
    viewCustomContent?: string
}

export interface SelectListItem {
    text: string,
    value: string
}

export const TransformableViewObjectsFormComponent = (props: FormComponentProps) => {
    const [views, setViews] = useState<SelectListItem[]>([])
    const [dialog, setDialog] = useState(false);
    const [viewsDialog, setViewsDialog] = useState(false);

    const value = useMemo(() => props.value as TransformableViewObjectsFormComponentModel, [props.value]);

    const update = (value: TransformableViewObjectsFormComponentModel) => {
        props.onChange && props.onChange(value);
    }

    const { executeCommand } = useFormComponentCommandProvider();

    useEffect(() => {
        executeCommand && value.className && value.className.length != 0 && executeCommand<SelectListItem[], string>(props, 'GetViews', value.className)
            .then(data => {
                if (data) {
                    setViews(data);
                }
            });
    }, [executeCommand, value.className]);

    return <FormItemWrapper
        label={props.label}
        explanationText={props.explanationText}
        invalid={props.invalid}
        validationMessage={props.validationMessage}
        markAsRequired={props.required}
        labelIcon={props.tooltip ? 'xp-i-circle' : undefined}
        labelIconTooltip={props.tooltip}
        childrenWrapperClassnames="transformable-view-component" >
        <div style={{ paddingBottom: 15 }}>
            <div style={{ paddingBottom: 10 }}>
                <Input label="Class Name" value={value.className} onChange={(e) => update({ ...value, className: e.currentTarget.value })} />
            </div>
            <Button label="Select Class Name" color={ButtonColor.Primary} onClick={() => setDialog(true)} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <Input label="Columns" value={value.columns} onChange={(e) => update({ ...value, columns: e.currentTarget.value })} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <Input label="Where Condition" value={value.whereCondition} onChange={(e) => update({ ...value, whereCondition: e.currentTarget.value })} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <Input label="Order By" value={value.orderBy} onChange={(e) => update({ ...value, orderBy: e.currentTarget.value })} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <Input type="number" label="Top Number" value={value.topN} onChange={(e) => update({ ...value, topN: e.currentTarget.valueAsNumber })} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <div style={{paddingBottom: 10}}>
                <Input label="View" value={value.view} onChange={(e) => update({ ...value, view: e.currentTarget.value })} />
            </div>
            <Button label="Select View" color={ButtonColor.Primary} onClick={() => setViewsDialog(true)} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <Input label="View Title" value={value.viewTitle} onChange={(e) => update({ ...value, viewTitle: e.currentTarget.value })} />
        </div>
        <div style={{ paddingBottom: 15 }}>
            <Input label="View Class Names" value={value.viewClassNames} onChange={(e) => update({ ...value, viewClassNames: e.currentTarget.value })} />
        </div>
        <div style={{ paddingBottom: 0 }}>
            <TextArea label="View Custom Content" value={value.viewCustomContent} onChange={(e) => update({ ...value, viewCustomContent: e.currentTarget.value })} />
        </div>
        <div style={{ zIndex: 999999999 }}>
        <WindowPortal>
            {dialog && <ObjectTypesDialog open={dialog} props={props} onSelect={(x) => update({ ...value, className: x })} closeDialog={() => setDialog(false)} value={value.className} />}
                {viewsDialog && <ViewsDialog open={viewsDialog} className={ value.className } props={props} onSelect={(x) => update({ ...value, view: x })} closeDialog={() => setViewsDialog(false)} value={value.view} />}
        </WindowPortal>
        </div>
    </FormItemWrapper>
}