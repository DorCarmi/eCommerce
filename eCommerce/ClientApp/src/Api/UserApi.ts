import axios from "axios";
import {Result} from "../Common";
import {
    GET_ALL_MANAGED_STORES,
    GET_ALL_OWNED_STORES,
    GET_USER_BASIC_INFO_PATH
} from "./ApiPaths";
import {BasicUserInfo} from "../Data/BasicUserInfo";

const instance = axios.create(
    {withCredentials : true}
);

export class UserApi {
    
    getAllOwnedStoreIds() {
        return instance.get<Result<string[]>>(GET_ALL_OWNED_STORES)
            .then(res => {
                return res.data
            })
            .catch(err => {
                return undefined
            })
    }

    getAllManagedStoreIds() {
        return instance.get<Result<string[]>>(GET_ALL_MANAGED_STORES)
            .then(res => {
                return res.data
            })
            .catch(err => {
                return undefined
            })
    }
        
    getUserBasicInfo(){
        return instance.get<Result<BasicUserInfo>>(GET_USER_BASIC_INFO_PATH)
            .then(res => {
                return res.data.value;
            })
            .catch(err => {
                return undefined
            })
    }
}