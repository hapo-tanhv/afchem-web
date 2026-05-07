
import { AtscadaFetchService } from '../Common/atscada-fetch-service.js'
import { AtscadaDataCompiler } from './atscada-data-compiler.js'

export class AtscadaDataService {
    constructor(timeout = 3000) {
        this.timeout = timeout;
        this.service = new AtscadaFetchService();
        this.compiler = new AtscadaDataCompiler();
    }

    async read(names) {
        const url = '/DataRealTime/Read';
        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(names),
            timeout: this.timeout
        };
        const response = await this.service.handler(url, options);
        return this.compiler.decodeResultPackages(response);
    }

    async write(writeParams) {
        const encodeParam = this.compiler.encodeCommands(writeParams);
        const url = '/DataRealTime/Write';
        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(encodeParam),
            timeout: this.timeout
        };
        const response = await this.service.handler(url, options);
        return this.compiler.decodeResultPackages(response);
    }
}