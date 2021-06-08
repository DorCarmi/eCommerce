export interface Tuple<T1, T2> {
    item1: T1,
    item2: T2
}

export interface LoginStatsInterface {
    stat: Tuple<string, number>[] 
}

export class LoginStats implements LoginStatsInterface{
    stat: Tuple<string, number>[];
    
    constructor(data: LoginStatsInterface) {
        this.stat = data.stat
    }
}