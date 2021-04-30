import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import {Login} from "./components/Login";
import {Register} from "./components/Register";
import './custom.css'

export default class App extends Component {
  static displayName = App.name;
  
  constructor(props) {
      super(props);
  }
  
  

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/login' component={Login} />
        <Route path='/register' component={Register} />
      </Layout>
    );
  }
}
