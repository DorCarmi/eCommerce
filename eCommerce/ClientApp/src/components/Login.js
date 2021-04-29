﻿import React, { Component } from 'react';
import { authApi } from "../Api/AuthApi"
import "./Login.css"

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = { 
            loginError: undefined,
            username: undefined,
            password: undefined,
            role: "member"
        };
        
        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }
    
    async handleSubmit(event){
        event.preventDefault();
        const {username, password, role} = this.state;
        const loginRedirectAndRes = await authApi.Login(username, password, role);
        if(loginRedirectAndRes) {
            const loginRes = loginRedirectAndRes.data;

            if (loginRes && loginRes.isSuccess) {
                window.location = loginRedirectAndRes.redirect
            } else {
                this.setState({
                    loginError: loginRes.error
                })
            }
        } else {
            this.setState({
                loginError: "You need to be a guest"
            })
        }
    }

    componentDidMount() {
        //this.populateWeatherData();
    }
    
    render() {
        return (
            <main class="LoginMain">
                <div class="LoginWindow">
                    <form class="LoginForm" onSubmit={this.handleSubmit}>
                        {this.state.loginError ? <div class="CenterItemContainer"><label>{this.state.loginError}</label></div> : null}
                        <input type="text" name="username" value={this.state.username} onChange={this.handleInputChange} placeholder="Username" required/>
                        <input type="password" name="password" value={this.state.password} onChange={this.handleInputChange} placeholder="Password" required/>
                        <select name="role" value={this.state.role} onChange={this.handleInputChange} required>
                            <option value="member">Member</option>
                            <option value="admin">Admin</option>
                        </select>
                        <div className="ConnectRegister">
                            <a href="/register">Create new account</a>
                            <input type="submit" value="Login"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}
