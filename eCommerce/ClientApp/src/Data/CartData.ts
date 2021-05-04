import {Item} from "./Item";

export class BasketData {
    constructor(data) {
        this.items = data.items;
        this.totalPrice = data.totalPrice;
    }
}

export class CartData {
    constructor(data) {
        this.baskets = data.baskets;
    }
}