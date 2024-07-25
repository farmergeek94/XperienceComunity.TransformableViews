import React, { useEffect, useState } from 'react'
import TransformableViewItem from '../../Shared/TransformableViewItem'
import { usePageCommand } from '@kentico/xperience-admin-base';
import { ActionTile, Button, ButtonColor, Cols, Column, Divider, DividerOrientation, Grid, Icon, Input, Row, Spacing, useWindowDimensions, Dialog } from '@kentico/xperience-admin-components';
import ViewEditDialog, { ViewDialogOptions } from './ViewEditDialog';

export default ({ categoryID }: { categoryID: number }) => {
    const [views, setViews] = useState<TransformableViewItem[]>([]);
    const [showDialog, setShowDialog] = useState(false);
    const [selectedView, setSelectedView] = useState<TransformableViewItem>();
    const [search, setSearch] = useState<string>();

    const [columnNumber, setColumnNumber] = useState(3);

    const { width } = useWindowDimensions();

    useEffect(() => {
        if (width > 800) {
            setColumnNumber(3)
        } else {
            setColumnNumber(1)
        }
    }, [width]);

    const { execute: getViewsCommand } = usePageCommand<TransformableViewItem[], number | null>("GetViews", {
        after: (response) => {
            if (response) {
                setViews(response);
            }
        }
    });

    useEffect(() => {
        getViewsCommand(categoryID)
    }, [categoryID]);


    const { execute: setViewCommand } = usePageCommand<TransformableViewItem, TransformableViewItem>("SetView", {
        after: (response) => {
            if (response) {
                getViewsCommand(categoryID);
            }
        }
    });

    const { execute: deleteViewCommand } = usePageCommand<number, number>("DeleteView", {
        after: (response) => {
            if (response) {
                getViewsCommand(categoryID);
            }
        }
    });

    const { execute: exportViewCommand } = usePageCommand<void, number>("ExportView");
    const { execute: importViewCommand } = usePageCommand<void, number>("ImportView");

    const openDialog = (view: TransformableViewItem) => {
        setSelectedView(view);
        setShowDialog(true);
    }

    const deleteView = (id: number | undefined) => {
        if(id){
            var view = views.find(x=>x.transformableViewID == id)
            if(view){
                if( window.confirm("Delete view: " + view.transformableViewDisplayName)) {
                    deleteViewCommand(id);
                }
            }
        }
    }

    return <div>
        <div style={{ paddingBottom: 20 }}>
            <Row>
                <Column>
                    <Row spacing={Spacing.L}>
                        <Column cols={Cols.Col2}>
                            <Button fillContainer={true} label="Add View" color={ButtonColor.Primary} onClick={() => openDialog({ transformableViewContent: "", transformableViewDisplayName: "", transformableViewTransformableViewCategoryID: categoryID, transformableViewType: 0, transformableViewClassName: "" })}></Button>
                        </Column>
                        <Column><Input placeholder="Search" value={search} type="text" onChange={(e) => setSearch(e.currentTarget.value)} clearButton={<span onClick={() => setSearch(undefined)}><Icon name={"xp-modal-close"} /></span>} /></Column>
                    </Row>
                </Column>
            </Row>
        </div>
        <div style={{ paddingBottom: 20 }}><Divider orientation={DividerOrientation.Horizontal} /></div>

        <Grid cols={3} columnGap={Spacing.L} rowGap={ Spacing.L }>
            {views.map(x => <div key={x.transformableViewID } className="grid-view-item" onClick={() => openDialog(x)}>
                <span style={{ paddingRight: 8 }}>
                    <Icon name="xp-layout" />
                </span>
                {x.transformableViewDisplayName}
                <span style={{ float: 'right', flex: 1, gap: 10, display: 'flex', justifyContent: 'flex-end' }}>
                    <span onClick={(e) => { e.stopPropagation(); exportViewCommand(x.transformableViewID) }} style={{ color: "blue" }}>
                        <Icon name={"xp-arrow-down-line"} />
                    </span>
                    <span onClick={(e) => { e.stopPropagation(); importViewCommand(x.transformableViewID) }} style={{ color: "green" }}>
                        <Icon name={"xp-arrow-up-line"} />
                    </span>
                    <span onClick={(e) => { e.stopPropagation(); deleteView(x.transformableViewID) }} style={{ color: "red" }}>
                        <Icon name={"xp-bin"} />
                    </span>
                </span>
            </div>)}
        </Grid>
        {showDialog && <ViewEditDialog open={showDialog} selectedView={selectedView} setViewsCommand={setViewCommand} closeDialog={() => setShowDialog(false)} />}
    </div>
}