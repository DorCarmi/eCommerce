import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import {Dropdown,DropdownButton} from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import {UserApi} from '../Api/UserApi'



export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
     super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {  
      collapsed: true,
      storeList:[],
      isLoggedIn:this.props.state.isLoggedIn
    };
  }


  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    const {isLoggedIn,storeList} = this.props.state
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">Website </NavbarBrand>
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
                  <DropdownButton id="dropdown-basic-button" title="My Cart">
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/GetCart">Get Cart</NavLink>
                    </NavItem>
                    
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/addItem">Add Item To Cart</NavLink>
                    </NavItem>
                    
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/EditItemAmountOfCart">Edit Item Amount Of Cart</NavLink>
                    </NavItem>
  
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/GetPurchaseCartPrice">Get Purchase Cart Price</NavLink>
                    </NavItem>
  
                    <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/GetPurchaseCartPrice">Get Purchase Cart Price</NavLink>
                  </NavItem>
                    
                    
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/PurchaseCart">Purchase Cart</NavLink>
                    </NavItem>
  
                  </DropdownButton>
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
          </Container>
        </Navbar>
      </header>
    );
  }
}
