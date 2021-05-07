import axios from "axios";
import {Result} from "../Common";
import {
    GET_ALL_OWNED_STORES,
    GET_USER_BASIC_INFO_PATH
} from "./ApiPaths";
import {BasicUserInfo} from "../Data/BasicUserInfo";

const instance = axios.create(
    {withCredentials : true}
);



export class UserApi {
    
    static getAllOwnedStoreIds() {
        return instance.get(GET_ALL_OWNED_STORES)
            .then(res => {
                return new Result<string[]>(res.data)
            })
            .catch(err => {
                return undefined
            })
    }
        
    static getUserBasicInfo(){
        return instance.get<Result<BasicUserInfo>>(GET_USER_BASIC_INFO_PATH)
            .then(res => {
                return res.data;
            })
            .catch(err => {
                return undefined
            })
    }
}