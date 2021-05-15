import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink,Input } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import {Dropdown,DropdownButton} from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import {StoreApi} from "../Api/StoreApi";
import {SearchComponent} from "./SearchComponent";



export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
     super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {  
      collapsed: true,
      storeList:[],
      isLoggedIn:this.props.state.isLoggedIn,
      itemToSearch:''
    };
    this.storeApi = new StoreApi();

    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }


  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

    handleInputChange(event) {
      const target = event.target;
      this.setState({
        [target.name]: target.value
      });
    }
    
  
  
  handleSubmit(){
    
    const searchItem = async () =>
    {
      const searchForItems = await this.storeApi.searchItems(this.state.itemToSearch);
      console.log(searchForItems);
      return searchForItems
    }
    
  }

  render () {
    const {isLoggedIn,storeList} = this.props.state
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" expand="md" light>
          <div className="containerNavBar">
            <div className="navBarDiv">
              <NavbarBrand tag={Link} to="/">Home</NavbarBrand>
              <label className="labelMargin">{`${this.props.state.userName ? "hello " + this.props.state.userName : ""}`}</label>
            </div>
            
            <div className="navBarSearch">
              <SearchComponent/>
            </div>
            
            <div className="navBarDiv">
              <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
              <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                <ul className="navbar-nav flex-grow">
                  {isLoggedIn ? null : 
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" exact to="/login">Login</NavLink>
                  </NavItem> }
  
                  {isLoggedIn ? 
                    <NavItem> 
                      <NavLink tag={Link} className="text-dark" exact to="/openStore">Add a Store</NavLink>
                    </NavItem>: null}
  
                  { storeList.length > 0  ?
                        <DropdownButton id="dropdown-basic-button" title="My Store List" className="dropdownMenu">
                          {storeList.map ((store) =>{
                            return(
                              <NavItem>
                                <NavLink tag={Link} className="text-dark" exact to={`/store/${store}`}>{store}</NavLink>
                              </NavItem>)})}
                         </DropdownButton> : null
                      }
                </ul>
              </Collapse>
              <NavLink tag={Link} className="text-dark" exact to="/Cart">
                <img src="/Images/cart.png" alt="Cart" class="image"/>
              </NavLink>
            </div>
          </div>
        </Navbar>

      </header>
    );
  }
}
