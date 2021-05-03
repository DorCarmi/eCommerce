// ========= Auth ========== //
export const AUTH_PATH = 'auth';
export const CONNECT_PATH = AUTH_PATH + '/connect';
export const LOGIN_PATH = AUTH_PATH + '/login';
export const REGISTER_PATH = AUTH_PATH + '/register';

// ========= Store ========== //

export const STORE_PATH = 'store'
export const OPEN_STORE_PATH = STORE_PATH + '/openStore'

// ========= Cart ========== //

export const CART_PATH = 'cart';
export const ADD_ITEM_TO_CART_PATH = CART_PATH + '/addItem';
export const EDIT_ITEM_IN_CART_PATH = CART_PATH + '/editItemAmount';
export const GET_CART_PATH = CART_PATH + '/getCart';
export const GET_PURCHASE_PRICE_CART_PATH = CART_PATH + "/getPurchasePrice";
export const PURCHASE_CART_PATH = CART_PATH + "/PurchaseCart"