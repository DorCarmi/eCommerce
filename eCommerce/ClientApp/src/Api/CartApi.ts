import axios from "axios";
import {Result} from "../Common";
import {
    ADD_ITEM_TO_CART_PATH,
    EDIT_ITEM_IN_CART_PATH,
    GET_CART_PATH,
    GET_PURCHASE_PRICE_CART_PATH,
    PURCHASE_CART_PATH
} from "./ApiPaths";
import {CartData} from "../Data/CartData";

const instance = axios.create(
    {withCredentials : true}
);

export class CartApi {

    static getCart() {
        return instance.get(GET_CART_PATH)
            .then(res => {
                res = new Result(res.data)
                if(res.isSuccess){
                    res.value = new CartData(res.value)
                }
                return res;
            })
    };

    static AddItem(itemId, storeId, amount) {
        return instance.post(ADD_ITEM_TO_CART_PATH, 
            {
                itemId: itemId,
                storeId: storeId,
                amount: amount
            })
            .then(res => {
                return new Result(res.data)
            })
    };

    static EditItemAmount(itemId, storeId, amount) {
        return instance.post(EDIT_ITEM_IN_CART_PATH,
            {
                itemId: itemId,
                storeId: storeId,
                amount: amount
            })
            .then(res => {
                return new Result(res.data)
            })
    };

    static GetPurchasePrice() {
        return instance.get(GET_PURCHASE_PRICE_CART_PATH)
            .then(res => {
                return new Result(res.data)
            })
    };

    static PurchasePrice(userName, idNumber, creditCardNumber, 
                         creditCardExpirationDate, threeDigitsOnBackOfCard, 
                         fullAddress) {
        return instance.post(PURCHASE_CART_PATH,
            {
                userName: userName,
                IDNumber: idNumber,
                CreditCardNumber: creditCardNumber,
                CreditCardExpirationDate: creditCardExpirationDate,
                ThreeDigitsOnBackOfCard: threeDigitsOnBackOfCard,
                FullAddress: fullAddress
            })
            .then(res => {
                return new Result(res.data)
            })
    };
}