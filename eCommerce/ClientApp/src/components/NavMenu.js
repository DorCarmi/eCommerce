import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink,Input } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import {Dropdown,DropdownButton} from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import {StoreApi} from "../Api/StoreApi";



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
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">Home </NavbarBrand>
            <label className="labelMargin">{`hello ${this.props.state.userName ? this.props.state.userName : ""}`}</label>

            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/login">{isLoggedIn ? null : "Login"}</NavLink>
                </NavItem>

                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Cart">
                    <img src="/Images/cart.png" alt="Cart" class="image"/>
                  </NavLink>
                </NavItem>

                {isLoggedIn? <>
                  <NavItem> 
                    <NavLink tag={Link} className="text-dark" to="/openStore">Add a Store</NavLink>
                  </NavItem> </>: null}

                {/*show stores*/}
                { storeList.length > 0  ?
                  <DropdownButton id="dropdown-basic-button" title="My Store List">
                    {storeList.map ((store) =>{
                      return(
                        <NavItem>
                          <NavLink tag={Link} className="text-dark" to={`/store/${store}`}>{store}</NavLink>
                        </NavItem>)})}
                   </DropdownButton> : null
                    }
              </ul>
            </Collapse>
            <form className="RegisterForm" onSubmit={this.handleSubmit}>
              <input type="text" name="itemToSearch" value={this.state.itemToSearch} onChange={this.handleInputChange} />
              <NavLink tag={Link} className="text-dark" to={`searchItems/${this.state.itemToSearch}`}>Search</NavLink>
            </form>
          </Container>
        </Navbar>

      </header>
    );
  }
}
