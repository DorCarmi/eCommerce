import React, {ChangeEvent, Component} from "react";
import {Item} from "../Data/Item";
import "./ItemDisplay.css"

interface ItemDisplayProps {
    item: Item
}

interface ItemDisplayState {
    quantity: number
}

export class ItemDisplay extends Component<ItemDisplayProps, ItemDisplayState> {
    static displayName = ItemDisplay.name;

    constructor(props: ItemDisplayProps) {
        super(props);

        this.state = {
            quantity: 0
        }
        this.handleQuantity = this.handleQuantity.bind(this);

    }

    handleQuantity(add: number){
        const {item} = this.props;
        this.setState({
            quantity: this.state.quantity + add > item.amount ? item.amount :
                      this.state.quantity + add < 0 ? 0 : 
                          this.state.quantity + add
        })
    }

    renderCartSection(){
        return (
            <div className="cartSection">
                <label>Quantity: {this.state.quantity}</label>
                <button onClick={() => this.handleQuantity(1)}>+1</button>
                <button onClick={() => this.handleQuantity(-1)}>-1</button>
                <button>Add to cart</button>
            </div>
        )
    }

    render() {
        const {item} = this.props;
        return (
            <div className="itemDisplay">
                <label className="label1">{item.itemName}</label>
                <label className="label2">{item.storeName}</label>
                <label>Price per unit: {item.pricePerUnit}</label>
                <label>Catagory: {item.category}</label>
                {this.renderCartSection()}
            </div>
        )
    }
}