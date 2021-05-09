import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {withRouter} from "react-router-dom";



class OpenStore extends Component {
    static displayName = OpenStore.name;

    constructor(props) {
        super(props)
        this.state = {
            storeId:''
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) history.push(path);
    }

    async handleSubmit(event){
        const {name,storeId,amount,category,keyWords,price} = this.state
        event.preventDefault();
        const res = await this.storeApi.openStore(storeId)
        if(res && res.isSuccess) {
            alert('add item succeed')
            // this.props.addStoreToState(storeId);
            this.redirectToHome('/')
        }
        else{
            alert(`add item failed because- ${res.error}`)
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
                        <h3>Open store</h3>
                    </div>
                    <form className="RegisterForm" onSubmit={this.handleSubmit}>
                        <input type="text" name="storeId" value={this.state.storeId}
                               placeholder={'Enter Store Id'} onChange={this.handleInputChange} required/>
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="submit"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}

export default withRouter(OpenStore);