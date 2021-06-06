import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";
import {
    makeRuleNodeComposite, makeRuleNodeLeaf
} from '../Data/StorePolicies/RuleInfo'
import AddRule from "./AddRule";
import {CombinationsNames} from "../Data/StorePolicies/Combinations";
import {makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";

class AddPolicy extends Component {
    static displayName = AddPolicy.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            discount:0,
            selectedCombination:0,
            toggler:false,
            firstRule:undefined,
            secondRule:undefined,
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
        const {firstRule,secondRule,selectedCombination,discount} = this.state
        const {storeId} = this.props
        event.preventDefault();
        if(firstRule) {
            let res = undefined
            let rule = makeRuleNodeLeaf(firstRule)
            if(secondRule){
                rule = makeRuleNodeComposite(makeRuleNodeLeaf(firstRule), makeRuleNodeLeaf(secondRule), parseInt(selectedCombination));
            }
            const discountNodeLeaf = makeDiscountNodeLeaf(rule,parseInt(discount));
            res = await this.storeApi.addDiscountToStore(storeId, discountNodeLeaf)

            if(res && res.isSuccess) {
                alert('add discount succeed')
                this.setState({
                    submitted:true
                })
            }
            else{
                if(res) {
                    alert(`add discount failed because- ${res.error}`)
                }
            }
        }


    }


    

    toggle(event){
        this.setState({
            toggler:!this.state.toggler
        })
    }
    
    addFirstRule(rule){
        this.setState({
            firstRule:rule
        })
    }

    addSecondRule(rule){
        this.setState({
            secondRule:rule
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
            return <Redirect exact to="/"/>
        }
        else{
        return (
            // <main className="RegisterMain">
                <div className="RegisterWindow">
                        <h3>{`Add Policy For The Store: ${storeId}`}</h3>
                    <form  onSubmit={this.handleSubmit}>
                        <AddRule addRule={(rule) =>this.addFirstRule(rule)} storeId={storeId}/>
                         <button onClick={this.toggle}>{`${toggler? "Don't " : ''}Combine Another Rule`}</button>
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
                                <AddRule addRule={(rule) =>this.addSecondRule(rule)} storeId={storeId}/></>:
                                    null
                        }
                        <div><label>Enter Discount</label>
                            <input type="number" name="discount" value={this.state.discount} onChange={this.handleInputChange}
                                    placeholder={'Enter Discount'} required/></div>
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="Add Discount"/>
                        </div>
                    </form>
                </div>
            // </main>
        );
    }
}}

export default withRouter(AddPolicy);