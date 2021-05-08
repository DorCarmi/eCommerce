import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {CartApi} from "../Api/CartApi"
import {Link} from "react-router-dom";
import {Item} from "../Data/Item";




export default class ShowResults extends Component {
    static displayName = ShowResults.name;

    constructor(props) {
        super(props)
        this.state = {
            itemsToShow:[]
        }
    }

    async componentDidMount() {
        const searchForItems = await StoreApi.searchItems(this.props.itemId);
        console.log(searchForItems);        
        if (searchForItems) {
            this.setState({
                itemsToShow: searchForItems.value
            })
        }
    }
    redirectToHome = (path) => {
        alert(path)
        const { history } = this.props;
        if(history) {
            alert('succed')
            history.push(path);
        }
    }

    async addToCart(itemId,storeId,amount){
        const additionStatus = await CartApi.AddItem(itemId,storeId,amount);
        console.log(additionStatus);
        if (additionStatus.isSuccess) {
            alert('Item has been added to the cart')
        }
        else{
            alert('Operation Failed')
        }
        
    }

    render() {
        const {itemsToShow} = this.state
        if (itemsToShow.length > 0) {
            return (
                <div>
                    <Table striped bordered hover>
                        <thead>
                        <tr>
                            <th>#</th>
                            <th>Item name</th>
                            <th>Store ID</th>
                            <th>Category</th>
                            <th>Amount</th>
                            <th>Price</th>
                        </tr>
                        </thead>
                        <tbody>

                        {
                            itemsToShow.map((item, index) => {
                                return (
                                    <tr>
                                        <td>{index + 1}</td>
                                        <td>{item.itemName}</td>
                                        <td>{item.storeName}</td>
                                        <td>{item.category}</td>
                                        <td>{item.amount}</td>
                                        <td>{item.pricePerUnit}</td>
                                        <td>
                                            <button onClick={() => this.addToCart(item.itemName,item.storeName,item.amount)}>Add To Cart</button>
                                        </td>
                                    </tr>
                                )
                            })
                        }
                        </tbody>
                    </Table>
                </div>
            );
        } else {
            return <div>
                There are no matching items in the store
            </div>
        }
    }
}
