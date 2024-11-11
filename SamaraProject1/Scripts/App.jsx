import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './components/Home';
import Producto from './components/Producto';
import Emprendedor from './components/Emprendedor';

const App = () => {
    return (
        <Router>
            <Layout>
                <Switch>
                    <Route exact path="/" component={Home} />
                    <Route path="/producto" component={Producto} />
                    <Route path="/emprendedor" component={Emprendedor} />
                </Switch>
            </Layout>
        </Router>
    );
};

export default App;