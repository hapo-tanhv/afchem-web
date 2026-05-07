
export class AtscadaDataWriter {
    constructor(dataService, maxNumberOfWrite = 10) {
        this.dataService = dataService;
        this.maxNumberOfWrite = maxNumberOfWrite;
        this.map = new Map();        
    }

    write(command) {
        command = command || {};
        if (!command.name) return;
        command.maxNumberOfWrite = this.maxNumberOfWrite;
        this.map.set(command.name, command);
    }

    async update() {
        if (this.map.size === 0) return;
        const resultPackages = await this.dataService.write([...this.map.values()]);        
        for (const [name, command] of this.map) {
            const resultPackage = resultPackages.find(resultPackage =>
                resultPackage['Name'] === name);
            command.update(resultPackage);
            if (command.isCompleted) this.map.delete(name);
        }
    }
}