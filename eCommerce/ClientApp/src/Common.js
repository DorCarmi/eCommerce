export class Result{
    constructor(resultData) {
        this.error = resultData.error;
        this.value = resultData.value;
        this.isFailure = resultData.isFailure;
        this.isSuccess = resultData.isSuccess;
    }
} 