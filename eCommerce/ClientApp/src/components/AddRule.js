import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";
import {
    makeRuleNodeComposite, makeRuleNodeLeaf
} from '../Data/StorePolicies/RuleInfo'
import {CombinationsNames} from "../Data/StorePolicies/Combinations";
import {makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";
import Rule from "./Rule";

class AddRule extends Component {
    static displayName = AddRule.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
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
        const {firstRule,secondRule,selectedCombination,toggler} = this.state
        const {storeId} = this.props
        event.preventDefault();
        if(firstRule) {
            let rule = makeRuleNodeLeaf(firstRule)
            if(toggler && secondRule){
                rule = makeRuleNodeComposite(rule, makeRuleNodeLeaf(secondRule), parseInt(selectedCombination));
            }
            const res = await this.storeApi.addRuleToStorePolicy(storeId, rule)

            if(res && res.isSuccess) {
                alert('add rule succeed')
                this.setState({
                    submitted:true
                })
            }
            else{
                if(res) {
                    alert(`add rule failed because- ${res.error}`)
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
        const {toggler,submitted} = this.state
        const {storeId} = this.props
        if(submitted){
            return <Redirect exact to={`/store/${storeId}`}/>
        }
        else{
            return (
                // <main className="RegisterMain">
                <div className="RegisterWindow">
                    <h3>{`Add Rule To Store Policy`}</h3>
                    <form  onSubmit={this.handleSubmit}>
                    <Rule addRule={(rule) =>this.addFirstRule(rule)} storeId={storeId}/>
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
                                <Rule addRule={(rule) =>this.addSecondRule(rule)} storeId={storeId}/></>:
                            null
                    }
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="Add Rule To Policy"/>
                        </div>
                    </form>
                </div>
                // </main>
            );
        }
    }}

export default withRouter(AddRule);