import React, { Component } from 'react';
import "./Login.css"

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = { };
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
                </div>
            </main>
        );
    }

    async populateWeatherData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        this.setState({ forecasts: data, loading: false });
    }
}
