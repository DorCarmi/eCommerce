import React, { Component } from 'react';
import WebSocketExample from "./WebSocketExample";

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Welcome to the greadt ecommerce store</h1>
        <WebSocketExample/>
      </div>
    );
  }
}
