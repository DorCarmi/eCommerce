import React, {Component} from "react";
import {Menu, MenuItem, ProSidebar, SidebarContent, SidebarHeader, SubMenu} from "react-pro-sidebar";
import "./SideBarMenu.css"
import {Link} from "react-router-dom";
import {NavItem, NavLink} from "reactstrap";
import {UserRole} from "../Data/UserRole";

export class SideBarMenu extends Component {
    static displayName = SideBarMenu.name;

    constructor(props) {
        super(props);
    }
    
    renderOwnedStoreListMenu() {
        const { ownedStoreList } = this.props;
        if(!ownedStoreList || ownedStoreList.length === 0) {
            return null;
        }
        
        return (
            <SubMenu title="Owned Stores">
                {ownedStoreList.map ((store) =>
                    (
                        <MenuItem>
                            <NavLink tag={Link} exact to={`/store/${store}`}>{store}</NavLink>
                        </MenuItem>
                        
                    )
                )}
            </SubMenu>
        )
    }


    renderManagedStoreListMenu() {
        const { managedStoreList } = this.props;
        if(!managedStoreList ||managedStoreList.length === 0) {
            return null;
        }

        return (
            <SubMenu title="Managed Stores">
                {managedStoreList.map ((store) =>
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
        const { role } = this.props
        console.log(role)
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
                            {this.renderOwnedStoreListMenu()}
                            {this.renderManagedStoreListMenu()}
                        </SubMenu>
                    </Menu>

                    {role === UserRole.Admin ?
                        <Menu className="menuLayout">
                            <SubMenu title="Admin panel">
                            </SubMenu>
                        </Menu> :
                        null}
                </SidebarContent>
        </ProSidebar>)
    }
}