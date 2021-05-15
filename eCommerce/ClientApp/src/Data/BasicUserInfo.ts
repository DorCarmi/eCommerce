export interface IBasicUserInfo {
    username: string,
    isLoggedIn: boolean
}

export class BasicUserInfo implements IBasicUserInfo {
    username: string;
    isLoggedIn: boolean;
    
    constructor(data: IBasicUserInfo) {
        this.username = data.username;
        this.isLoggedIn = data.isLoggedIn;

    }
    
    static create(username: string, isLoggedIn: boolean) {
        return new BasicUserInfo({
            username: username,
            isLoggedIn: isLoggedIn
        })

    }
}
