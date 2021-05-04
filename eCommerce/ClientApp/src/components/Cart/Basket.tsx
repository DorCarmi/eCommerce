import React, { Component } from 'react';
import {BasketData} from "../../Data/CartData";
import {Item} from "../../Data/Item";
import "./Basket.css"

interface BasketProps {
    basket: BasketData
}

export class Basket extends Component<BasketProps> {
    static displayName = Basket.name;

    constructor(props: BasketProps) {
        super(props);
    }

    renderItem(item: Item) {
        return (
            <div className="horizontalBorders">
                <div className="basketItemsContainer">
                    <label>Item: {item.itemName}</label>
                    <div>
                        Amount: <input>{item.amount}</input>
                    </div>
                </div>
            </div>
        )
    }


    renderBasketItems(items: Item[]) {
        return items.map(item => this.renderItem(item))
    }

    render() {
        const basket = this.props.basket;
        return (
            <div>
                <h3>{basket.storeId}</h3>
                {this.renderBasketItems(basket.items)}
            </div>
        );
    }
}