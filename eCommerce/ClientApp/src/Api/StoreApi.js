import axios from "axios";
import {Result} from "../Common";
import {
    ADD_ITEM_TO_STORE_PATH,
    EDIT_ITEM_IN_STORE_PATH, GET_ALL_ITEMS_IN_STORE_PATH, GET_ITEM_IN_STORE_PATH,
    OPEN_STORE_PATH,
    REMOVE_ITEM_FROM_STORE_PATH
} from "./ApiPaths";

const instance = axios.create(
    {withCredentials : true}
);



export class StoreApi {
    
    static openStore(name,storeId,amount,category,keyWords,price) {
        return instance.post(OPEN_STORE_PATH,
            {
                itemName: name,
                storeName: storeId,
                amount: amount,
                category: category,
                keyWords: [keyWords],
                pricePerUnit: price
            })
            .then(res => {
                return new Result(res.data)

            })
        }

    static addItem(item)
    {
        return instance.post(ADD_ITEM_TO_STORE_PATH, item)
            .then(res => {
                return new Result(res.data)
            })
    }

    static removeItem(storeId, itemId)
    {
        return instance.post(REMOVE_ITEM_FROM_STORE_PATH, 
            {
                storeId: storeId,
                itemId: itemId
            })
            .then(res => {
                return new Result(res.data)
            })
    }

    static editItem(item)
    {
        return instance.post(EDIT_ITEM_IN_STORE_PATH, item)
            .then(res => {
                return new Result(res.data)
            })
    }
    
    // ========== Store query ========== //

    static getItem(storeId, itemId)
    {
        return instance.get(GET_ITEM_IN_STORE_PATH)
            .then(res => {
                return new Result(res.data)
            })
    }

    static getAllItems(storeId)
    {
        return instance.get(GET_ALL_ITEMS_IN_STORE_PATH(storeId))
            .then(res => {
                return new Result(res.data)
            }).catch(err => undefined)
    }
    
}