import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {Link} from "react-router-dom";
import {UserApi} from "../Api/UserApi";
import {Item} from "../Data/Item";
import {StorePermission} from '../Data/StorePermission'
import {NavLink} from "reactstrap";

export default class StoreHistory extends Component {
    static displayName = StoreHistory.name;

    constructor(props) {
        super(props)
        const {storeId} = props
        this.state = {
            storeId: storeId,
            historyDetails: [],
        }
        this.storeApi = new StoreApi();
    }

    async getHistory(){
        const fetchedHistory = await this.storeApi.getPurchaseHistory(this.state.storeId)
        if (fetchedHistory && fetchedHistory.isSuccess) {
            console.log(fetchedHistory.value)
            this.setState({
                historyDetails: fetchedHistory.value.records
            })
        }
    }

    async componentDidMount() {
        await this.getHistory();
        const fetchedPermissions = await this.storeApi.getPurchaseHistory(this.state.storeId)
        if(fetchedPermissions.isSuccess){
            this.setState({
                permissions:fetchedPermissions.value
            })
        }
    }

    async componentDidUpdate(prevProps, prevState, undefined) {
        // if (prevProps.storeId !== this.props.storeId) {
        //     console.log(`update `);
        //     console.log(this.props);
        //     console.log(prevProps);
        //     await this.setState({
        //         storeId: this.props.storeId
        //     })
        //     await this.getItems();
        // }
    }

    redirectToHome = (path) => {
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
        const { storeId, permissions} = this.state
        return (
            // <div>Store History</div>)
            <div>
                <Table striped bordered hover>
                    <thead>
                    <tr>
                        <th>#</th>
                        <th>User Name</th>
                        <th>Store ID</th>
                        <th>Item</th>
                        <th>Amount</th>
                        <th>Total Price</th>
                        <th>Date</th>
                    </tr>
                    </thead>
                    <tbody>
                    
                    {
                        this.state.historyDetails.map((detail, index) => {
                            return (
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{detail.username}</td>
                                    <td>{detail.storeId}</td>
                                    <td>{detail.basket.items.map((item) => <div>{item.itemName}</div>)}</td>
                                    <td>{detail.basket.items.map((item) => <div>{item.amount}</div>)}</td>
                                    <td>{detail.basket.totalPrice}</td>
                                    <td>{detail.purchaseTime}</td>

                                </tr>
                            )
                        })
                    }
                    </tbody>
                </Table>
            </div>
        );
    }

}
