import {RuleNode} from "./RuleInfo";

export enum DiscountNodeType {
    Leaf,
    Composite,
}

export type DiscountNode = DiscountNodeLeaf

export type DiscountNodeLeaf = {
    type: DiscountNodeType,
    rule: RuleNode
}

export function makeDiscountNodeLeaf(rule: RuleNode): DiscountNodeLeaf {
    return {
        type: DiscountNodeType.Leaf,
        rule: rule
    }
}