export interface Tuple<T1, T2> {
    Item1: T1,
    Item2: T2
}

export interface LoginStatsInterface {
    statsArr: Tuple<string, number>[] 
}

export class LoginStats implements LoginStatsInterface{
    statsArr: Tuple<string, number>[];
    
    constructor(data: LoginStatsInterface) {
        this.statsArr = data.statsArr
    }
}