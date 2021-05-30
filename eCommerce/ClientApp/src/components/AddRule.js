import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {withRouter} from "react-router-dom";
import {RuleType,RuleTypesOptions,makeRuleNodeLeaf,makeRuleInfo} from '../Data/StorePolicies/RuleInfo'
import {Item} from '../Data/Item'
import {Comperators,ComperatorsNames} from "../Data/StorePolicies/Comperators";

class AddRule extends Component {
    static displayName = AddRule.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            kind:'',
            ruleType:0,
            whatIsTheRuleOf:'',
            selectedComperator:0,
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) history.push(path);
    }

    async handleSubmit(event){
        const {ruleType,whatIsTheRuleOf,selectedComperator} = this.state
        const {itemId,storeId} = this.props
        const ruleTypeIdx=ruleType
        const comperatorIdx = selectedComperator
        console.log(ruleTypeIdx)
        console.log(comperatorIdx)
        console.log(Comperators.NOT_EQUALS)
        event.preventDefault();
        const res = await this.storeApi.addRuleToStorePolicy(storeId,makeRuleNodeLeaf(makeRuleInfo(ruleType,whatIsTheRuleOf,
                                                                                        itemId,selectedComperator)))

        if(res && res.isSuccess) {
            alert('edit item succeed')
            this.redirectToHome(`/store/${storeId}`)
        }
        else{
            if(res) {
                alert(`edit item failed because- ${res.error}`)
            }
        }
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }


    
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    render () {
        const {storeId,itemId} = this.props
        const comperatorValue = ComperatorsNames[this.state.selectedComperator]
        const ruleTypeValue = RuleTypesOptions[this.state.ruleType]
        console.log(ruleTypeValue)
        console.log(comperatorValue)
        console.log('&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&')
        return (
            <main className="RegisterMain">
                <div className="RegisterWindow">
                    <div className="CenterItemContainer">
                        <h3>{`Add Rule For Item: ${itemId} In Store: ${storeId}`}</h3>
                    </div>
                    <form  onSubmit={this.handleSubmit}>
                        <label>
                            Choose Rule Type:
                            <select  onChange={this.handleInputChange} name="ruleType" className="searchContainer">
                                {RuleTypesOptions.map((ruleType,index) => <option  value={index}>{ruleType}</option>)}
                            </select>
                        </label>
                        <div><input type="text" name="whatIsTheRuleOf" value={this.state.whatIsTheRuleOf} onChange={this.handleInputChange}
                               placeholder={'What Is The Rule Of'} required/></div>
                        <div><input type="text" name="kind" value={this.state.kind} onChange={this.handleInputChange}
                                    placeholder={'Enter Kind Of Rule'} required/></div>
                        <div><label>
                            Choose Comperator:
                            <select  onChange={this.handleInputChange} name="selectedComperator" className="searchContainer">
                                {ComperatorsNames.map((comperator,index) => <option  value={index}>{comperator}</option>)}
                            </select>
                        </label></div>
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="submit"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}

export default withRouter(AddRule);