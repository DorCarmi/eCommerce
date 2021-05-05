import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {Link} from "react-router-dom";




export default class Store extends Component {
    static displayName = Store.name;

    constructor(props) {
        super(props)
        const {storeId} = props
        this.state = {
            storeId: storeId,
            items: []
        }
    }

    async componentDidMount() {
        const fetchedItems = await StoreApi.getAllItems(this.state.storeId)
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
                                    <td>{item.name}</td>
                                    <td>{item.category}</td>
                                    <td>{item.amount}</td>
                                    <td>{item.price}</td>
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
