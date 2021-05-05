import React, { Component } from 'react';
import {CartApi} from "../Api/CartApi";
import {Basket} from "./Cart/Basket";
import {CartData} from "../Data/CartData";

interface CartState {
    errorMessage: string | undefined,
    cart: CartData | undefined
}

export class Cart extends Component<{}, CartState> {
    static displayName = Cart.name;

    constructor(props: any) {
        super(props);
        this.state = {
            errorMessage: undefined,
            cart: undefined
        };
        
        this.handleAmountUpdate = this.handleAmountUpdate.bind(this);
    }
    
    async handleAmountUpdate(storeId: string, itemId: string, amount: number){
        const updateRes = await CartApi.EditItemAmount(itemId, storeId, amount);
        if(!updateRes || updateRes.isFailure){
            this.setState({
                errorMessage: `Error in update: ${updateRes?.error}`,
                cart: undefined
            });
            return;
        }
    }
    
    async componentDidMount() {
        await this.getCart();
    }

    async getCart(){
        let cartRes = await CartApi.getCart();
        if(!cartRes || cartRes.isFailure){
            this.setState({
                errorMessage: `Error when getting cart: ${cartRes?.error}`,
                cart: undefined
            });
            return;
        }

        this.setState({
            cart: cartRes.value
        })
    }

    /*async componentDidMount() {
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
    }*/

    renderCart(cart: CartData){
        const renderedBaskets = cart.baskets.map(basket => <Basket basket={basket} handleAmountUpdate={this.handleAmountUpdate}/> );
        
        return (
            <div>
                {renderedBaskets.length == 0 ? <h4 className="None">Empty cart</h4> : renderedBaskets}
            </div>
        ) 
    }
    
    render() {
        const { cart, errorMessage } = this.state
        return (
            <main>
                { cart ?  this.renderCart(cart) : <h1>{errorMessage}</h1>}
            </main>
        );
    }
}