import { createContext } from "react";
import TransformableViewCategoryItem, { ITransformableViewCategoryItem } from "../../Shared/TransformableViewCategoryItem";

export interface TVCategoryListPageTemplateProperties {
    tags: TransformableViewCategoryItem[];
    taxonomies: TransformableViewCategoryItem[];
}

export interface TVCategoryListDialogOptions {
    isOpen: boolean
    headline: string
    message: string
    type?: "setCategory" | "deleteCategory"
}

export interface TVCategoryListPageState extends TVCategoryListPageTemplateProperties {
    dialogOptions: TVCategoryListDialogOptions,
    selectedCategory?: TransformableViewCategoryItem | null
}

export interface ITVCategoryListContext {
    setCurrentCategory: (value?: TransformableViewCategoryItem | null) => void
    setDialog: (x: TVDialogAction) => void
    dialogOptions: TVCategoryListDialogOptions
    tags: TransformableViewCategoryItem[],
    taxonomies: TransformableViewCategoryItem[],
    selectedCategory?: TransformableViewCategoryItem | null
}

export interface TVDialogAction {
    dialogOptions: TVCategoryListDialogOptions,
    selectCategory?: TransformableViewCategoryItem | null
}

export type TVCategoryActions = { type: "categorySelect", data?: ITransformableViewCategoryItem | null }
    | { type: "setDialog", data: TVDialogAction }

export const TVCategoryListReducer = (state: TVCategoryListPageState, action: TVCategoryActions) => {
    const { type, data } = action;
    const newState = { ...state };
    switch (type) {
        case "categorySelect":
            newState.selectedCategory = data;
            break;
        case "setDialog":
            newState.dialogOptions = data.dialogOptions;
            newState.selectedCategory = data.selectCategory;
            break;
    }
    return newState;
}

export const dialogDefaults = {
    isOpen: false,
    headline: "",
    message: ""
};

export const TVCategoryListContext = createContext<ITVCategoryListContext>({
    setCurrentCategory: (x?: ITransformableViewCategoryItem | null) => { },
    setDialog: (x: TVDialogAction) => { },
    dialogOptions: dialogDefaults,
    tags: [],
    taxonomies: []
});