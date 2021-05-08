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
import AddItem from './components/AddItem'
import EditItem from './components/EditItem'
import ShowResults from './components/ShowResults'


import {BrowserRouter,useHistory} from "react-router-dom";
import {UserApi} from "./Api/UserApi";
import {StoreApi} from "./Api/StoreApi";
import {ItemDisplay} from "./components/ItemDisplay";
import {Item} from "./Data/Item";

export default class App extends Component {
  static displayName = App.name;
  
  constructor(props) {
      super(props)
      this.state = {
          isLoggedIn: false,
          storeList:[],
          userName:''
      }
      
      this.setLoginHandler = this.setLoginHandler.bind(this);
      this.addStoreHandler = this.addStoreHandler.bind(this);
  }
  
  setLoginHandler(username){
      this.setState({
          isLoggedIn: true,
      });
  }
  async componentDidMount() {
      // remove
      const userBasicInfo = await UserApi.getUserBasicInfo();
      console.log(userBasicInfo.username);

      // remove
      const searchForItems = await StoreApi.searchItems("a");
      console.log(searchForItems);
      
      // remove
      const searchForStores = await StoreApi.searchStore("a");
      console.log(searchForStores);
      
      const fetchedStoredList = await UserApi.getAllOwnedStoreIds()
      if (userBasicInfo && fetchedStoredList && fetchedStoredList.isSuccess) {
          this.setState({
              isLoggedIn:userBasicInfo.isLoggedIn,
              userName:userBasicInfo.username,
              storeList: fetchedStoredList.value
          })
      }
  }

    addStoreHandler(store){
      alert("in app add store handler" + store)
        this.setState({
            storeList:[...this.state.storeList, store]
        });
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
            <Route path='/login' component={() => <Login setLoginState={this.setLoginHandler}/>} />
            <Route path='/register' component={Register}/>
              <Route path='/cart' component={Cart} />
              <Route exact path="/store/:id" render={({match}) => (<Store  storeId={match.params.id} 
                                                                          storeList={this.state.storeList} redirect={this.redirectToHome}/>
              )} />            
              <Route path='/openStore' exact component={() => <OpenStore addStoreToState={this.addStoreHandler} history={useHistory()}/>} />
              <Route exact path="/store/:id/addItem" render={({match}) => <AddItem storeId ={match.params.id}/>} />
              <Route exact path="/store/:id/editItem/:itemId" render={({match}) => <EditItem storeId ={match.params.id} itemId ={match.params.itemId}/>} />
              <Route exact path="/showResults/:itemId" render={({match}) => <ShowResults itemId ={match.params.itemId} />} />

              <Route exact path="/itemDisplay" render={() => <ItemDisplay
                  item={new Item({
                      itemName: "phone",
                      storeName: "store1",
                      amount: 3,
                      category: "electronics",
                      keyWords: ["a", "a1"],
                      pricePerUnit: 3
                  })}
                />} />

          </Layout>
        </BrowserRouter>
    );
  }
}
