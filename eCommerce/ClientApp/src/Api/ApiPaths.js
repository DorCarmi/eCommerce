const API_PATH = "api"

// ========= Auth ========== //
export const AUTH_PATH = API_PATH + '/auth';
export const CONNECT_PATH = AUTH_PATH + '/connect';
export const LOGIN_PATH = AUTH_PATH + '/login';
export const REGISTER_PATH = AUTH_PATH + '/register';

// ========= Store ========== //

export const STORE_PATH = API_PATH + '/store';
export const OPEN_STORE_PATH = STORE_PATH + '/openStore';
export const ADD_ITEM_TO_STORE_PATH = STORE_PATH + '/addItem';
export const REMOVE_ITEM_FROM_STORE_PATH = STORE_PATH + '/removeItem';
export const EDIT_ITEM_IN_STORE_PATH = STORE_PATH + '/editItem';
export function GET_ITEM_IN_STORE_PATH(storeId, itemId) { return  STORE_PATH + `/${storeId}/${itemId}`; }
export function GET_ALL_ITEMS_IN_STORE_PATH(storeId){ return STORE_PATH + '/getAllItems/' + storeId }
export const SEARCH_ITEMS_PATH = STORE_PATH + '/search';
export const SEARCH_STORE_PATH = STORE_PATH + '/searchStore';
export function STAFF_OF_STORE_PATH(storeId){ return STORE_PATH + `/${storeId}/staff` }



// ========= Cart ========== //

export const CART_PATH = API_PATH + '/cart';
export const ADD_ITEM_TO_CART_PATH = CART_PATH + '/addItem';
export const EDIT_ITEM_IN_CART_PATH = CART_PATH + '/editItemAmount';
export const GET_CART_PATH = CART_PATH + '/getCart';
export const GET_PURCHASE_PRICE_CART_PATH = CART_PATH + "/getPurchasePrice";
export const PURCHASE_CART_PATH = CART_PATH + "/PurchaseCart";

// ========= User ========== //

export const USER_PATH = API_PATH + '/user';
export const GET_ALL_OWNED_STORES = USER_PATH + '/getALlStoreIds';
export const GET_USER_BASIC_INFO_PATH = USER_PATH + '/getUserBasicInfo';