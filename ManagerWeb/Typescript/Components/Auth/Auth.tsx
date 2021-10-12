import React from "react";
import ReactDOM from "react-dom";

class Auth extends React.Component<{}, {}>{

    render = () => {
        return (
            <div className="m-auto text-center">
                <h1 className="text-2xl">Přihlášení</h1>
                <form className="flex flex-col w-2/5 m-auto mt-8">
                    <div asp-validation-summary="All" className="text-red-600 mb-4"></div>
                    <div className="flex">
                        <div className="w-1/2">
                            <div className="relative inline-block w-2/3 m-auto">
                                <input className="effect-11 w-full" placeholder="Login" />
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                        <div className="w-1/2">
                            <div className="relative inline-block w-2/3 m-auto">
                                <input type="password" className="effect-11 w-full" placeholder="Heslo" />
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                    </div>
                    <div className="flex mt-8">
                        <button type="submit" className="m-auto bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500">Potvrdit</button>
                    </div>
                </form>
            </div>
        );
    }
}

ReactDOM.render(<Auth />, document.getElementById('auth'));