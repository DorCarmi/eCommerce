﻿import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {withRouter} from "react-router-dom";
import {Item} from "../Data/Item";



class EditItem extends Component {
    static displayName = EditItem.name;

    constructor(props) {
        super(props)
        this.state = {
            amount:undefined,
            storeId:'',
            category:'',
            keyWords:'',
            price:undefined
        }

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) history.push(path);
    }

    async handleSubmit(event){
        const {amount,category,keyWords,price} = this.state
        const {itemId,storeId} = this.props
        event.preventDefault();
        const res = await StoreApi.editItem(Item.createItem(itemId, storeId, amount, category, [keyWords], price))

        if(res && res.isSuccess) {
            alert('edit item succeed')
            // this.props.addStoreToState(storeId);
            this.redirectToHome('/')
        }
        else{
            alert(`edit item failed because- ${res.error}`)
        }

    }
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }


    render () {
        return (
            <main className="RegisterMain">
                <div className="RegisterWindow">
                    <div className="CenterItemContainer">
                        <h3>{`Edit The Item :${this.props.itemId}`}</h3>
                    </div>
                    <form className="RegisterForm" onSubmit={this.handleSubmit}>
                        <input type="number" name="amount" value={this.state.amount} onChange={this.handleInputChange}
                               placeholder={'Enter amount'} required/>
                        <input type="text" name="category" value={this.state.category} onChange={this.handleInputChange}
                               placeholder={'Enter Item Category'} required/>
                        <input type="text" name="keyWords" value={this.state.keyWords} onChange={this.handleInputChange}
                               placeholder={'Enter Item keyWords'} required/>
                        <input type="number" name="price" value={this.state.price} onChange={this.handleInputChange}
                               placeholder={'Enter Item price'} required/>
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="submit"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}

export default withRouter(EditItem);