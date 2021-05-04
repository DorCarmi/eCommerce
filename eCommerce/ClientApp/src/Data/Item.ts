export class Item {
    constructor(data) {
        this.itemName = data.itemName;
        this.storeName = data.storeName;
        this.amount = data.amount;
        this.category = data.category;
        this.keyWords = data.keyWords;
        this.pricePerUnit = data.pricePerUnit;
    }
}