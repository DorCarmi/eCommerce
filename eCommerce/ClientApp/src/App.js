import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Login } from "./components/Login";
import Store from './components/Store'
import {OpenStore} from "./components/OpenStore";
import { Cart } from "./components/Cart"
import Register from "./components/Register";
import './custom.css'
import AddItem from './components/AddItem'
import EditItem from './components/EditItem'

import {BrowserRouter,useHistory} from "react-router-dom";
import {UserApi} from "./Api/UserApi";
import {ItemSearchDisplay} from "./components/ItemsSearchDisplay";
import {SearchComponent} from "./components/SearchComponent";

export default class App extends Component {
  static displayName = App.name;
  
  constructor(props) {
      super(props)
      this.state = {
          isLoggedIn: false,
          storeList:[],
          userName:''
      }
      this.userApi = new UserApi();
      
      this.addStoreHandler = this.addStoreHandler.bind(this);
      this.updateLoginHandler = this.updateLoginHandler.bind(this);
  }
  
  async componentDidMount() {
      const userBasicInfo = await this.userApi.getUserBasicInfo();
      console.log(userBasicInfo.username);
      
      const fetchedStoredList = await this.userApi.getAllOwnedStoreIds()
      if (userBasicInfo && fetchedStoredList && fetchedStoredList.isSuccess) {
          this.setState({
              isLoggedIn:userBasicInfo.isLoggedIn,
              userName:userBasicInfo.username,
              storeList: fetchedStoredList.value
          })
      }
  }

    addStoreHandler(store){
        this.setState({
            storeList:[...this.state.storeList, store]
        });
    }

    async updateLoginHandler(username){
      this.setState({
          isLoggedIn: true,
          username: username
      })
    }
    
    redirectToHome = (path) => {
        alert(path)
        const { history } = this.props;
        if(history) {
            alert('succed')
            history.push(path);
        }
    }

    render () {
    return (
        <BrowserRouter>
          <Layout state={this.state}>
            <Route exact path='/' component={Home} />
            <Route exact path='/login' component={() => <Login isLoggedIn={this.state.isLoggedIn} loginUpdateHandler={this.updateLoginHandler}/>} />
            <Route exact path='/register' component={Register}/>
              <Route exact path='/cart' component={Cart} />
              <Route exact path="/store/:id" render={({match}) => (<Store  storeId={match.params.id} 
                                                                          storeList={this.state.storeList} redirect={this.redirectToHome}/>
              )} />            
              <Route exact path='/openStore' component={() => <OpenStore addStoreToState={this.addStoreHandler}/>} />
              <Route exact path="/store/:id/addItem" render={({match}) => <AddItem storeId ={match.params.id}/>} />
              <Route exact path="/store/:id/editItem/:itemId" render={({match}) => <EditItem storeId ={match.params.id} itemId ={match.params.itemId}/>} />
              <Route exact path="/searchItems/:query" render={({match}) => <ItemSearchDisplay itemQuery={match.params.query} />} />

              <Route exact path="/searchItems1" render={() => <SearchComponent />} />
          </Layout>
        </BrowserRouter>
    );
  }
}
