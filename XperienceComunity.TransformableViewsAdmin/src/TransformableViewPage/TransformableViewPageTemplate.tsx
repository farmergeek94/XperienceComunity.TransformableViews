import { usePageCommand } from '@kentico/xperience-admin-base';
import { Button, ButtonColor, Cols, Column, Divider, DividerOrientation, Headline, HeadlineSize, Input, Paper, Row } from '@kentico/xperience-admin-components';
import React, { useEffect, useReducer, useRef } from 'react'
import './TransformableViewPage.css'
import { TVCategoryListContext, TVCategoryListPageTemplateProperties, TVCategoryListReducer, dialogDefaults, TVDialogAction } from './Components/Methods';
import CategoriesDialog from './Components/GeneralDialog';
import TransformableViewCategoryItem, { ITransformableViewCategoryItem } from '../Shared/TransformableViewCategoryItem';
import TVTreeView from './Components/TVTreeView';
import TVList from './Components/TVList';


export const TransformableViewPageTemplate = ({ tags, taxonomies }: TVCategoryListPageTemplateProperties) => {
    const [state, dispatch] = useReducer(TVCategoryListReducer, {
        tags,
        taxonomies,
        dialogOptions: dialogDefaults,
    });


    const rowRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (rowRef && rowRef.current) {
            rowRef.current.style.height = "100%";
        }
    }, [rowRef.current])

    const setCurrentCategory = (value?: TransformableViewCategoryItem | null) => {
        dispatch({ type: "categorySelect", data: value });
    }

    const setDialog = (data: TVDialogAction) => {
        dispatch({ type: "setDialog", data });
    }

    const { execute: exportViewsCommand } = usePageCommand("ExportViews");
    const { execute: importViewsCommand } = usePageCommand("ImportViews");

    return <div style={{ display: 'flex', flexDirection: "column", height: "100%" }}>
        <div style={{ paddingBottom: 20, display: "flex", alignItems: "end", justifyContent: "space-between" }}>
            <Headline size={HeadlineSize.M}>Transformable Views</Headline>
            <div style={{display: 'flex', gap: "10px", paddingRight: "20px"}}>
                <Button onClick={() => exportViewsCommand()} label="Export Views To FileSystem"></Button>
                <Button onClick={() => importViewsCommand()} label="Import Views To Database"></Button>
            </div>
        </div>
        <div style={{ flex: 1 }}>
            <TVCategoryListContext.Provider value={{ setCurrentCategory, dialogOptions: state.dialogOptions, setDialog, tags: state.tags, selectedCategory: state.selectedCategory, taxonomies: state.taxonomies }}>
                <Row ref={rowRef}>
                    <Column cols={Cols.Col3} className="treeview-wrapper">
                        <Paper fullHeight={true}>
                            <div style={{ padding: 20 }}>
                                <div style={{ paddingBottom: 20 }}><Headline size={HeadlineSize.S}>Taxonomies</Headline></div>
                                <div style={{ paddingBottom: 20 }}><Divider orientation={DividerOrientation.Horizontal} /></div>
                                <TVTreeView />
                            </div>
                        </Paper>
                    </Column>
                    <Column cols={Cols.Col9} className="views-wrapper">
                        <Paper fullHeight={true}>
                            <div style={{ padding: 20 }}>
                                <div style={{ paddingBottom: 20 }}><Headline size={HeadlineSize.S}>Views</Headline></div>
                                <div style={{ paddingBottom: 20 }}><Divider orientation={DividerOrientation.Horizontal} /></div>
                                {state.selectedCategory && state.selectedCategory.transformableViewCategoryID && <TVList categoryID={state.selectedCategory.transformableViewCategoryID} />}
                            </div>
                        </Paper>
                    </Column>
                </Row>
            </TVCategoryListContext.Provider>
        </div>
    </div>
}
 


