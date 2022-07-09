import { useParams } from "react-router-dom";

function Hello() {
    let { message } = useParams(); // This parameter is protected from XSS by react-router
    let anotherParam = new URLSearchParams(window.location.search).get("xssparam"); // This parameter is protected from XSS by URLSearchParams
    let xssparam = window.location.search.split('xssparam=')[1];

    return (
        <>
        <h1>{message}</h1>
        <h2>{anotherParam}</h2>
        <h3>{xssparam}</h3>
        </>
    )
}

export default Hello;