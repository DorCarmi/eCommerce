import React, {Component} from "react";
import {Menu, MenuItem, ProSidebar, SidebarContent, SidebarHeader, SubMenu} from "react-pro-sidebar";
import "./SideBarMenu.css"
import {Link} from "react-router-dom";
import {NavItem, NavLink} from "reactstrap";

export class SideBarMenu extends Component {
    static displayName = SideBarMenu.name;

    constructor(props) {
        super(props);
    }
    
    renderStoreListMenu() {
        const { storeList } = this.props;
        if(storeList.length === 0) {
            return null;
        }
        
        return (
            <SubMenu title="My stores">
                {storeList.map ((store) =>
                    (
                        <MenuItem>
                            <NavLink tag={Link} exact to={`/store/${store}`}>{store}</NavLink>
                        </MenuItem>
                    )
                )}
            </SubMenu>
        )
    }
    
    render() {
        return (
            <ProSidebar breakPoint="md" className="sideBarContainer">
                <SidebarHeader>
                    <div className="sideBarHeaderContainer">
                        Managment Panel
                    </div>
                </SidebarHeader>
    
                <SidebarContent>
                    <Menu className="menuLayout">
                        <SubMenu title="Member panel">
                        <MenuItem>
                            <NavLink tag={Link} exact to="/openStore">Create new store</NavLink>
                        </MenuItem>
                        {this.renderStoreListMenu()}
                        </SubMenu>
                    </Menu>

                    <Menu className="menuLayout">
                        <SubMenu title="Admin panel">
                        </SubMenu>
                    </Menu>
                </SidebarContent>
        </ProSidebar>)
    }
}