import React, {Component} from "react";
import {Menu, MenuItem, ProSidebar, SidebarContent, SidebarHeader} from "react-pro-sidebar";
import "./SideBarMenu.css"

export class SideBarMenu extends Component {
    static displayName = SideBarMenu.name;

    render() {
        return (
            <ProSidebar
            breakPoint="md"
            style={{
                height: '100%',
                width: '270px',
                backgroundColor: '#e1e1e1'
            }}>
                <SidebarHeader>
                    <div
                        style={{
                            padding: '24px',
                            textTransform: 'uppercase',
                            fontWeight: 'bold',
                            fontSize: 14,
                            letterSpacing: '1px',
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                            whiteSpace: 'nowrap',
                        }}
                    >
                        "hello"
                    </div>
                </SidebarHeader>
    
                <SidebarContent>
                    <Menu iconShape="circle">
                        <MenuItem
                            suffix={<span className="badge red">A</span>}
                        >
                            dashboard
                        </MenuItem>
                    </Menu>
                </SidebarContent>
        </ProSidebar>)
    }
}