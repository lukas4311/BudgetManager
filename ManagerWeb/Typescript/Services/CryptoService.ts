import { CryptoApiInterface } from "../ApiClient/Main";

export default class CryptoService {
    private cryptoApi: CryptoApiInterface;

    constructor(cryptoApi: CryptoApiInterface) {
        this.cryptoApi = cryptoApi;
    }

    
}