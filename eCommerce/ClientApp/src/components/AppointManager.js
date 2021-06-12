﻿import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";

export class AppointManager extends Component {
    static displayName = AppointManager.name;

    constructor(props) {
        super(props)
        this.state = {
            managerId:'',
            submitted: false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    async handleSubmit(event){
        const {managerId} = this.state
        event.preventDefault();
        const res = await this.storeApi.appointManager(this.props.storeId,managerId)
        if(res && res.isSuccess) {
            this.setState({
                submitted: true
            })
        }
        else{
            alert(`appoint manager failed because- ${res.error}`)
        }

    }
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    render () {
        if (this.state.submitted) {
            return <Redirect exact to="/"/>
        } else {
            return (
                <main className="RegisterMain">
                    <div className="RegisterWindow">
                        <div className="CenterItemContainer">
                            <h3>{`Appoint a manager for store - ${this.props.storeId}`}:</h3>
                        </div>
                        <form className="RegisterForm" onSubmit={this.handleSubmit}>
                            <input type="text" name="managerId" value={this.state.managerId}
                                   placeholder={'Enter Manager Id'} onChange={this.handleInputChange} required/>
                            <div className="CenterItemContainer">
                                <input className="action" type="submit" value="submit"/>
                            </div>
                        </form>
                    </div>
                </main>
            );
        }
    }
}