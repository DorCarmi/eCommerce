import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { authApi } from "../Api/AuthApi"
import "./Login.css"
import {Link} from "react-router-dom";
import {CartApi} from "../Api/CartApi";

export class Cart extends Component {
    static displayName = Cart.name;

    constructor(props) {
        super(props);
        this.state = {
            cart: undefined
        };
    }
    
    async componentDidMount() {
        let cartRes = await CartApi.getCart();
        if(cartRes.isFailure){
            alert(cartRes.Error);
        }
        this.setState({
            cart: cartRes.Value
        })
    }

    render() {
        const { cart } = this.state
        return (
            <main>
                { cart ? <h1>Display cart</h1> : <h1>Display cart</h1>}
            </main>
        );
    }
}