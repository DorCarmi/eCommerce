import axios from "axios";
import {Result} from "../Common";
import {OPEN_STORE_PATH} from "./ApiPaths";

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
                pricePerUnit:price
            })
            .then(res => {
                return new Result(res.data)
                
            })
            .catch(res => undefined);
    }


}