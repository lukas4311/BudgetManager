/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./Typescript/Components/Menu.tsx");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./Typescript/Components/Menu.tsx":
/*!****************************************!*\
  !*** ./Typescript/Components/Menu.tsx ***!
  \****************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const react_1 = __importDefault(__webpack_require__(/*! react */ "react"));
const react_dom_1 = __importDefault(__webpack_require__(/*! react-dom */ "react-dom"));
class Menu extends react_1.default.Component {
    constructor(props) {
        super(props);
        this.redirectToPage = (link) => {
            window.location.href = '/' + link;
        };
        this.menuClick = this.menuClick.bind(this);
        this.state = { isClosed: true };
    }
    menuClick() {
        this.setState((prev) => ({ isClosed: !prev.isClosed }));
    }
    render() {
        return (react_1.default.createElement(react_1.default.Fragment, null,
            react_1.default.createElement("a", { className: "menuTogglerParent", onClick: this.menuClick },
                react_1.default.createElement("span", { className: "menu-toggler " + (this.state.isClosed ? "" : "checked"), id: "menu-toggler" })),
            react_1.default.createElement("ul", null,
                react_1.default.createElement("li", { className: "menu-item" },
                    react_1.default.createElement("a", { className: "fa fa-facebook", href: "https://www.facebook.com/", target: "_blank" })),
                react_1.default.createElement("li", { className: "menu-item" },
                    react_1.default.createElement("a", { className: "fa fa-google", href: "https://www.google.com/", target: "_blank" })),
                react_1.default.createElement("li", { className: "menu-item" },
                    react_1.default.createElement("a", { className: "fa fa-dribbble", href: "https://dribbble.com/", target: "_blank" })),
                react_1.default.createElement("li", { className: "menu-item" },
                    react_1.default.createElement("a", { className: "fa fa-codepen", href: "https://codepen.io/", target: "_blank" })),
                react_1.default.createElement("li", { className: "menu-item" },
                    react_1.default.createElement("a", { className: "fa fa-linkedin", href: "https://www.linkedin.com/", target: "_blank" })),
                react_1.default.createElement("li", { className: "menu-item" },
                    react_1.default.createElement("a", { className: "fa fa-github", href: "https://github.com/", target: "_blank" })))));
    }
}
react_dom_1.default.render(react_1.default.createElement(Menu, null), document.getElementById('navMenu'));


/***/ }),

/***/ "react":
/*!************************!*\
  !*** external "React" ***!
  \************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = React;

/***/ }),

/***/ "react-dom":
/*!***************************!*\
  !*** external "ReactDOM" ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ReactDOM;

/***/ })

/******/ });
//# sourceMappingURL=menu.js.map