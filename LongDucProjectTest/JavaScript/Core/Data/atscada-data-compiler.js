
import { AtscadaDataResultPackage } from './atscada-data-result-package.js'

export class AtscadaDataCompiler {
    decodeResultPackages(response) {
        response = response || {};
        const resultPackages = [];
        if (!this.checkFailed(response)) {
            for (const result of response['Result']) {
                resultPackages.push(
                    new AtscadaDataResultPackage(result));
            }
        }
        return resultPackages;
    }

    encodeCommands(commands) {
        commands = commands || [];
        const encodeParam = [];
        for (const command of commands) {
            encodeParam.push({
                'Name': command.name,
                'Value': command.value
            });
        }
        return encodeParam;
    }

    checkFailed(response) {
        return !response || !response['Status'] || !response['Result'];
    }
}