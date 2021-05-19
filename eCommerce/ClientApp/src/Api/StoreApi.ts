import axios from "axios";
import {Result} from "../Common";
import {
    ADD_ITEM_TO_STORE_PATH,
    EDIT_ITEM_IN_STORE_PATH,
    GET_ALL_ITEMS_IN_STORE_PATH,
    GET_ITEM_IN_STORE_PATH,
    GET_STORE_PERMISSION_FOR_USER_PATH,
    OPEN_STORE_PATH,
    REMOVE_ITEM_FROM_STORE_PATH,
    SEARCH_ITEMS_PATH,
    SEARCH_STORE_PATH,
    STAFF_OF_STORE_PATH,
    STAFF_PERMISSIONS_OF_STORE_PATH, UPDATE_MANAGER_PERMISSIONS_IN_STORE_PATH
} from "./ApiPaths";
import {Item} from "../Data/Item";
import {StorePermission} from "../Data/StorePermission";
import {StaffPermission} from "../Data/StaffPermission";

const instance = axios.create(
    {withCredentials : true}
);

export class StoreApi {
    
    openStore(storeId: string) {
        return instance.post<Result<any>>(OPEN_STORE_PATH, 
            {},
            {
                params: {
                    storeId: storeId
                }
            })
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
        }
        
   static async addItem(item: Item)
    {
        return instance.post<Result<any>>(ADD_ITEM_TO_STORE_PATH, item)
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }

    removeItem(storeId: string, itemId: string)
    {
        return instance.post<Result<any>>(REMOVE_ITEM_FROM_STORE_PATH, 
            {
                storeId: storeId,
                itemId: itemId
            })
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }

    editItem(item: Item)
    {
        return instance.post<Result<any>>(EDIT_ITEM_IN_STORE_PATH, item)
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }
    
    // ========== Store query ========== //

    getItem(storeId: string, itemId: string)
    {
        return instance.get<Result<Item>>(GET_ITEM_IN_STORE_PATH(storeId, itemId))
            .then(res => {
                return res.data
            })
            .catch(res => undefined)
    }

    getAllItems(storeId: string)
    {
        return instance.get<Result<Item[]>>(GET_ALL_ITEMS_IN_STORE_PATH(storeId))
            .then(res => {
                return new Result(res.data)
            }).catch(err => undefined)
    }
    
    searchItems(query: string){
        return instance.get<Result<Item[]>>(SEARCH_ITEMS_PATH,
            {
                params: {
                    query: query
                }
            })
            .then(res => {
                return new Result(res.data)
            }).catch(err => undefined)
    }

    searchStore(query: string){
        return instance.get<Result<string[]>>(SEARCH_STORE_PATH,
            {
                params: {
                    query: query
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    getStorePermissionForUser(storeId: string)
    {
        return instance.get<Result<StorePermission[]>>(GET_STORE_PERMISSION_FOR_USER_PATH(storeId))
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    // ========== Store staff ========== //
    
    appointOwner(storeId: string, appointedUserId: string){
        return instance.post<Result<any>>(STAFF_OF_STORE_PATH(storeId),
            {},
            {
                params: {
                    role: "owner",
                    userId: appointedUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }

    appointManager(storeId: string, appointedUserId: string){
        return instance.post<Result<any>>(STAFF_OF_STORE_PATH(storeId),
            {},
            {
                params: {
                    role: "manager",
                    userId: appointedUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }
    
    updateManagerPermissions(storeId: string, managerUserId: string,
                             permissions: StorePermission[]){
        return instance.put<Result<any>>(UPDATE_MANAGER_PERMISSIONS_IN_STORE_PATH(storeId),
            {
                storePermissions: permissions
            },
            {
                params: {
                    role: "manager",
                    userId: managerUserId
                }
            })
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }
    
    getStoreStaffPermissions(storeId: string){
        return instance.get<Result<StaffPermission[]>>(STAFF_PERMISSIONS_OF_STORE_PATH(storeId))
            .then(res => {
                return res.data
            }).catch(err => undefined)
    }
}