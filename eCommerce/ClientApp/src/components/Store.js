import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";


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

    render() {
        const {items} = this.state
        if (items.length > 0) {
            return (
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

            );
        } else {
            return <div>Error occured while fetching items of this store</div>
        }
    }
}
