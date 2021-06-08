import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {Link, Redirect} from "react-router-dom";
import {UserApi} from "../Api/UserApi";
import {Item} from "../Data/Item";
import {StorePermission} from '../Data/StorePermission'
import {NavLink} from "reactstrap";
import {StatsApi} from "../Api/StatsApi";

export default class ShowStatsOutput extends Component {
    static displayName = ShowStatsOutput.name;

    constructor(props) {
        super(props)
        this.state = {
            stats:[]
        }
        this.StatsApi = new StatsApi()
    }
    

    async componentDidMount() {
        const fetchedStats = await this.StatsApi.loginStats(this.props.date)
        console.log(fetchedStats)
        if (fetchedStats && fetchedStats.isSuccess) {
            this.setState({
                stats: fetchedStats.value.stat
            })
        }
    }



    

    render() {
        const {stats} = this.state
            return (
                <div>
                    <Table striped bordered hover>
                        <thead>
                        <tr>
                            <th>User Type</th>
                            <th>Amount</th>
                        </tr>
                        </thead>
                        <tbody>

                        {
                            stats.map((stat) => {
                                return (
                                    <tr>
                                        <td>{stat.Item1}</td>
                                        <td>{stat.Item2}</td>
                                    </tr>
                                )
                            })
                        }
                            {/*{*/}
                            {/*    stats.map((stat) => {*/}
                            {/*        return (<tr>*/}
                            {/*            <td>{stat.Item1}</td>*/}
                            {/*            <td>{stat.Item2}</td>*/}
                            {/*        </tr>)*/}
                            {/*    })*/}
                            {/*}*/}
                        </tbody>
                    </Table>
                </div>
            );
    }
}
