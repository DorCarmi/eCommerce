import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {withRouter} from "react-router-dom";
import {
    makeRuleNodeComposite
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
            items:[]
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
    
    async componentDidMount() {
        const fetchedItems = await this.storeApi.getAllItems(this.props.storeId)
        if (fetchedItems && fetchedItems.isSuccess) {
            this.setState({
                items: fetchedItems.value
            })
        }

    }

    async handleSubmit(event){
        const {firstRule,secondRule,selectedCombination,discount} = this.state
        const {storeId} = this.props
        event.preventDefault();
        if(firstRule) {
            let res = undefined
            let rule = firstRule
            if(secondRule){
                rule = makeRuleNodeComposite(firstRule, secondRule, parseInt(selectedCombination));
            }
            const discountNodeLeaf = makeDiscountNodeLeaf(rule,discount);
            res = await this.storeApi.addDiscountToStore(storeId, discountNodeLeaf)

            if(res && res.isSuccess) {
                alert('add discount succeed')
                this.redirectToHome(`/store/${storeId}`)
            }
            else{
                if(res) {
                    alert(`add discount failed because- ${res.error}`)
                }
            }
        }


        this.handleSubmit = this.handleSubmit.bind(this);
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
        const {toggler,items} = this.state
        const {storeId} = this.props
        const combinatorValue = CombinationsNames[this.state.selectedCombination]

        return (
            // <main className="RegisterMain">
                <div className="RegisterWindow">
                        <h3>{`Add Policy For The Store: ${storeId}`}</h3>
                    <form  onSubmit={this.handleSubmit}>
                        <label>
                            Choose An Item:
                            <select  onChange={this.handleChange} name="selectedCombination" className="searchContainer">
                                {items.map((item) => <option  value={item}>{item}</option>)}
                            </select>
                        </label>
                        <AddRule addRule={(rule) =>this.addFirstRule(rule)}/>
                         <button onClick={this.toggle}>{`${toggler? "Don't " : ''}Combine Another Rule`}</button>
                        {
                            toggler ?
                                <>
                                <div>
                                    <label>
                                        Choose Compbination:
                                        
                                        <select  onChange={this.handleChange} name="selectedCombination" className="searchContainer">
                                            {CombinationsNames.map((combination,index) => <option  value={index}>{combinatorValue}</option>)}
                                        </select>
                                    </label>
                                </div>
                                <AddRule addRule={(rule) =>this.addSecondRule(rule)}/></>:
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
}

export default withRouter(AddPolicy);