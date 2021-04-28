import React, { Component } from 'react';
import { Connect } from "../Api/AuthApi"
import "./Login.css"

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = { };
        
        this.handleConnect = this.handleConnect.bind(this);
    }

    async handleConnect(){
        const connectRes = await Connect();
        if(connectRes){
            this.setCookie(connectRes)
        } else {
            alert("Error")
        }
    }
    
    setCookie(token){
        let now = new Date();
        let time = now.getTime();
        let expireTime = time + 1000*36000;
        now.setTime(expireTime);
        document.cookie = `_auth=${token};expires=${now.toUTCString()};path=/;secure=true;`;
    }

    componentDidMount() {
        //this.populateWeatherData();
    }
    
    render() {
        return (
            <main class="LoginMain">
                <div class="LoginWindow">
                    <form class="LoginForm">
                        <input type="text" name="Username" placeholder="Username"/>
                        <input type="password" name="Password" placeholder="Password"/>
                        <input type="submit" value="Login"/>
                    </form>
                    <div className="RegisterConnect">
                        <a href="/register">Not registered yet?</a>
                        <button onClick={this.handleConnect}>Connect as guest</button>
                    </div>
                </div>
            </main>
        );
    }
}
