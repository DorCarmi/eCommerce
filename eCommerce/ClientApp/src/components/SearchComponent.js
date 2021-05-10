import React, {ChangeEvent, Component} from 'react';
import "./SearchComponent.css";
import {Link} from "react-router-dom";
import {NavLink} from "reactstrap";


export class SearchComponent extends Component {
    static displayName = SearchComponent.name;
    
    constructor(props) {
        super(props);
        this.state = {
            searchQuery: ""
        }
        
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    handleInputChange(event){
        const value = event.target.value;
        this.setState({
            searchQuery: value
        });
    }
    
    render(){
        return (
            <div className="searchContainer">
                <select className="searchOptions">
                    <option>Item</option>
                </select>
                <input placeholder="Search" value={this.state.searchQuery} onChange={this.handleInputChange}/>
                <Link className="searchLink" exact to={`/searchItems/${this.state.searchQuery}`}>
                    <div className="imageDiv">
                        <img src="/Images/search.png" alt="Search" className="imageFitSize"/>
                    </div>
                </Link>
            </div>
        )
    }
}
