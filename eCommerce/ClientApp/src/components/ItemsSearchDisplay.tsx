import React, {Component} from "react";
import {Item} from "../Data/Item";
import "./ItemDisplay.css"
import {ItemDisplay} from "./ItemDisplay";
import {StoreApi} from "../Api/StoreApi";

interface ItemsSearchDisplayProps {
    itemQuery: string
}

interface ItemsSearchDisplayState {
    items: Item[] | undefined
}

export class ItemSearchDisplay extends Component<ItemsSearchDisplayProps, ItemsSearchDisplayState> {
    static displayName = ItemSearchDisplay.name;
    private storeApi: StoreApi;

    constructor(props: ItemsSearchDisplayProps) {
        super(props);
        this.state = {
            items: undefined
        }
        this.storeApi = new StoreApi();
    }

    async componentDidMount() {
        const searchForItems = await this.storeApi.searchItems(this.props.itemQuery);

        if (searchForItems && searchForItems.isSuccess) {
            this.setState({
                items: searchForItems.value
            })
        }
    }

    render() {
        const { items } = this.state;
        
        return (
            <div className="itemsDisplay">
                {items ?
                    items.map((item) => <ItemDisplay item={item}/>) :
                    "Getting items"}
            </div>
        )
    }
}