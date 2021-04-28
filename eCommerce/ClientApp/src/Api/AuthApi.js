import axios from "axios";

export function Connect() {
    return axios.get('Auth/Connect')
        .then(res => res.data)
        .catch(res => undefined);
}