import React, {Component} from "react";
import {Table} from 'react-bootstrap'



export default class Store extends Component {
    static displayName = Store.name;

    constructor(props) {
        super(props)
        const {storeId,storeList} = props
        const {name,amount, category, price} = storeList[storeId]
        this.state = {
            items:[{name,amount, category, price}]
        }
        //
        // this.setLoginHandler = this.setLoginHandler.bind(this);
        // this.addStoreHandler = this.addStoreHandler.bind(this);

    }
    //
    // setLoginHandler(username){
    //     this.setState({
    //         isLoggedIn: true,
    //         username: username
    //     });
    // }
    //
    // addStoreHandler(storeName){
    //     alert(storeName)
    //     this.setState({
    //         storeList:[...this.state.storeList, storeName]
    //     });
    // }

    render () {

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
                    this.state.items.map((item,index)=>{
                        return(
                            <tr>
                                <td>{index+1}</td>
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
    }
}
