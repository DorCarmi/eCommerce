import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import {Dropdown,DropdownButton} from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';



export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true,
      isLoggedIn: false,
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    const {storeList,isLoggedIn} = this.props.state
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">Website {", hello" + this.state.username ? this.state.username : ""}</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/login">{this.props.state.isLoggedIn ? null : "Login"}</NavLink>
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
                {storeList.length > 0 ?
                  <DropdownButton id="dropdown-basic-button" title="My Store List">
                    {storeList.map ((store,index) =>{
                      return(
                        <NavItem>
                          <NavLink tag={Link} className="text-dark" to={`/store/${index}`}>{store}</NavLink>
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
