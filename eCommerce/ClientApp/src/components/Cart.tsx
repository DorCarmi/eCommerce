import React, { Component } from 'react';
import {CartApi} from "../Api/CartApi";
import {Basket} from "./Cart/Basket";
import {Item} from "../Data/Item";
import {BasketData, CartData, CartDataType} from "../Data/CartData";

interface CartState {
    cart: CartData | undefined
}

export class Cart extends Component<{}, CartState> {
    static displayName = Cart.name;

    constructor(props: any) {
        super(props);
        this.state = {
            cart: undefined
        };
    }
    
    /*async componentDidMount() {
        let cartRes = await CartApi.getCart();
        if(cartRes.isFailure){
            alert(cartRes.Error);
        }
        this.setState({
            cart: cartRes.Value
        })
    }*/

    async componentDidMount() {
        const item1 = new Item({
            itemName: "phone",
            storeName: "store1",
            amount: 3,
            category: "electronics",
            keyWords: ["a", "a1"],
            pricePerUnit: 3
        })

        const item2 = new Item({
            itemName: "watermelon",
            storeName: "store2",
            amount: 3,
            category: "sweat",
            keyWords: ["a", "a1"],
            pricePerUnit: 10
        })
        
        const basket1 = new BasketData({
            storeId: "store1",
            items: [item1],
            totalPrice: 9
        })

        const basket2 = new BasketData({
            storeId: "store2",
            items: [item1, item2],
            totalPrice: 9
        })
        const cart = new CartData({baskets: [basket1, basket2]});
        
        this.setState({
            cart: cart
        })
    }

    renderCart(cart: CartData){
        const renderedBaskets = cart.baskets.map(basket => <Basket basket={basket}/> );
        return (
            <div>
                {renderedBaskets}
            </div>
        ) 
    }
    
    render() {
        const { cart } = this.state
        return (
            <main>
                { cart ? this.renderCart(cart) : <h1>Error</h1>}
            </main>
        );
    }
}