
import { AtscadaDataResultPackage } from './atscada-data-result-package.js'

export class AtscadaDataReader {
    constructor(dataService) {
        this.dataService = dataService;
        this.map = new Map();
    }

    read(name) {
        if (this.map.has(name)) return this.map.get(name);
        const resultPackage = new AtscadaDataResultPackage();
        resultPackage['Name'] = name;
        return resultPackage;
    }

    async update(names) {
        this.map.clear();
        const resultPackages = await this.dataService.read(names);
        for (const resultPackage of resultPackages) {
            if (resultPackage['Name'])
                this.map.set(
                    resultPackage['Name'],
                    resultPackage);
        }
    }
}