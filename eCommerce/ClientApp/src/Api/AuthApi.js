import axios from "axios";
import {CONNECT_PATH, LOGIN_PATH, REGISTER_PATH} from "./ApiPaths";
import {Result} from "../Common";

const instance = axios.create(
    {withCredentials : true}
);

export class RedirectWithData {
    constructor(data, redirectLocation) {
        this.data = data;
        this.redirect = redirectLocation
    }
}

export class authApi {
    static Connect() {
        return instance.get(CONNECT_PATH)
            .then(res => {
                let data = res.data;
                return new RedirectWithData(data, res.headers['redirectto'])
            })
            .catch(res => undefined);
    }
    
    static Login(username, password, role) {
        return instance.post(LOGIN_PATH,
            {
                username: username,
                password: password,
                role: role
            })
            .then(res => {
                return new RedirectWithData(
                    new Result(res.data),
                    res.headers['redirectto']
                );
            })
            .catch(res => undefined);
    }
    
    static Register(username, password, email, name, address, birthday) {
        return instance.post(REGISTER_PATH,
            {
                username: username,
                password: password,
                email: email,
                name: name,
                address: address,
                birthday: birthday
            })
            .then(res => {
                return new RedirectWithData(
                    new Result(res.data),
                    res.headers['redirectto']
                );
            })
            .catch(res => undefined);
    }

    static isLoggedIn() {
        return instance.post(REGISTER_PATH,
            {})
            .then(res => {
                return new RedirectWithData(
                    new Result(res.data),
                    res.headers['redirectto']
                );
            })
            .catch(res => false);
    }
}