import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";
import {
    makeRuleNodeComposite, makeRuleNodeLeaf
} from '../Data/StorePolicies/RuleInfo'
import AddRule from "./AddRule";
import {Combinations, CombinationsNames} from "../Data/StorePolicies/Combinations";
import {makeDiscountCompositeNode, makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";
import Discount from "./Discount";

class AddDiscount extends Component {
    static displayName = AddDiscount.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            discount:0,
            selectedCombination:0,
            toggler:false,
            firstDiscount:undefined,
            secondDiscount:undefined,
            submitted:false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.toggle = this.toggle.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);


    }
    //
    // redirectToHome = (path) => {
    //     const { history } = this.props;
    //     if(history) history.push(path);
    // }



    async handleSubmit(event){
        const {firstDiscount,secondDiscount,selectedCombination,discount,toggler} = this.state
        const {storeId} = this.props
        event.preventDefault();
        if(firstDiscount) {
            let res = undefined
            let discount = firstDiscount
            if(toggler && secondDiscount){
                discount = makeDiscountCompositeNode(firstDiscount, secondDiscount, parseInt(selectedCombination));
            }
            res = await this.storeApi.addDiscountToStore(storeId, discount)

            if(res && res.isSuccess) {
                alert('add policy succeed')
                this.setState({
                    submitted:true
                })
            }
            else{
                if(res) {
                    alert(`add policy failed because- ${res.error}`)
                }
            }
        }


    }




    toggle(event){
        this.setState({
            toggler:!this.state.toggler
        })
    }

    addFirstDiscount(discount){
        this.setState({
            firstDiscount:discount
        })
    }

    addSecondDiscount(discount){
        this.setState({
            secondDiscount:discount
        })
    }



    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }
    render () {
        const {toggler,items,submitted} = this.state
        const {storeId} = this.props
        const combinatorValue = CombinationsNames[this.state.selectedCombination]
        if(submitted){
            return <Redirect exact to={`/store/${storeId}`}/>
        }
        else{
            return (
                // <main className="RegisterMain">
                <div className="RegisterWindow">
                    <h3>{`Add Discounts To The Policy Of The Store: ${storeId}`}</h3>
                    <form  onSubmit={this.handleSubmit}>
                        <Discount addDiscount={(discount) =>this.addFirstDiscount(discount)} storeId={storeId}/>
                        <button onClick={this.toggle}>{`${toggler? "Don't " : ''}Combine Another Discount`}</button>
                        {
                            toggler ?
                                <>
                                    <div>
                                        <label>
                                            Choose Combination:

                                            <select  onChange={this.handleInputChange} name="selectedCombination" className="searchContainer">
                                                {CombinationsNames.map((combination,index) => <option  value={index}>{combination}</option>)}
                                            </select>
                                        </label>
                                    </div>
                                    <Discount addDiscount={(discount) =>this.addSecondDiscount(discount)} storeId={storeId}/></>:
                                null
                        }
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="Add Policy"/>
                        </div>
                    </form>
                </div>
                // </main>
            );
        }
    }}

export default withRouter(AddDiscount);