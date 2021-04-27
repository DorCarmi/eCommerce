import axios from "axios";

export function Connect() {
    return axios.get('weatherforecast')
        .then(function (res) {
            console.log(res);
            return res
        });
}