import React, { Component } from 'react';
import { authApi } from "../Api/AuthApi"
import "./Register.css"

export class Register extends Component {
    static displayName = Register.name;

    constructor(props) {
        super(props);
        this.state = { 
            registrationError: undefined,
            username: undefined,
            password: undefined,
            email: undefined,
            name: undefined,
            address: undefined,
            birthday: undefined
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
        const {username, password, email, name, address, birthday} = this.state;
        /* loginRedirectAndRes = await authApi.Login(username, password, role);
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
        }*/
    }

    componentDidMount() {
        //this.populateWeatherData();
    }
    
    render() {
        return (
            <main class="RegisterMain">
                <div class="RegisterWindow">
                    <div class="CenterItemContainer">
                        <h3>Register</h3>
                    </div>
                    <form class="RegisterForm" onSubmit={this.handleSubmit}>
                        {this.state.registrationError ? <div class="CenterItemContainer"><label>{this.state.registrationError}</label></div> : null}
                        <input type="text" name="username" value={this.state.username} onChange={this.handleInputChange} placeholder="Username" required/>
                        <input type="password" name="password" value={this.state.password} onChange={this.handleInputChange} placeholder="Password" required/>
                        <input type="email" name="email" value={this.state.email} onChange={this.handleInputChange} placeholder="email@email.com" required/>
                        <input type="text" name="name" value={this.state.name} onChange={this.handleInputChange} placeholder="Your name" required/>
                        <input type="text" name="address" value={this.state.address} onChange={this.handleInputChange} placeholder="Your address" required/>
                        <input type="date" name="birthday" value={this.state.birthday} onChange={this.handleInputChange} required/>
                        <div className="CenterItemContainer">
                            <input class="action" type="submit" value="Register"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}
