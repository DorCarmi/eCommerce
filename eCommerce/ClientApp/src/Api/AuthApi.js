import axios from "axios";
import {CONNECT_PATH, LOGIN_PATH} from "./ApiPaths";
import {Result} from "../Common";

const instance = axios.create(
    {withCredentials : true}
);

export class ConnectData {
    constructor(token, redirectLocation) {
        this.token = token;
        this.redirect = redirectLocation
    }
}

export class authApi {
    static Connect() {
        return instance.get(CONNECT_PATH)
            .then(res => {
                let data = res.data;
                return new ConnectData(data, res.headers['redirectto'])
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
                return new Result(res.data)
            })
            .catch(res => undefined);
    }
}