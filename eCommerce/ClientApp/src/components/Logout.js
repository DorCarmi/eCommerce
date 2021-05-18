import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { AuthApi } from "../Api/AuthApi"
import "./Login.css"
import {Redirect} from "react-router-dom";
import {UserRole} from "../Data/UserRole";

export class Logout extends Component {
    static displayName = Logout.name;

    constructor(props) {
        super(props);
        this.state = {
            redirectHome:false
        };
        this.authApi = new AuthApi();

    }
    
    async componentDidMount() {
        const logoutStatus = await this.authApi.Logout()
        if(logoutStatus.isSuccess){
            this.props.logoutUpdateHandler()
        }

    }

    render() {
                return <Redirect exact to="/"/>
            
        }
}