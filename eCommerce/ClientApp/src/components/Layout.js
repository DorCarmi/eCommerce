import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import {SideBarMenu} from "./SideBarMenu";

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    let { state } = this.props;
    console.log(state)
    return (
      <div>
        <NavMenu state={state}/>
        <div style={{
            display: 'flex',
            position: 'relative'
        }}>
            {state.isLoggedIn ? <SideBarMenu role={state.role} storeList={state.storeList}/> : null}
            <Container>
              {this.props.children}
            </Container>
        </div>
      </div>
    );
  }
}
