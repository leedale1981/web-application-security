import logo from './logo.svg';
import './App.css';
import { BrowserRouter, Switch, Route } from "react-router-dom";
import Hello from './Hello';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <BrowserRouter>
          <Switch>
            <Route
              path="/hello/:message"
              render={() => (
                <Hello />
              )}
            />
          </Switch>
        </BrowserRouter>
      </header>
    </div>
  );
}

export default App;
