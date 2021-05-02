import axios from "axios";
import {Result} from "../Common";
import {GET_CART_PATH} from "./ApiPaths";

const instance = axios.create(
    {withCredentials : true}
);

export class CartApi {

    static getCart() {
        return instance.get(GET_CART_PATH)
            .then(res => {
                return new Result(res.data)

            })
            .catch(res => res);
    }
}