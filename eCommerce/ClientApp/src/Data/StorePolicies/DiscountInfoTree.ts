export enum DiscountNodeType {
    Leaf,
    Composite,
}

export type RuleNode = DiscountNodeLeaf

export type DiscountNodeLeaf = {
    type: DiscountNodeType,
    rule: RuleNode
}

export function makeRuleNodeLeaf(rule: RuleNode): DiscountNodeLeaf {
    return {
        type: DiscountNodeType.Leaf,
        rule: rule
    }
}