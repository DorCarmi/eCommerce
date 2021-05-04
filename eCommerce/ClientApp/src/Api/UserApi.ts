import axios from "axios";
import {Result} from "../Common";
import {
GET_ALL_OWNED_STORES
} from "./ApiPaths";

const instance = axios.create(
    {withCredentials : true}
);



export class StoreApi {
    
    static getAllOwnedStoreIds() {
        return instance.get(GET_ALL_OWNED_STORES)
            .then(res => {
                return new Result<string[]>(res.data)
            })
        }
}