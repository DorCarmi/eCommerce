import React, {Component} from "react";


export default class Store extends Component {
    static displayName = Store.name;

    constructor(props) {
        super(props)
        this.state = {
            current_store:props.state.storeList
        }

        this.setLoginHandler = this.setLoginHandler.bind(this);
        this.addStoreHandler = this.addStoreHandler.bind(this);

    }

    setLoginHandler(username){
        this.setState({
            isLoggedIn: true,
            username: username
        });
    }

    addStoreHandler(storeName){
        alert(storeName)
        this.setState({
            storeList:[...this.state.storeList, storeName]
        });
    }

    render () {
        return (
            <div>
                My Store
            </div>
        );
    }
}
