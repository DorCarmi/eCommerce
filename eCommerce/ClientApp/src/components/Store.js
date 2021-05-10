import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {Link} from "react-router-dom";
import {Item} from "../Data/Item";




export default class Store extends Component {
    static displayName = Store.name;

    constructor(props) {
        super(props)
        const {storeId} = props
        this.state = {
            storeId: storeId,
            items: []
        }
        this.storeApi = new StoreApi();
    }

    async componentDidMount() {
        const fetchedItems = await this.storeApi.getAllItems(this.state.storeId)
        if (fetchedItems && fetchedItems.isSuccess) {
            this.setState({
                items: fetchedItems.value
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
    
    removeItem = async (storeId,itemId) =>
    {
        const res = await this.storeApi.removeItem(storeId,itemId)

        if(res && res.isSuccess) {
            alert('edit item succeed')
            // this.props.addStoreToState(storeId);
            this.redirectToHome('/')
        }
        else{
            if(res) {
                alert(`edit item failed because- ${res.error}`)
            }
        }

    }
    
    render() {
        const {items,storeId} = this.state
        if (items.length > 0) {
            return (
                <div>
                    <Link to={`${storeId}/addItem`}>Add an Item</Link>
                    <Table striped bordered hover>
                    <thead>
                    <tr>
                        <th>#</th>
                        <th>Item name</th>
                        <th>Category</th>
                        <th>Amount</th>
                        <th>Price</th>
                    </tr>
                    </thead>
                    <tbody>

                    {
                        this.state.items.map((item, index) => {
                            return (
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{item.itemName}</td>
                                    <td>{item.category}</td>
                                    <td>{item.amount}</td>
                                    <td>{item.pricePerUnit}</td>
                                    <td>
                                        <div>
                                        <Link to={`${storeId}/editItem/${item.itemName}`}>Edit Item</Link>
                                        </div>
                                        <button onClick={() => this.removeItem(storeId,item.itemName)}>Remove Item</button>
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
                <div>
                 <Link to={`${storeId}/addItem`}>Add an Item</Link>   
                </div>
                Empty Store
            </div>
        }
    }
}
