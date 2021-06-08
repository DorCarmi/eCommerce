import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StatsApi} from "../Api/StatsApi";

export default class ShowStatsOutput extends Component {
    static displayName = ShowStatsOutput.name;

    constructor(props) {
        super(props)
        this.state = {
            stats:[],
            date: undefined
        }
        this.StatsApi = new StatsApi()
    }
    

    async componentDidMount() {
        const fetchedStats = await this.StatsApi.loginStats(this.props.date)
        console.log("stats")
        console.log(fetchedStats)
        if (fetchedStats && fetchedStats.isSuccess) {
            this.setState({
                stats: fetchedStats.value.stat,
                date: this.props.date
            })
        }
    }
    
    render() {
        const {stats, date} = this.state
            return (
                date ?
                <div style={{marginTop: "10px"}}>
                    <h3>Statistics for date {date}</h3>
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
                                        <td>{stat.item1}</td>
                                        <td>{stat.item2}</td>
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
                </div> :
                    <h2>Empty stats</h2>
            );
    }
}
