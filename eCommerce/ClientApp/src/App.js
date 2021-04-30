import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import Login from "./components/Login";
import Register from "./components/Register";
import './custom.css'
import {BrowserRouter} from "react-router-dom";

export default class App extends Component {
  static displayName = App.name;
  
  constructor(props) {
      super(props)
      alert("a")
      this.state = {
          isLoggedIn: false,
          username: undefined
      }
      
      this.setLoginHandler = this.setLoginHandler.bind(this);
  }
  
  setLoginHandler(username){
      this.setState({
          isLoggedIn: true,
          username: username
      });
  }

  render () {
    return (
        <BrowserRouter>>
          <Layout state={this.state}>
            <Route exact path='/' component={Home} />
            <Route path='/login' component={() => <Login setLoginState={this.setLoginHandler}/>} />
            <Route path='/register' component={Register} />
          </Layout>
        </BrowserRouter>
    );
  }
}
