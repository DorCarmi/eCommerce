import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import Login from "./components/Login";
import Store from './components/Store'
import OpenStore from "./components/OpenStore";
import { Cart } from "./components/Cart"
import Register from "./components/Register";
import './custom.css'
import {BrowserRouter,useHistory} from "react-router-dom";

export default class App extends Component {
  static displayName = App.name;
  
  constructor(props) {
      super(props)
      this.state = {
          isLoggedIn: false,
          storeList:[],
          username: undefined
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

    addStoreHandler(store){
      alert(store)
        this.setState({
            storeList:[...this.state.storeList, store]
        });
    }

  render () {
    return (
        <BrowserRouter>>
          <Layout state={this.state}>
            <Route exact path='/' component={Home} />
            <Route path='/login' component={() => <Login setLoginState={this.setLoginHandler}/>} />
            <Route path='/register' component={Register} />
              <Route path='/cart' component={Cart} />
              <Route exact path="/store/:id" render={({match}) => (<Store storeId={match.params.id} 
                                                                          storeList={this.state.storeList} />
              )} />            
              <Route path='/openStore' exact component={() => <OpenStore addStoreToState={this.addStoreHandler} history={useHistory()}/>} />


          </Layout>
        </BrowserRouter>
    );
  }
}
